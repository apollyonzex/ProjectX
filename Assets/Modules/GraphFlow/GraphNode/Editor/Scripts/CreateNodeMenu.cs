
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GraphNode.Editor {

    public class CreateNodeMenu : PopupWindowContent {
        public CreateNodeMenu(GraphView view, Vector2 local_position, PortView port) {
            m_view = view;
            m_local_position.x = Mathf.Round(local_position.x * (1f/16)) * 16;
            m_local_position.y = Mathf.Round(local_position.y * (1f/16)) * 16;
            m_port = port;
        }

        public override void OnOpen() {
            if (m_view.editor != null) {
                if (m_port == null) {
                    var iter = m_view.editor.enumerate_available_node_types();
                    while (iter.MoveNext()) {
                        var ty = iter.Current;
                        var ctor = ty.GetConstructor(System.Type.EmptyTypes);
                        if (ctor != null) {
                            m_items.Add(new MenuItem(NodeEditor.build_name(ty), ctor));
                        }
                    }
                } else {
                    var iter = m_view.editor.enumerate_available_node_types(m_port.port);
                    while (iter.MoveNext()) {
                        var (ty, p) = iter.Current;
                        var ctor = ty.GetConstructor(System.Type.EmptyTypes);
                        if (ctor != null) {
                            m_items.Add(new MenuItem(NodeEditor.build_name(ty), ctor, p));
                        }
                    }
                }
                m_items.Sort((a, b) => a.name.CompareTo(b.name));
            }
        }

        public override void OnClose() {
            m_view.window.Focus();
        }

        public override Vector2 GetWindowSize() {
            return new Vector2(256, 256);
        }

        public override void OnGUI(Rect rect) {
            if (m_created_node != null) {
                GUILayout.Label($"Create {m_created_node.node_name}", EditorStyles.boldLabel);
                GUILayout.BeginVertical(GUI.skin.box);
                m_created_node.create_wizard_gui(m_view.editor);
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Back")) {
                    m_created_node = null;
                    m_connecting_port = null;
                    return;
                }
                GUILayout.FlexibleSpace();
                GUI.enabled = m_created_node.validate_create_wizard();
                if (GUILayout.Button("Create")) {
                    create_node();
                    return;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                
                return;
            }

            GUILayout.Label("Create Node", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("Filter");
            m_filter = GUILayout.TextField(m_filter);
            if (EditorGUI.EndChangeCheck()) {
                update_items();    
            }
            if (GUI.GetNameOfFocusedControl() != "Filter") {
                GUI.FocusControl("Filter");
            } else {
                var e = Event.current;
                if (e.isKey && e.keyCode == KeyCode.Return) {
                    foreach (var item in m_items) {
                        if (item.visible) {
                            create_node(item);
                            break;
                        }
                    }
                }
            }
            m_scroll_position = GUILayout.BeginScrollView(m_scroll_position);
            var style = GraphResources.styles.menu_item;

            foreach (var item in m_items) {
                if (item.visible) {
                    if (GUILayout.Button(item.name, style)) {
                        create_node(item);
                    }
                }
            }
            
            GUILayout.EndScrollView();
        }

        private void update_items() {
            if (m_filter == string.Empty) {
                foreach (var item in m_items) {
                    item.visible = true;
                }
            } else {
                var s = m_filter.ToLower();
                foreach (var item in m_items) {
                    item.visible = filter(item.name.ToLower(), s);
                }
            }
        }

        static private bool filter(string content, string filter) {
            int last = 0;
            foreach (var c in filter) {
                var idx = content.IndexOf(c, last);
                if (idx < last) {
                    return false;
                }
                last = idx + 1;
            }
            return true;
        }

        private void create_node(MenuItem item) {
            var node = item.ctor.Invoke(null) as Node;
            node.position = m_local_position;

            m_created_node = GraphTypeCache.create_node_editor(node.GetType());
            if (m_created_node == null) {
                Debug.LogError($"GraphNode \'{node.GetType().Name}\': create editor failed");
                editorWindow.Close();
            } else {
                m_created_node.attach(m_view.editor, node);
                m_connecting_port = item.port;
                if (!m_created_node.has_create_wizard) {
                    create_node();
                }
            }
        }

        private void create_node() {

            m_view.window.undo.begin_group();
            if (m_port == null) {
                m_view.add_node(new NodeView(m_view, m_created_node), true);
            } else {
                var node_view = new NodeView(m_view, m_created_node);

                var port = node_view.find_static_port(m_connecting_port);
                if (port != null) {
                    ConnectionView conn;
                    if (m_port.port.io.is_input()) {
                        conn = new ConnectionView((InputPortView)m_port, (OutputPortView)port);
                    } else {
                        conn = new ConnectionView((InputPortView)port, (OutputPortView)m_port);
                    }
                    
                    m_view.add_node(node_view, true);
                    m_view.window.undo.record(new GraphUndo.CreateConnection(conn, null, null));
                    m_view.add_connection_unchecked(conn);
  
                } else {
                    m_view.add_node(node_view, true);
                }
            }

            m_view.window.undo.end_group();

            editorWindow.Close();
        }


        GraphView m_view;
        Vector2 m_local_position;
        PortView m_port;

        NodeEditor m_created_node;
        NodePort m_connecting_port;

        private class MenuItem {
            public string name;
            public System.Reflection.ConstructorInfo ctor;
            public NodePort port;
            public bool visible = true;

            public MenuItem(string name, System.Reflection.ConstructorInfo ctor, NodePort port = null) {
                this.name = name;
                this.ctor = ctor;
                this.port = port;
            }
        }

        List<MenuItem> m_items = new List<MenuItem>();
        Vector2 m_scroll_position;
        string m_filter = string.Empty;
    }

}