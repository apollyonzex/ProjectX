
using GraphNode.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphNode;

namespace BehaviourFlow.Editor {

    [GraphEditor(typeof(BehaviourTree))]
    public class BehaviourTreeEditor : GenericGraphEditor {

        public new BehaviourTree graph => base.graph as BehaviourTree;

        public override void on_open() {
            base.on_open();
            var graph = this.graph;
            if (graph.externals == null) {
                graph.externals = new Dictionary<string, Nodes.ExternalNode>();
            }
            if (view.runtime) {
                m_runtime = new Runtime();
                m_runtime.init(this);
            }
        }

        public override void attach(Graph graph, GraphView view) {
            base.attach(graph, view);
            if (try_get_property("constants", out var pe)) {
                pe.on_changed += (_pe, _) => rebuild_constant_indices();
            }
            build_constant_indices();
        }

        public override void on_close() {
            base.on_close();
            if (m_runtime != null) {
                m_runtime.fini();
                m_runtime = null;
            }

        }

        public override void on_add_node(NodeView node) {
            base.on_add_node(node);
            m_runtime?.on_node_added(node);
        }

        public override void on_remove_node(NodeView node) {
            base.on_remove_node(node);
            m_runtime?.on_node_removed(node);
        }

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            if (m_runtime != null) {
                if (m_runtime.root != null) {
                    var context = m_runtime.root.context;
                    GUILayout.BeginVertical(GUI.skin.box);
                    var dict = Utility.get_expression_externals(graph.context_type);
                    foreach (var kvp in dict) {
                        kvp.Value.get_value(context, context.context_type, out var val);
                        EditorGUILayout.LabelField(kvp.Key, val?.ToString());
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUI.skin.box);
                    var shared_int_iter = context.enumerate_shared_ints();
                    while (shared_int_iter.MoveNext()) {
                        var (name, value) = shared_int_iter.Current;
                        EditorGUILayout.LabelField($"shared_int.{name}", value.ToString());
                    }
                    var shared_float_iter = context.enumerate_shared_floats();
                    while (shared_float_iter.MoveNext()) {
                        var (name, value) = shared_float_iter.Current;
                        EditorGUILayout.LabelField($"shared_float.{name}", value.ToString());
                    }
                    GUILayout.EndVertical();
                }
            } else {
                if (GUILayout.Button("Export")) {
                    var path = EditorUtility.SaveFilePanel("Export", null, view.window.asset.name, "bt");
                    if (!string.IsNullOrEmpty(path)) {
                        try {
                            var exporter = new Exports.Exporter(graph);
                            using (var writer = new System.IO.BinaryWriter(System.IO.File.Create(path))) {
                                exporter.save_to(writer);
                            }
                            view.window.ShowNotification(new GUIContent("Exported"));
                        } catch (System.IO.IOException) {
                            view.window.ShowNotification(new GUIContent("Exported Failed"));
                        }
                    }
                }
            }
        }

        public bool try_get_constant(string name, out Constant constant) {
            return m_constant_indices.TryGetValue(name, out constant);
        }

        public void rebuild_constant_indices() {
            m_constant_indices.Clear();
            build_constant_indices();
            foreach (var node in view.nodes) {
                if (node.editor is BTNodeEditor ne) {
                    ne.rebuild_all_expressions();
                }
            }
        }

        private void build_constant_indices() {
            foreach (var e in graph.constants) {
                if (string.IsNullOrEmpty(e.name) || m_constant_indices.ContainsKey(e.name)) {
                    continue;
                }
                m_constant_indices.Add(e.name, e);
            }
        }

        private Dictionary<string, Constant> m_constant_indices = new Dictionary<string, Constant>();

        private class Runtime {
            public BehaviourTreeEditor editor;
            public BTExecutorBase root;
            public HashSet<BTExecutorBase> executors = new HashSet<BTExecutorBase>();
            public Dictionary<BTNode, BTNodeEditor> nodes = new Dictionary<BTNode, BTNodeEditor>();

            public static readonly Color NORMAL_COLOR = new Color32(90, 97, 105, 255);
            public static readonly Color PENDING_COLOR = new Color32(144, 128, 0, 255);
            public static readonly Color SUCCESS_COLOR = new Color32(0, 128, 0, 255);
            public static readonly Color FAILED_COLOR = new Color32(128, 0, 0, 255);
            public static readonly Color ABORT_COLOR = new Color32(32, 32, 32, 255);

            public void init(BehaviourTreeEditor editor) {
                this.editor = editor;
                BTExecutorBase.instance_event += on_instance_event;
                root = BehaviourTreeDebugger.instance?.current;
                EditorApplication.playModeStateChanged += on_play_mode_changed;
                EditorApplication.pauseStateChanged += on_pause_changed;
                EditorApplication.update += update;
                m_last_update_time = EditorApplication.timeSinceStartup;
                if (root != null) {

                    foreach (var e in root.enumerate_executors()) {
                        executors.Add(e);
                        e.node_event += on_node_event;
                    }
                    foreach (var node_view in editor.view.nodes) {
                        if (node_view.editor is BTNodeEditor ne) {
                            nodes.Add(ne.node as BTNode, ne);
                        }
                    }
                    foreach (var e in executors) {
                        foreach (var (asset, node) in e.pending_nodes) {
                            if (asset == editor.view.window.asset && nodes.TryGetValue(node, out var ne)) {
                                ne._set_node_color(PENDING_COLOR);
                            }
                        }
                    }
                }
            }

