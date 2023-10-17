
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {

    public class GraphEditorWindow : EditorWindow {

        protected virtual float inspector_min_width => 256;
        protected virtual float view_min_width => 256;

        public const float INSPECTOR_SEPARATOR = 4;

        protected virtual void OnEnable() {
            m_stack = new List<(GraphView view, GraphInspector inspector)>(1);
            var root_view = new GraphView(this);
            m_stack.Add((root_view, new GraphInspector(root_view)));
            
            inspector_width = inspector_width;
            minSize = new Vector2(view_min_width + inspector_min_width + INSPECTOR_SEPARATOR, 64);

            if (m_asset != null) {
                var graph = m_asset.load_graph();
                root_view.open(graph);
            }

            titleContent = new GUIContent("Graph");
        }

        protected virtual void OnDisable() {
            if (m_asset != null) {
                if (is_dirty) {
                    if (EditorUtility.DisplayDialog("Graph Have Been Modified", "Do you want to save?", "Save", "Don't Save")) {
                        save();
                    }
                }
                close_views();
            }
            m_stack = null;
        }

        protected bool save() {
            if (save_views() && m_asset.save_graph(m_stack[0].view.editor.graph)) {
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }

        protected bool save_as() {
            if (m_asset == null) {
                return false;
            }
            var path = EditorUtility.SaveFilePanelInProject("SaveAs", m_asset.GetType().Name, "asset", string.Empty);
            if (string.IsNullOrEmpty(path)) {
                return false;
            }
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (asset != null) {
                if (asset == m_asset) {
                    return save();
                }
                if (!EditorUtility.DisplayDialog("The asset already exists", "Do you overwrite?", "Overwrite", "Cancel")) {
                    return false;
                }
            }
            m_asset = CreateInstance(m_asset.GetType()) as GraphAsset;
            if (!save_views() || !m_asset.save_graph(m_stack[0].view.editor.graph)) {
                return false;
            }
            AssetDatabase.CreateAsset(m_asset, path);
            AssetDatabase.SaveAssets();
            return true;
        }

        protected virtual void OnGUI() {
            var top = get_stack_top();
            top.view.on_hook_event();

            var view_rect = new Rect(Vector2.zero, new Vector2(position.width - inspector_width - INSPECTOR_SEPARATOR, position.height));
            top.view.on_gui(view_rect);

            on_inspector_separator_gui(new Rect(position.width - inspector_width - INSPECTOR_SEPARATOR, 0, INSPECTOR_SEPARATOR, position.height));

            top.inspector.on_gui(new Rect(position.width - inspector_width, 0, inspector_width, position.height));

            top.view.on_divs_gui();

            if (m_asset != null) {
                titleContent = new GUIContent(is_dirty ? $"Graph - {m_asset.name} [UNSAVED]" : $"Graph - {m_asset.name}");
            }

            var ev = Event.current;
            switch (ev.type) {
                case EventType.KeyDown:
                    if (ev.keyCode == KeyCode.S && !ev.alt) {
                        if (GraphView.is_mac() ? ev.command : ev.control) {
                            if (ev.shift) {
                                save_as();
                            } else {
                                save();
                            }
                            ev.Use();
                        }
                    }
                    break;
            }
        }

        protected List<(GraphView view, GraphInspector inspector)> m_stack;
        public GraphUndo undo => m_undo;
        private GraphUndo m_undo = new GraphUndo();
        private int m_saved_undo_dirty_count;

        public bool is_dirty => m_saved_undo_dirty_count != m_undo.dirty_count;

        protected (GraphView view, GraphInspector inspector) get_stack_top() {
            return m_stack[m_stack.Count - 1];
        }

        internal (GraphView view, GraphInspector inspector) pop_stack() {
            var last = m_stack.Count - 1;
            var ret = m_stack[last];
            m_stack.RemoveAt(last);
            return ret;
        }

        internal void push_stack(GraphView view, GraphInspector inspector) {
            m_stack.Add((view, inspector));
        }

        protected void close_views() {
            while (m_stack.Count > 1) {
                pop_stack().view.close();
            }
            get_stack_top().view.close();
            m_undo.clear();
            m_saved_undo_dirty_count = m_undo.dirty_count;
            m_cached_graphs.Clear();
        }

        protected bool save_views() {
            for (int i = m_stack.Count - 1; i >= 0; --i) {
                if (!m_stack[i].view.save()) {
                    return false;
                }
            }
            m_saved_undo_dirty_count = m_undo.dirty_count;
            return true;
        }

        public float inspector_width {
            get => m_inspector_width;
            private set {
                m_inspector_width = Mathf.Max(value, inspector_min_width);
            }
        }
        private float m_inspector_width = 0;

        protected GraphAsset m_asset;
        public GraphAsset asset => m_asset;


        private void on_inspector_separator_gui(Rect rect) {
            var control_id = GUIUtility.GetControlID(FocusType.Passive, rect);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight, control_id);
            var e = Event.current;
            switch (e.type) {
                case EventType.MouseDown: 
                    if (rect.Contains(e.mousePosition) && GUIUtility.hotControl == 0) {
                        GUIUtility.hotControl = control_id;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == control_id) {
                        inspector_width -= e.mousePosition.x - rect.center.x;
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == control_id) {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }
        }

        internal void on_view_navigation_gui(Rect rect) {
            if (m_stack.Count > 1) {
                GUILayout.BeginArea(rect);
                GUILayout.BeginHorizontal(GUI.skin.box);
                var last = m_stack.Count - 1;
                for (int i = 0; i < last; ++i) {
                    var (view, _) = m_stack[i];
                    if (GUILayout.Button(view.name, GUILayout.ExpandWidth(false))) {
                        m_undo.begin_group();
                        var target = i + 1;
                        while (m_stack.Count > target) {
                            var graph = pop_stack();
                            m_undo.record(new GraphUndo.PopGraph(graph.view, graph.inspector));
                        }
                        m_undo.end_group();
                        break;
                    }
                    GUILayout.Label("/", GUILayout.ExpandWidth(false));
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        [UnityEditor.Callbacks.OnOpenAsset(999)]
        private static bool on_open(int instance_id, int line) {
            if (EditorUtility.InstanceIDToObject(instance_id) is GraphAsset asset) {
                open(asset);
                return true;
            }
            return false;
        }

        [System.NonSerialized]

        private bool m_runtime;
        public bool runtime {
            get => m_runtime;
            private set => m_runtime = value;
        }

        public static void open(GraphAsset asset) {
            if (asset == null) {
                return;
            }
            var wnd = GetWindow<GraphEditorWindow>();
            if (wnd.m_asset != null && wnd.is_dirty) {
                var opt = EditorUtility.DisplayDialogComplex("Graph Have Been Modified", "Do you want to save?", "Save", "Cancel", "Don't Save");
                if (opt == 1) {
                    return;
                }
                if (opt == 0) {
                    wnd.save();
                }
            }
            wnd.close_views();
            wnd.m_asset = asset;
            wnd.runtime = false;
            var graph = asset.load_graph();
            wnd.m_stack[0].view.open(graph);
        }

        public static void open_runtime(GraphAsset asset) {
            if (asset == null) {
                return;
            }
            var wnd = GetWindow<GraphEditorWindow>();
            if (wnd.m_asset != null && wnd.is_dirty) {
                var opt = EditorUtility.DisplayDialogComplex("Graph Have Been Modified", "Do you want to save?", "Save", "Cancel", "Don't Save");
                if (opt == 1) {
                    return;
                }
                if (opt == 0) {
                    wnd.save();
                }
            }
            wnd.close_views();
            wnd.m_asset = asset;
            wnd.runtime = true;
            var graph = asset.get_graph<Graph>();
            wnd.m_stack[0].view.open(graph);
        }

        public void push_graph(Graph graph) {
            if (get_graph_view(graph, out var view)) {
                m_stack.Add(view);
                m_undo.record(new GraphUndo.PushGraph(view.view, view.inspector));
            }
        }

        private bool get_graph_view(Graph graph, out (GraphView view, GraphInspector inspector) view) {
            if (!m_cached_graphs.TryGetValue(graph, out view)) {
                var _view = new GraphView(this);
                if (!_view.open(graph)) {
                    return false;
                }
                view.view = _view;
                view.inspector = new GraphInspector(_view);
                m_cached_graphs.Add(graph, view);
            }
            return true;
        }

        private Dictionary<Graph, (GraphView view, GraphInspector inspector)> m_cached_graphs = new Dictionary<Graph, (GraphView view, GraphInspector inspector)>();
    }

}