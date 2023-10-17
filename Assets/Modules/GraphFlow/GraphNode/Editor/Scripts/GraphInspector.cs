
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {

    public class GraphInspector {
        
        public GraphInspector(GraphView graph) {
            this.graph = graph;
        }

        public static GraphInspector current { get; private set; }

        public Rect rect { get; private set; }

        public float max_width => rect.size.x - 6;

        public void on_gui(Rect rect) {
            this.rect = rect;
            GUILayout.BeginArea(rect);
            on_gui_layout();
            GUILayout.EndArea();
        }

        public void on_gui_layout() {
            current = this;
            GUI.color = Color.white;
            m_scroll_position = EditorGUILayout.BeginScrollView(m_scroll_position);

            var new_page = GUILayout.SelectionGrid(m_page, PAGE_NAMES, PAGE_NAMES.Length);
            if (new_page != m_page) {
                GUIUtility.keyboardControl = 0;
                m_page = new_page;

                switch (m_page) {
                    case 0:
                        if (graph.editor != null) {
                            graph.editor.on_inspector_disable();
                        }
                        if (graph.selected_nodes.Count == 1) {
                            m_last_node = graph.selected_nodes[0].editor;
                            m_last_node.on_inspector_enable();
                        }
                        break;
                    case 1:
                        if (m_last_node != null) {
                            m_last_node.on_inspector_disable();
                            m_last_node = null;
                        }
                        if (graph.editor != null) {
                            graph.editor.on_inspector_enable();
                        }
                        break;

                    
                }
            } else {

            }
            switch (m_page) {
                case 0: {
                    switch (graph.selected_nodes.Count) {
                        case 0: 
                            if (m_last_node != null) {
                                m_last_node.on_inspector_disable();
                                m_last_node = null;
                            }
                            break;
                        case 1: {
                            var node = graph.selected_nodes[0].editor;
                            if (node != m_last_node) {
                                m_last_node?.on_inspector_disable();
                                m_last_node = node;
                                m_last_node.on_inspector_enable();
                            }
                            EditorGUILayout.HelpBox(node.node_name, MessageType.None);
                            node.on_inspector_gui();
                            break;
                        }
                        default:
                            if (m_last_node != null) {
                                m_last_node.on_inspector_disable();
                                m_last_node = null;
                            }
                            EditorGUILayout.HelpBox($"{graph.selected_nodes.Count} Nodes Selected", MessageType.Info);
                            break;
                    }
                    break;
                }
                case 1: {
                    if (graph.editor != null) {
                        EditorGUILayout.HelpBox(graph.editor.graph.GetType().Name, MessageType.None);
                        graph.editor.on_inspector_gui();
                    }
                    break;
                }
            }
            EditorGUILayout.EndScrollView();

            var ev = Event.current;
            if (ev.type == EventType.MouseDown) {
                var rect = GUILayoutUtility.GetLastRect();
                if (rect.Contains(ev.mousePosition)) {
                    GUIUtility.keyboardControl = 0;
                    ev.Use();
                }
            }

            current = null;
        } 

        static readonly string[] PAGE_NAMES = new string[] { "Node", "Graph" };

        public GraphView graph { get; }

        Vector2 m_scroll_position;
        int m_page = 0;

        NodeEditor m_last_node = null;
    }

}