            private void on_pause_changed(PauseState obj) {
                if (obj == PauseState.Paused) {
                    m_paused_frame = Time.renderedFrameCount;
                    update_nodes_in_paused();
                }
            }

            private void on_play_mode_changed(PlayModeStateChange obj) {
                
            }

            public void fini() {
                EditorApplication.playModeStateChanged -= on_play_mode_changed;
                EditorApplication.pauseStateChanged -= on_pause_changed;
                EditorApplication.update -= update;
                BTExecutorBase.instance_event -= on_instance_event;
                foreach (var e in executors) {
                    e.node_event -= on_node_event;
                }
            }

            public void on_node_added(NodeView node_view) {
                if (node_view.editor is BTNodeEditor ne) {
                    nodes.Add(ne.node as BTNode, ne);
                }
            }

            public void on_node_removed(NodeView node_view) {
                if (node_view.editor is BTNodeEditor ne) {
                    nodes.Remove(ne.node as BTNode);
                    ne._set_node_color(NORMAL_COLOR);
                }
            }

            private void on_instance_event(BTExecutorBase exec, bool added) {
                if (exec.root == root) {
                    if (added) {
                        executors.Add(exec);
                        exec.node_event += on_node_event;
                    } else {
                        executors.Remove(exec);
                        exec.node_event -= on_node_event;
                        if (root == exec) {
                            root = null;
                        }
                    }
                }
            }

            private void on_node_event(BehaviourTreeAsset asset, BTChildNode node, BTExecutorBase.NodeEventType type) {
                if (asset == editor.view.window.asset && nodes.TryGetValue(node, out var ne)) {
                    var color = NORMAL_COLOR;
                    bool need_update = false;
                    switch (type) {
                        case BTExecutorBase.NodeEventType.Pending:
                            color = PENDING_COLOR;
                            if (ne.runtime_break_when_pending) {
                                Debug.Break();
                            }
                            break;
                        case BTExecutorBase.NodeEventType.Success:
                            color = SUCCESS_COLOR;
                            need_update = true;
                            if (ne.runtime_break_when_success) {
                                Debug.Break();
                            }
                            break;
                        case BTExecutorBase.NodeEventType.Failed:
                            color = FAILED_COLOR;
                            need_update = true;
                            if (ne.runtime_break_when_failed) {
                                Debug.Break();
                            }
                            break;
                        case BTExecutorBase.NodeEventType.Abort:
                            color = ABORT_COLOR;
                            need_update = true;
                            if (ne.runtime_break_when_abort) {
                                Debug.Break();
                            }
                            break;
                    }
                    ne._set_node_color(color);
                    if (need_update) {
                        ne.runtime_color = color;
                        ne.runtime_fading = 1;
                        add_updating_node(ne);
                    } else {
                        remove_updating_node(ne);
                        m_need_repaint = true;
                    }
                }
            }

            private void update() {
                var dt = (float)(EditorApplication.timeSinceStartup - m_last_update_time);
                m_last_update_time = EditorApplication.timeSinceStartup;
                if (EditorApplication.isPaused) {
                    if (m_paused_frame != Time.renderedFrameCount) {
                        m_paused_frame = Time.renderedFrameCount;
                        update_nodes_in_paused();
                    }
                } else {
                    if (m_updating_nodes.Count != 0) {
                        m_need_repaint = true;
                        for (int i = 0; i < m_updating_nodes.Count; ) {
                            var node = m_updating_nodes[i];
                            node.runtime_fading -= dt * 2;
                            if (node.runtime_fading <= 0) {
                                node._set_node_color(NORMAL_COLOR);
                                remove_updating_node(node);
                                continue;
                            }
                            node._set_node_color(Color.LerpUnclamped(NORMAL_COLOR, node.runtime_color, node.runtime_fading));
                            ++i;
                        }
                    }
                }
                if (m_need_repaint) {
                    m_need_repaint = false;
                    editor.view.window.Repaint();
                }
            }

            private void update_nodes_in_paused() {
                if (m_updating_nodes.Count != 0) {
                    for (int i = 0; i < m_updating_nodes.Count;) {
                        var node = m_updating_nodes[i];
                        if (node.runtime_fading != 1) {
                            node._set_node_color(NORMAL_COLOR);
                            remove_updating_node(node);
                            continue;
                        }
                        node.runtime_fading = 0;
                        ++i;
                    }
                    m_need_repaint = true;
                }
            }

            private double m_last_update_time;
            private int m_paused_frame;
            private bool m_need_repaint;
            private List<BTNodeEditor> m_updating_nodes = new List<BTNodeEditor>();

            private void add_updating_node(BTNodeEditor node) {
                if (node.runtime_updating_index != -1) {
                    return;
                }
                node.runtime_updating_index = m_updating_nodes.Count;
                m_updating_nodes.Add(node);
            }

            private void remove_updating_node(BTNodeEditor node) {
                if (node.runtime_updating_index == -1) {
                    return;
                }
                var last = m_updating_nodes.Count - 1;
                if (node.runtime_updating_index != last) {
                    m_updating_nodes[last].runtime_updating_index = node.runtime_updating_index;
                    m_updating_nodes[node.runtime_updating_index] = m_updating_nodes[last];
                }
                node.runtime_updating_index = -1;
                m_updating_nodes.RemoveAt(last);
            }
        }

        private Runtime m_runtime;
    }
}