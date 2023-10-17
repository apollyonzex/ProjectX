
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System;

namespace InvokeFlow.Editor {

    [GraphEditor(typeof(InvokeGraph))]
    public class InvokeGraphEditor : GenericGraphEditor {

        public override void attach(Graph graph, GraphView view) {
            base.attach(graph, view);
            init_property_editors();
            build_stack_frame(false);
        }

        public override void on_open() {
            base.on_open();
            proc_entries.build_names();
            struct_defs.build_names();
            collections.build_names();
            elements.build_names();
            if (view.runtime) {
                m_runtime = new Runtime();
                m_runtime.init(this);
            }
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

        public override void on_graph_saving() {
            if (m_graph is InvokeGraph g) {
                var elements = this.elements.items;
                var collections = this.collections.items;
                g.obj_count = elements.Count;
                for (int i = 0; i < elements.Count; ++i) {
                    elements[i].node.stack_pos = g.obj_count - i;
                }
                g.obj_count += collections.Count;
                for (int i = 0; i < collections.Count; ++i) {
                    collections[i].node.stack_pos = g.obj_count - i;
                }
            }
        }

        public virtual void build_stack_frame(bool rise_event) {
            if (graph is InvokeGraph g) {
                g.stack_frame = new int[m_variables_editor.available_variables.Count];
                for (int i = 0; i < g.stack_frame.Length; ++i) {
                    g.stack_frame[i] = m_variables_editor.available_variables[i].value_in_stack;
                }
            }

            if (rise_event) {
                foreach (var node in view.nodes) {
                    if (node.editor is InvokeNodeEditor ne) {
                        ne.notify_stack_changed();
                    }
                }
            }
        }

        public virtual List<Variable> stack_frame => m_variables_editor.available_variables;

        public readonly NodeEditorList<ProcEntryNodeEditor> proc_entries = new NodeEditorList<ProcEntryNodeEditor>();

        public readonly NodeEditorList<StructDefNodeEditor> struct_defs = new NodeEditorList<StructDefNodeEditor>();
        public readonly NodeEditorList<CollectionNodeBaseEditor> collections = new NodeEditorList<CollectionNodeBaseEditor>();
        public readonly NodeEditorList<ElementNodeEditor> elements = new NodeEditorList<ElementNodeEditor>();

        protected virtual void init_property_editors() {
            if (try_get_property("variables", out var pe)) {
                m_variables_editor = pe as VariablesEditor;
            }
        }

        protected VariablesEditor m_variables_editor;


        Runtime m_runtime;

        class Runtime : GraphView.IDiv {
            public InvokeGraphEditor editor;
            public Dictionary<InvokeNode, InvokeNodeEditor> nodes = new Dictionary<InvokeNode, InvokeNodeEditor>();

            public static readonly Color NORMAL_COLOR = new Color32(90, 97, 105, 255);
            public static readonly Color PENDING_COLOR = new Color32(144, 128, 0, 255);
            public static readonly Color SUCCESS_COLOR = new Color32(0, 128, 0, 255);

            public void init(InvokeGraphEditor editor) {
                this.editor = editor;
                Recorder.graph_event += on_graph_event;
                foreach (var node_view in editor.view.nodes) {
                    if (node_view.editor is InvokeNodeEditor ne) {
                        nodes.Add(ne.node as InvokeNode, ne);
                    }
                }
                EditorApplication.playModeStateChanged += on_play_mode_state_changed;
                EditorApplication.pauseStateChanged += on_pause_state_changed;
                editor.view.add_div(this);
            }



            public void fini() {
                Recorder.graph_event -= on_graph_event;
                EditorApplication.playModeStateChanged -= on_play_mode_state_changed;
                EditorApplication.pauseStateChanged -= on_pause_state_changed;
                editor.view.remove_div(this);
            }

            public void on_node_added(NodeView node_view) {
                if (node_view.editor is InvokeNodeEditor ne) {
                    nodes.Add(ne.node as InvokeNode, ne);
                }
            }

            public void on_node_removed(NodeView node_view) {
                if (node_view.editor is InvokeNodeEditor ne) {
                    nodes.Remove(ne.node as InvokeNode);
                    ne._set_node_color(NORMAL_COLOR);
                }
            }

            void on_graph_event(IContext context, InvokeGraph graph, bool status) {
                if (!ContextRecorder.is_current(context)) {
                    return;
                }
                if (status) {
                    m_graph_stack.Push(graph);
                    ContextRecorder.notify_recorded();
                    if (graph == editor.graph) {
                        Recorder.node_event += on_node_event;
                        var frame_count = Time.renderedFrameCount;
                        if (frame_count != m_last_frame_count) {
                            m_last_frame_count = frame_count;
                            m_events.Clear();
                            m_event_index = -1;
                            editor.view.window.Repaint();
                        }
                    }
                } else {
                    m_graph_stack.Pop();
                    if (graph == editor.graph) {
                        Recorder.node_event -= on_node_event;
                        if (m_pop_count != 0) {
                            m_events.Add(new NodeEvent { pop_count = m_pop_count });
                            m_pop_count = 0;
                        }
                    }
                }
            }

            void on_node_event(IContext context, InvokeNode node, bool status) {
                if (!ContextRecorder.is_current(context) || m_graph_stack.Peek() != editor.graph) {
                    return;
                }
                if (status) {
                    nodes.TryGetValue(node, out var node_editor);
                    m_events.Add(new NodeEvent { pop_count = m_pop_count, push = true, node = node_editor, data = node_editor?.runtime_build_data(context) });
                    m_pop_count = 0;
                    if (node_editor != null && node_editor.break_when_invoke) {
                        Debug.Break();
                    }
                } else {
                    ++m_pop_count;
                }
            }

            void on_play_mode_state_changed(PlayModeStateChange state) {
                if (state == PlayModeStateChange.EnteredPlayMode) {
                    m_last_frame_count = -1;
                }
            }

            void on_pause_state_changed(PauseState state) {
                if (state == PauseState.Paused) {
                    
                } else {

                }
                editor.view.window.Repaint();
            }

            struct NodeEvent {
                public int pop_count;
                public bool push;
                public InvokeNodeEditor node;
                public object data;
            }

            List<NodeEvent> m_events = new List<NodeEvent>();
            int m_pop_count = 0;
            int m_last_frame_count;
            List<InvokeNodeEditor> m_stack = new List<InvokeNodeEditor>();
            int m_event_index = -1;
            Stack<InvokeGraph> m_graph_stack = new Stack<InvokeGraph>();

            Rect m_rect = new Rect(16, 16, 0, 64);

            bool GraphView.IDiv.contains(Vector2 point) {
                return is_visible() && m_rect.Contains(point);
            }

            bool is_visible() => m_events.Count != 0 && (!EditorApplication.isPlaying || EditorApplication.isPaused);

            void GraphView.IDiv.on_gui() {
                if (Event.current.type == EventType.Layout) {
                    var width = editor.view.window.position.width;
                    width -= editor.view.window.inspector_width + GraphEditorWindow.INSPECTOR_SEPARATOR + 32;
                    m_rect.width = width;
                }

                if (m_event_index == -1 && m_stack.Count != 0) {
                    clear_stack();
                }

                if (!is_visible()) {
                    return;
                }

                GUILayout.BeginArea(m_rect);
                GUILayout.BeginVertical("Events", GUI.skin.window);

                GUILayout.BeginHorizontal();
                var new_index = m_event_index;
                var last = m_events.Count - 1;
                if (GUILayout.Button("|<", GUILayout.Width(32))) {
                    new_index = 0;
                }
                if (GUILayout.Button("<", GUILayout.Width(32))) {
                    new_index = Mathf.Max(0, new_index - 1);
                }
                if (GUILayout.Button(">", GUILayout.Width(32))) {
                    new_index = Mathf.Min(new_index + 1, last);
                }
                new_index = EditorGUILayout.IntSlider(new_index, 0, last);
                GUILayout.Label($"/ {last}", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                if (new_index > m_event_index) {
                    advance_stack(new_index);
                } else if (new_index < m_event_index) {
                    clear_stack();
                    advance_stack(new_index);
                }


                GUILayout.EndVertical();
                if (Event.current.type == EventType.Repaint) {
                    m_rect.height = GUILayoutUtility.GetLastRect().height;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndArea();
            }

            void clear_stack() {
                foreach (var node in m_stack) {
                    if (node != null) {
                        node._set_node_color(NORMAL_COLOR);
                        node.runtime_leave();
                    }
                }
                m_stack.Clear();
                m_event_index = -1;
            }

            void advance_stack(int new_index) {
                for (int i = m_event_index + 1; i <= new_index; ++i) {
                    var ev = m_events[i];
                    while (ev.pop_count > 0) {
                        var last = m_stack.Count - 1;
                        var ne = m_stack[last];
                        if (ne != null) {
                            ne._set_node_color(NORMAL_COLOR);
                            ne.runtime_leave();
                        }
 
                        m_stack.RemoveAt(last);
                        --ev.pop_count;
                    }
                    if (ev.push) {
                        if (m_stack.Count > 0) {
                            m_stack[m_stack.Count - 1]?._set_node_color(PENDING_COLOR);
                        }
                        m_stack.Add(ev.node);
                        if (ev.node != null) {
                            ev.node._set_node_color(SUCCESS_COLOR);
                            ev.node.runtime_enter(ev.data);
                        }
      
                    }
                }
                m_event_index = new_index;
            }
        }
    }

}