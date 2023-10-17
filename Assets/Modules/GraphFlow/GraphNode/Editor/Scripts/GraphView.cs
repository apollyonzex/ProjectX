
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GraphNode.Editor {

    public class GraphView {



        public GraphView(GraphEditorWindow window) {
            m_window = window;
            m_current_state = m_state_normal;
            undo = window?.undo ?? new GraphUndo();
        }

        public void on_hook_event() {
            m_current_state.on_hook_event(this);
        }

        public void on_gui(Rect rect) {
            m_control_id = GUIUtility.GetControlID(FocusType.Keyboard);           
            switch (Event.current.type) {
                case EventType.Layout:
                    on_layout(rect);
                    m_window.on_view_navigation_gui(rect);
                    break;

                case EventType.Repaint:
                    on_repaint();
                    GUI.color = Color.white;
                    m_window.on_view_navigation_gui(rect);
                    break;

                default:
                    on_events();
                    break;
            }
        }

        private void draw_grids() {
            var size = m_rect.size;
            var center = size * 0.5f;
            
            var grid_tex = GraphResources.grid_texture;
            var cross_tex = GraphResources.cross_texture;

            var off_x = -(center.x + m_local_offset.x) * m_zoom / grid_tex.width;
            var off_y = -(center.y - m_local_offset.y) * m_zoom / grid_tex.height;

            var tile_off = new Vector2(off_x, off_y);

            var tile_amount_x = size.x * m_zoom / grid_tex.width;
            var tile_amount_y = size.y * m_zoom / grid_tex.height;

            var tile_amount = new Vector2(tile_amount_x, tile_amount_y);

            GUI.DrawTextureWithTexCoords(m_rect, grid_tex, new Rect(tile_off, tile_amount));
            GUI.DrawTextureWithTexCoords(m_rect, cross_tex, new Rect(tile_off + new Vector2(0.5f, 0.5f), tile_amount));
        }


        public GraphEditorWindow window => m_window;
        private GraphEditorWindow m_window;
        private Rect m_window_in_screen;
        private Rect m_rect;
        private float m_zoom = 1;
        private Vector2 m_local_offset;
        private Rect m_local_rect;
        private int m_control_id;
        

        public Vector2 local_offset {
            get => m_local_offset;
            set {
                if (m_local_offset != value) {
                    m_local_offset = value;
                    m_need_recull = true;
                    m_window.Repaint();
                }
            }
        }

        public float zoom {
            get => m_zoom;
            set {
                value = Mathf.Clamp(value, 1, 5);
                if (m_zoom != value) {
                    m_zoom = value;
                    m_need_recull = true;
                    m_window.Repaint();
                }
            }
        }

        public GraphUndo undo { get; }

        public bool runtime => window != null ? window.runtime : false;

        internal Rect local_rect => m_local_rect;

        private void on_layout(Rect rect) {
            if (m_rect != rect) {
                m_rect = rect;
                m_need_recull = true;
            }

            editor?.on_layout();

            var r = GUIUtility.ScreenToGUIRect(m_window.position);
            m_window_in_screen = new Rect(-r.min, r.size); 

            if (m_next_layout_events != null) {
                m_next_layout_events.Invoke();
                m_next_layout_events = null;
            }

            begin_clip_and_scale();
            if (m_need_recull) {
                m_need_recull = false;
                m_local_rect.size = m_rect.size * m_zoom;
                foreach (var node in m_nodes) {
                    node.on_layout_for_recull();
                }
            }
            foreach (var node in m_nodes) {
                if (!node.culled) {
                    node.on_gui();
                }
            }
            end_clip_and_scale();
        }

        private void on_repaint() {

            draw_grids();
            begin_clip_and_scale();
            
            foreach (var conn in m_connections) {
                conn.on_repaint(this);
            }

            m_current_state.on_repaint_before_nodes(this);
            //int visible_count = 0;
            foreach (var node in m_nodes) {
                if (!node.culled) {
                    node.on_gui();
                    //++visible_count;
                }
            }
            m_current_state.on_repaint_after_nodes(this);
            end_clip_and_scale();
        }

        private void on_events() {

            begin_clip_and_scale();
            foreach (var node in m_nodes) {
                if (!node.culled) {
                    node.on_gui();
                }
            }
            end_clip_and_scale();

            
            m_window.on_view_navigation_gui(m_rect);

            var ev = Event.current;
            switch (ev.type) {
                case EventType.ScrollWheel:
                    if (m_rect.Contains(ev.mousePosition) && pick_div(ev.mousePosition) == null) {
                        var pt = gui_to_local(ev.mousePosition);
                        if (ev.delta.y > 0) {
                            zoom += 0.1f * zoom;
                        } else {
                            zoom -= 0.1f * zoom;
                        }
                        local_offset += (gui_to_local(ev.mousePosition) - pt) / zoom;
                        ev.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (!ev.alt) {
                        switch (ev.keyCode) {
                            case KeyCode.Z: {
                                if (is_mac() ? ev.command : ev.control) {
                                    if (m_current_state.not_busy) {
                                        if (ev.shift) {
                                            if (undo.redo()) {
                                                m_window.ShowNotification(new GUIContent("Redo"));
                                                var e = EditorGUIUtility.CommandEvent("UndoRedoPerformed");
                                                e.type = EventType.ValidateCommand;
                                                m_window.SendEvent(e);
                                            }
                                        } else {
                                            if (undo.undo()) {
                                                m_window.ShowNotification(new GUIContent("Undo"));
                                                var e = EditorGUIUtility.CommandEvent("UndoRedoPerformed");
                                                e.type = EventType.ValidateCommand;
                                                m_window.SendEvent(e);
                                            }
                                        }
                                    }
                                    ev.Use();
                                }
                                break;
                            }
                            case KeyCode.Y: {
                                if (!ev.shift && (is_mac() ? ev.command : ev.control)) {
                                    if (m_current_state.not_busy) {
                                        if (undo.redo()) {
                                            m_window.ShowNotification(new GUIContent("Redo"));
                                            var e = EditorGUIUtility.CommandEvent("UndoRedoPerformed");
                                            e.type = EventType.ValidateCommand;
                                            m_window.SendEvent(e);
                                        }
                                    }
                                    ev.Use();
                                }
                                break;
                            }
                        }
                    }
                    break;
                case EventType.ValidateCommand:
                    switch (ev.commandName) {
                        case "SoftDelete":
                        case "Delete":
                        case "Duplicate":
                        case "SelectAll":
                        case "FrameSelected":
                        case "Copy":
                        case "Paste":
                            ev.Use();
                            break;
                    }
                    break;
                case EventType.ExecuteCommand:
                    switch (ev.commandName) {
                        case "PopMenu":
                            if (m_current_state.not_busy) {
                                if (m_menu != null) {
                                    m_menu.DropDown(new Rect(m_menu_position, Vector2.zero));
                                } else if (m_create_node_menu != null) {
                                    var content = m_create_node_menu;
                                    m_create_node_menu = null;
                                    PopupWindow.Show(new Rect(m_menu_position, Vector2.zero), content);
                                }
                            }
                            m_menu = null;
                            m_create_node_menu = null;
                            ev.Use();
                            break;
                        case "SoftDelete":
                        case "Delete":
                            delete_selected_nodes();
                            ev.Use();
                            break;
                        case "Duplicate":
                            duplicate_selected_nodes();
                            ev.Use();
                            break;
                        case "Copy":
                            copy_selected_nodes();
                            ev.Use();
                            break;
                        case "Paste":
                            paste();
                            ev.Use();
                            break;
                        case "SelectAll":
                            select_all_nodes();
                            ev.Use();
                            break;
                        case "FrameSelected":
                            frame_selected_nodes();
                            ev.Use();
                            break;
                    }
                    break;
                default:
                    m_current_state.on_events(this);
                    break;
            }
        }

        internal Vector2 gui_to_local(Vector2 point) {
            return (point - m_rect.center - m_local_offset) * m_zoom;
        }

        internal Vector2 local_to_gui(Vector2 point) {
            return point / m_zoom + m_rect.center + m_local_offset;
        }

        internal Vector2 local_to_clipped_gui(Vector2 point) {
            return point + (m_rect.size * 0.5f + m_local_offset) * m_zoom;
        }

        internal Vector2 gui_to_clipped_gui(Vector2 point) {
            return (point - m_rect.position) * m_zoom;
        }

        internal Vector2 clipped_gui_to_gui(Vector2 point) {
            return point / m_zoom + m_rect.position;
        }


        private List<NodeView> m_nodes = new List<NodeView>();

        public IReadOnlyList<NodeView> nodes => m_nodes;

        public IReadOnlyList<NodeView> selected_nodes => m_selected_nodes;
        private List<NodeView> m_selected_nodes = new List<NodeView>();

        private void select_node(NodeView node) {
            if (!node.selected) {
                node.selected = true;
                m_selected_nodes.Add(node);
            }
        }

        private void unselect_node(NodeView node) {
            var idx = m_selected_nodes.IndexOf(node);
            if (idx != -1) {
                var last = m_selected_nodes.Count - 1;
                if (idx != last) {
                    m_selected_nodes[idx] = m_selected_nodes[last];
                }
                m_selected_nodes.RemoveAt(last);
                node.selected = false;
            }
        }

        private void select_all_nodes() {
            m_selected_nodes.Clear();
            foreach (var node in m_nodes) {
                m_selected_nodes.Add(node);
                node.selected = true;
            }
        }

        private void unselect_all_nodes() {
            foreach (var node in m_selected_nodes) {
                node.selected = false;
            }
            m_selected_nodes.Clear();
        }

        private void move_to_top(NodeView node) {
            m_next_layout_events += () => {
                var idx = m_nodes.IndexOf(node);
                if (idx != -1) {
                    var last = m_nodes.Count - 1;
                    if (idx != last) {
                        m_nodes[idx] = m_nodes[last];
                        m_nodes[last] = node;
                    }
                }
            };
        }

        private void frame_selected_nodes() {
            if (m_selected_nodes.Count == 0) {
                local_offset = Vector2.zero;
                zoom = 1;
            } else {
                var node = m_selected_nodes[0];
                var min = node.position;
                var max = min + node.size;
                for (int i = 1; i < m_selected_nodes.Count; ++i) {
                    node = m_selected_nodes[i];
                    min = Vector2.Min(min, node.position);
                    max = Vector2.Max(max, node.position + node.size);
                }
                var a = m_rect.width / m_rect.height;
                var size = max - min;
                if (a * size.y <= size.x) {
                    zoom = size.x / m_rect.width;
                } else {
                    zoom = size.y / m_rect.height;
                }
                local_offset = (min + max) * (-0.5f / zoom);
            }
        }

        private void copy_selected_nodes() {
            if (window == null || window.asset == null) {
                return;
            }
            var nodes = new List<NodeView>(m_selected_nodes.Count);
            int count = 0;
            foreach (var node in m_selected_nodes) {
                if (node.editor.unique || node.editor.has_create_wizard) {
                    nodes.Add(null);
                } else {
                    ++count;
                    nodes.Add(node);
                }
            }
            if (count == 0) {
                return;
            }
            if (s_copied == null) {
                s_copied = new GraphView(null);
            }
            s_copied.open(window.asset.new_graph());
            for (int i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                if (node == null) {
                    continue;
                }
                var node_editor = node.editor.clone(s_copied.editor);
                node_editor.node.position = node.editor.node.position + m_local_offset + new Vector2(16, 16);
                nodes[i] = node = new NodeView(s_copied, node_editor);
                s_copied.add_node_without_undo(node, true);
                s_copied.select_node(node);
            }
            for (int i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                if (node == null) {
                    continue;
                }
                var original = m_selected_nodes[i];
                var iter = original.enumerate_ports();
                while (iter.MoveNext()) {
                    var port = iter.Current;

                    foreach (var conn in port.connections) {
                        PortView peer = conn.input == port ? (PortView)conn.output : conn.input;
                        if (!peer.node.selected) {
                            continue;
                        }
                        var idx = m_selected_nodes.IndexOf(peer.node);
                        if (idx < i) {
                            continue;
                        }
                        var cloned = nodes[idx];
                        if (cloned == null) {
                            continue;
                        }
                        var cloned_port1 = node.find_port(original.editor, port.port);
                        var cloned_port2 = cloned.find_port(peer.node.editor, peer.port);
                        ConnectionView new_conn;
                        if (port.port.io.is_input()) {
                            new_conn = new ConnectionView((InputPortView)cloned_port1, (OutputPortView)cloned_port2);
                        } else {
                            new_conn = new ConnectionView((InputPortView)cloned_port2, (OutputPortView)cloned_port1);
                        }
                        s_copied.add_connection_unchecked(new_conn);
                    }
                }
            }
            foreach (var node in nodes) {
                node?.editor.on_duplicate_done(m_selected_nodes, nodes);
            }

            window?.ShowNotification(new GUIContent("Copy"));
            
        }

        private void paste() {
            if (s_copied == null || !s_copied.editor.graph.GetType().IsAssignableFrom(editor.graph.GetType())) {
                window?.ShowNotification(new GUIContent("Paste Failed"));
                return;
            }
            unselect_all_nodes();
            var nodes = new List<NodeView>(s_copied.m_selected_nodes.Count);
            undo.begin_group();
            foreach (var node in s_copied.m_selected_nodes) {
                var node_editor = node.editor.clone(editor);
                var pos = node.editor.node.position - m_local_offset;
                pos.x = Mathf.Round(pos.x / 16) * 16;
                pos.y = Mathf.Round(pos.y / 16) * 16;
                node_editor.node.position = pos;
                var cloned = new NodeView(this, node_editor);
                nodes.Add(cloned);
                add_node(cloned, true);
                select_node(cloned);
            }
            for (int i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                var original = s_copied.m_selected_nodes[i];
                var iter = original.enumerate_ports();
                while (iter.MoveNext()) {
                    var port = iter.Current;

                    foreach (var conn in port.connections) {
                        PortView peer = conn.input == port ? (PortView)conn.output : conn.input;
                        var idx = s_copied.m_selected_nodes.IndexOf(peer.node);
                        if (idx < i) {
                            continue;
                        }
                        var cloned = nodes[idx];
                        var cloned_port1 = node.find_port(original.editor, port.port);
                        var cloned_port2 = cloned.find_port(peer.node.editor, peer.port);
                        ConnectionView new_conn;
                        if (port.port.io.is_input()) {
                            new_conn = new ConnectionView((InputPortView)cloned_port1, (OutputPortView)cloned_port2);
                        } else {
                            new_conn = new ConnectionView((InputPortView)cloned_port2, (OutputPortView)cloned_port1);
                        }
                        undo.record(new GraphUndo.CreateConnection(new_conn, null, null));
                        add_connection_unchecked(new_conn);
                    }
                }
            }
            undo.end_group();
            foreach (var node in nodes) {
                node?.editor.on_duplicate_done(s_copied.m_selected_nodes, nodes);
            }
            window?.ShowNotification(new GUIContent("Paste"));
        }

        private static GraphView s_copied;

        private void begin_clip_and_scale() {
            GUI.EndClip();

            GUI.BeginClip(new Rect((m_window_in_screen.position + m_rect.position) * m_zoom, m_rect.size * m_zoom));

            var s = 1 / m_zoom;
            GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1));
        }

        private void end_clip_and_scale() {
            GUI.EndClip();
            GUI.BeginClip(m_window_in_screen);
            GUI.matrix = Matrix4x4.identity;
        }

        private bool m_need_recull = false;

        private System.Action m_next_layout_events;

        public void add_node(NodeView node, bool rise_event) {
            var cmd = new GraphUndo.AddNode(node);
            undo.record(cmd);
            add_node_without_undo(node, rise_event);
        }

        public void add_node_without_undo(NodeView node, bool rise_event) {
            m_nodes.Add(node);
            node.on_layout_for_recull();
            if (rise_event) {
                editor.on_add_node(node);
            }
        }

        internal void remove_node_unchecked(NodeView node) {
            unselect_node(node);
            m_nodes.Remove(node);
            editor.on_remove_node(node);
        }

        public void add_connection_unchecked(ConnectionView conn, bool do_connect = true) {
            if (m_connections.Add(conn)) {
                conn.connect(do_connect);
            }
        }

        public void remove_connection_unchecked(ConnectionView conn) {
            if (m_connections.Remove(conn)) {
                conn.disconnect();
            }
        }

        private (NodeView node, PortView port) pick_node_in_gui(Vector2 point) {
            return pick_node_in_clipped_gui(gui_to_clipped_gui(point));
        }

        private (NodeView node, PortView port) pick_node_in_clipped_gui(Vector2 point) {
            for (int i = m_nodes.Count - 1; i >= 0; --i) {
                var node = m_nodes[i];
                if (node.culled) {
                    continue;
                }
                if (node.contains(point)) {
                    return (node, node.pick_port(point));
                }
            }
            return (null, null);
        }

        private abstract class State {
            public abstract void on_events(GraphView view);
            public virtual void on_repaint_after_nodes(GraphView view) {}
            public virtual void on_repaint_before_nodes(GraphView view) {}
            public virtual void on_hook_event(GraphView view) {}
            public virtual bool not_busy => false;
        }

        private State m_current_state;

        private class NormalState : State {
            public override bool not_busy => m_button == -1;
            public override void on_events(GraphView view) {
                var ev = Event.current;
                switch (ev.type) {
                    case EventType.MouseDown:
                        if (view.m_rect.Contains(ev.mousePosition) && view.pick_div(ev.mousePosition) == null && GUIUtility.hotControl == 0) {
                            on_button_down(view, ev);
                            ev.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == view.m_control_id) {
                            if (ev.button == m_button) {
                                if (m_button == 0) {
                                    m_button = -1;
                                    if (m_hovered_node != null) {
                                        if (m_hovered_port != null) {
                                            if (!m_hovered_port.connected) {
                                                view.m_state_connecting_port.start(view, m_hovered_port, null, ev.mousePosition, null, null);
                                            } else if (m_hovered_port is InputPortView input_view) {
                                                if (input_view.port.can_mulit_connect) {
                                                    var disconnect = ev.modifiers.HasFlag(is_mac() ? EventModifiers.Command : EventModifiers.Control);
                                                    if (disconnect) {
                                                        var candidates = input_view.get_connection_array();
                                                        var conn = candidates[0];
                                                        view.m_state_connecting_port.start(view, conn.output, conn, ev.mousePosition, candidates, m_hovered_port);
                                                    } else {
                                                        view.m_state_connecting_port.start(view, m_hovered_port, null, ev.mousePosition, null, null);
                                                    }
                                                } else {
                                                    var conn = input_view.get_first_connection();
                                                    view.m_state_connecting_port.start(view, conn.output, conn, ev.mousePosition, null, null);
                                                }
                                            } else if (m_hovered_port is OutputPortView output_view) {
                                                if (output_view.port.can_mulit_connect) {
                                                    var disconnect = ev.modifiers.HasFlag(is_mac() ? EventModifiers.Command : EventModifiers.Control);
                                                    if (disconnect) {
                                                        var candidates = output_view.get_connection_array();
                                                        var conn = candidates[0];
                                                        view.m_state_connecting_port.start(view, conn.input, conn, ev.mousePosition, candidates, m_hovered_port);
                                                    } else {
                                                        view.m_state_connecting_port.start(view, m_hovered_port, null, ev.mousePosition, null, null);
                                                    }
                                                } else {
                                                    var conn = output_view.get_first_connection();
                                                    view.m_state_connecting_port.start(view, conn.input, conn, ev.mousePosition, null, null);
                                                }
                                            }
                                        } else {
                                            if (!m_hovered_node.selected) {
                                                view.select_node(m_hovered_node);
                                                view.move_to_top(m_hovered_node);
                                            }
                                            view.m_state_moving_selected_nodes.start_moving(view, ev);
                                        }
                                    } else {
                                        view.m_state_selection_box.start_selection_box(view, ev);
                                    }
                                } else if (m_button == 1 || m_button == 2) {
                                    m_button = -1;
                                    view.m_state_panning.start_panning(view, ev);
                                }
                            }
                            ev.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == view.m_control_id) {
                            if (ev.button == m_button) {
                                if (m_button == 0) {
                                    if (m_multi_selection) {
                                        if (m_hovered_node != null) {
                                            if (m_hovered_node.selected) {
                                                view.unselect_node(m_hovered_node);
                                            } else {
                                                view.select_node(m_hovered_node);
                                                view.move_to_top(m_hovered_node);
                                            }
                                            
                                            m_hovered_node = null;
                                        }
                                    } else {
                                        if (m_hovered_node != null) {
                                            var select_time = EditorApplication.timeSinceStartup;
                                            

                                            if (!m_hovered_node.selected) {
                                                view.unselect_all_nodes();
                                                view.select_node(m_hovered_node);
                                                view.move_to_top(m_hovered_node);
                                            } else {
                                                view.unselect_all_nodes();
                                                view.select_node(m_hovered_node);
                                                if (select_time - m_last_select_node_time < 0.5) {
                                                    m_hovered_node.editor.on_double_click();
                                                }
                                            }

                                            m_last_select_node_time = select_time;
                                            m_hovered_node = null;
                                        }
                                    }
                                    
                                } else if (m_button == 1) {
                                    if (m_hovered_node == null) {
                                        if (m_multi_selection && view.m_selected_nodes.Count != 0) {
                                            if (view.m_selected_nodes.Count == 1) {
                                                view.pop_single_node_menu(ev.mousePosition);
                                            } else {
                                                view.pop_mulit_nodes_menu(ev.mousePosition);
                                            }
                                        } else {
                                            view.unselect_all_nodes();
                                            view.pop_create_node_menu(ev.mousePosition, null); 
                                        }
                                    } else {
                                        if (!m_hovered_node.selected) {
                                            if (m_multi_selection) {
                                                if (view.m_selected_nodes.Count == 0) {
                                                    view.pop_create_node_menu(ev.mousePosition, null);
                                                } if (view.m_selected_nodes.Count == 1) {
                                                    view.pop_single_node_menu(ev.mousePosition);
                                                } else {
                                                    view.pop_mulit_nodes_menu(ev.mousePosition);
                                                }
                                            } else {
                                                view.unselect_all_nodes();
                                                view.select_node(m_hovered_node);
                                                view.pop_single_node_menu(ev.mousePosition);
                                            }
                                        } else {
                                            if (view.m_selected_nodes.Count == 1) {
                                                view.pop_single_node_menu(ev.mousePosition);
                                            } else {
                                                view.pop_mulit_nodes_menu(ev.mousePosition);
                                            }
                                        }
                                    }
                                }
                                m_button = -1;
                                GUIUtility.hotControl = 0;
                            }
                            ev.Use();
                        }
                        break;
                }
            }

            public void on_button_down(GraphView view, Event ev) {
                if (m_button == -1) {
                    m_button = ev.button;
                    if (m_button == 0) {
                        m_multi_selection = ev.modifiers.HasFlag(is_mac() ? EventModifiers.Command : EventModifiers.Control);
                        (m_hovered_node, m_hovered_port) = view.pick_node_in_gui(ev.mousePosition);
                        if (m_hovered_node != null) {
                            if (m_hovered_port == null && !m_hovered_node.selected && !m_multi_selection) {
                                view.unselect_all_nodes();
                            }
                        } else {
                            if (!m_multi_selection) {
                                view.unselect_all_nodes();
                            }
                        }
                    } else if (m_button == 1) {
                        m_multi_selection = ev.modifiers.HasFlag(is_mac() ? EventModifiers.Command : EventModifiers.Control);
                        (m_hovered_node, m_hovered_port) = view.pick_node_in_gui(ev.mousePosition);
                    }
                    GUIUtility.hotControl = view.m_control_id;
                    if (GUIUtility.keyboardControl != view.m_control_id) {
                        GUIUtility.keyboardControl = view.m_control_id;
                    }
                }
            }

            private NodeView m_hovered_node;
            private PortView m_hovered_port;
            private bool m_multi_selection;
            private int m_button = -1;
            private double m_last_select_node_time = -1;
        }

        private NormalState m_state_normal = new NormalState();

        private class PanningState : State {
            public override void on_events(GraphView view) {

            }

            public override void on_hook_event(GraphView view) {
                var ev = Event.current;
                switch (ev.type) {
                    case EventType.MouseDown:
                        ev.Use();
                        break;

                    case EventType.MouseDrag:
                        if (ev.button == m_button) {
                            view.local_offset += ev.delta;
                        }
                        ev.Use();
                        break;

                    case EventType.MouseUp:          
                        if (ev.button == m_button) {
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                        }
                        ev.Use();
                        break;
                        
                    case EventType.Ignore:
                        if (ev.rawType == EventType.MouseUp) {
                            if (ev.rawType == EventType.MouseUp) {
                                view.m_current_state = view.m_state_normal;
                                GUIUtility.hotControl = 0;
                            }
                        }
                        break;
                }
            }

            public void start_panning(GraphView view, Event ev) {
                view.local_offset += ev.delta;
                view.m_current_state = this;
                m_button = ev.button;
            }

            private int m_button;
        }

        private PanningState m_state_panning = new PanningState();


        private class SelectionBoxState : State {
            public override void on_events(GraphView view) {

            }

            public override void on_hook_event(GraphView view) {
                var ev = Event.current;
                switch (ev.type) {
                    case EventType.MouseDown:
                        ev.Use();
                        break;

                    case EventType.MouseDrag:
                        if (ev.button == 0) {
                            m_current = view.gui_to_local(ev.mousePosition);
                            update_selections(view);
                        }
                        ev.Use();
                        break;

                    case EventType.MouseUp:
                        if (ev.button == 0) {
                            m_nodes.Clear();
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                        }
                        ev.Use();
                        break;

                    case EventType.Ignore:
                        if (ev.rawType == EventType.MouseUp) {
                            m_nodes.Clear();
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                        }
                        break;
                }
            }

            public override void on_repaint_after_nodes(GraphView view) {
                var pt1 = view.local_to_clipped_gui(m_down);
                var pt2 = view.local_to_clipped_gui(m_current);

                var r = new Rect(pt1, pt2 - pt1);

                Handles.color = Color.white;
                Handles.DrawSolidRectangleWithOutline(r, new Color(0, 0, 0, 0.1f), new Color(1, 1, 1, 0.6f));
            }

            public void start_selection_box(GraphView view, Event ev) {
                m_down = view.gui_to_local(ev.mousePosition - ev.delta);
                m_current = view.gui_to_local(ev.mousePosition);
                view.m_current_state = this;
                
                update_selections(view);
            }

            private void update_selections(GraphView view) {
                var min = Vector2.Min(m_down, m_current);
                var max = Vector2.Max(m_down, m_current);
                var r = new Rect(view.local_to_clipped_gui(min), max - min);
                foreach (var node in view.m_nodes) {
                    bool flag = false;
                    if (node.overlaps(r)) {
                        if (m_nodes.Add(node)) {
                            flag = true;
                        }
                    } else {
                        if (m_nodes.Remove(node)) {
                            flag = true;
                        }
                    }
                    if (flag) {
                        if (node.selected) {
                            view.unselect_node(node);
                        } else {
                            view.select_node(node);
                        }
                    }
                }
            }

            Vector2 m_down;
            Vector2 m_current;
            HashSet<NodeView> m_nodes = new HashSet<NodeView>();
        }

        private SelectionBoxState m_state_selection_box = new SelectionBoxState();

        private class MovingSelectedNodesState : State {
            public override void on_events(GraphView view) {

            }

            public override void on_hook_event(GraphView view) {
                var ev = Event.current;
                switch (ev.type) {
                    case EventType.MouseDown:
                        ev.Use();
                        break;

                    case EventType.MouseDrag:
                        if (ev.button == 0) {
                            move(view, ev.delta);
                        }
                        ev.Use();
                        break;

                    case EventType.MouseUp:
                        if (ev.button == 0) {
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                            end(view);
                        }
                        ev.Use();
                        break;

                    case EventType.Ignore:
                        if (ev.rawType == EventType.MouseUp) {
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                            end(view);
                        }
                        break;
                }
            }

            public void start_moving(GraphView view, Event ev) {
                m_delta = Vector2.zero;
                view.m_current_state = this;
                m_cmd = new GraphUndo.MoveNodes(view.m_selected_nodes);
                move(view, ev.delta);
            }

            private void end(GraphView view) {
                m_cmd.update_new_values();
                if (m_cmd.validate()) {
                    view.undo.record(m_cmd);
                }
                m_cmd = null;
            }

            private void move(GraphView view, Vector2 delta) {
                m_delta += delta * view.zoom;
                delta.x = Mathf.Round(m_delta.x * (1f / 16)) * 16;
                delta.y = Mathf.Round(m_delta.y * (1f / 16)) * 16;
                if (delta != Vector2.zero) {
                    m_delta -= delta;
                    foreach (var node in view.m_selected_nodes) {
                        node.position += delta;
                        node.on_layout_for_recull();
                    }
                }
            }

            Vector2 m_delta;
            GraphUndo.MoveNodes m_cmd;
        }

        private MovingSelectedNodesState m_state_moving_selected_nodes = new MovingSelectedNodesState();

        private class ConnectingPortState : State {
            public override void on_events(GraphView view) {

            }

            public override void on_hook_event(GraphView view) {
                var ev = Event.current;
                switch (ev.type) {
                    case EventType.MouseDown:
                        ev.Use();
                        break;

                    case EventType.MouseDrag:
                        if (ev.button == 0) {
                            update(view, ev.mousePosition);
                        }
                        ev.Use();
                        break;

                    case EventType.MouseUp:
                        if (ev.button == 0) {
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                            end(view);
                        }
                        ev.Use();
                        break;

                    case EventType.KeyDown:
                        if (ev.keyCode == KeyCode.None) {
                            ev.Use();
                        } else if (ev.keyCode == KeyCode.Tab) {
                            switch_candidate(view);
                            ev.Use();
                        }
                        break;

                    case EventType.Ignore:
                        if (ev.rawType == EventType.MouseUp) {
                            view.m_current_state = view.m_state_normal;
                            GUIUtility.hotControl = 0;
                            end(view);
                        }
                        break;
                }
            }

            public override void on_repaint_before_nodes(GraphView view) {
                Vector2 inlet, outlet;
                Color color = m_port.color;
                if (m_port.port.io.is_input()) {
                    inlet = m_port.node.pos_in_clipped_gui + m_port.position.center;
                    if (m_target_port == null) {
                        outlet = m_target;
                        color.a *= 0.55f;
                    } else {
                        outlet = m_target_port.node.pos_in_clipped_gui + m_target_port.position.center;
                    }
                } else {
                    if (m_target_port == null) {
                        inlet = m_target;
                        color.a *= 0.55f;
                    } else {
                        inlet = m_target_port.node.pos_in_clipped_gui + m_target_port.position.center;
                    }
                    outlet = m_port.node.pos_in_clipped_gui + m_port.position.center;
                }
                view.draw_connection(outlet, inlet, color);
            }

            public void start(GraphView view, PortView port, ConnectionView conn, Vector2 pt, ConnectionView[] candidates, PortView candidates_owner) {
                m_port = port;
                m_removed_conn = conn;
                m_candidates = candidates;
                m_candidates_owner = candidates_owner;
                view.m_current_state = this;
                update(view, pt);

                if (m_removed_conn != null) {

                    m_removed_conn.output.node.editor.on_port_connecting(m_removed_conn.output.port, true);
                    m_removed_conn.input.node.editor.on_port_connecting(m_removed_conn.input.port, true);

                    view.remove_connection_unchecked(m_removed_conn);
                }
            }

            private void update(GraphView view, Vector2 pt) {
                m_target = view.gui_to_clipped_gui(pt);
                update(view);
            }

            private void update(GraphView view) {
                m_target_port = null;
                var (target, target_port) = view.pick_node_in_clipped_gui(m_target);
                if (target_port != null && target_port.node != m_port.node) {
                    if (m_port.port.can_connect_with(m_port.node.editor.node, target.editor.node, target_port.port)) {
                        if (view.m_editor.validate_connection(m_port, target_port)) {
                            m_target_port = target_port;
                        }
                    }
                }
            }

            private void end(GraphView view) {
                if (m_target_port != null) {
                    ConnectionView conn = null;
                    if (m_port.port.io.is_input()) {
                        if (m_removed_conn != null && m_removed_conn.output == m_target_port) {
                            view.add_connection_unchecked(m_removed_conn);
                        } else {
                            conn = new ConnectionView(m_port as InputPortView, m_target_port as OutputPortView);
                        }
                    } else {
                        if (m_removed_conn != null && m_removed_conn.input == m_target_port) {
                            view.add_connection_unchecked(m_removed_conn);
                        } else {
                            conn = new ConnectionView(m_target_port as InputPortView, m_port as OutputPortView);
                        }
                    }
                    if (conn != null) {
                        ConnectionView old_connection = null;
                        if (!m_target_port.port.can_mulit_connect) {
                            old_connection = m_target_port.get_first_connection();
                        }
                        view.undo.begin_group();
                        var cmd = new GraphUndo.CreateConnection(conn, old_connection, m_removed_conn);
                        view.undo.record(cmd);
                        if (old_connection != null) {
                            old_connection.input.node.editor.on_port_connecting(old_connection.input.port, true);
                            old_connection.output.node.editor.on_port_connecting(old_connection.output.port, true);
                            view.remove_connection_unchecked(cmd.old_connection);
                        }
                        view.add_connection_unchecked(cmd.new_connection);
                        if (old_connection != null) {
                            old_connection.input.node.editor.on_port_connecting(old_connection.input.port, false);
                            old_connection.output.node.editor.on_port_connecting(old_connection.output.port, false);
                        }
                        view.undo.end_group();
                    }
                } else if (m_removed_conn != null) {
                    var cmd = new GraphUndo.DestroyConnection(m_removed_conn);
                    view.undo.record(cmd);
                } else {
                    view.pop_create_node_menu(Event.current.mousePosition, m_port);
                }

                if (m_removed_conn != null) {
                    m_removed_conn.input.node.editor.on_port_connecting(m_removed_conn.input.port, false);
                    m_removed_conn.output.node.editor.on_port_connecting(m_removed_conn.output.port, false);
                }
                
                m_port = null;
                m_target_port = null;
                m_removed_conn = null;
            }

            private void switch_candidate(GraphView view) {
                if (m_candidates == null || m_candidates.Length == 1) {
                    return;
                }
                int index = 0;
                for (int i = 0; i < m_candidates.Length; ++i) {
                    if (m_candidates[i] == m_removed_conn) {
                        index = (i + 1) % m_candidates.Length;
                    }
                }
                m_removed_conn.input.node.editor.on_port_connecting(m_removed_conn.input.port, false);
                m_removed_conn.output.node.editor.on_port_connecting(m_removed_conn.output.port, false);
                view.add_connection_unchecked(m_removed_conn);

                m_removed_conn = m_candidates[index];
                if (m_candidates_owner.port.io.is_input()) {
                    m_port = m_removed_conn.output;
                } else {
                    m_port = m_removed_conn.input;
                }
                m_removed_conn.output.node.editor.on_port_connecting(m_removed_conn.output.port, true);
                m_removed_conn.input.node.editor.on_port_connecting(m_removed_conn.input.port, true);

                view.remove_connection_unchecked(m_removed_conn);
                update(view);
            }

            PortView m_port;
            Vector2 m_target;
            PortView m_target_port;
            ConnectionView m_removed_conn;
            ConnectionView[] m_candidates;
            PortView m_candidates_owner;
        }

        private ConnectingPortState m_state_connecting_port = new ConnectingPortState();

        public static bool is_mac() => SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;

        internal void draw_connection(Vector2 outlet_point, Vector2 inlet_point, Color color) {
            var thickness = 5f;
            var inv_room = 1 / m_zoom;
            var pt1 = outlet_point;
            var pt2 = inlet_point;
            var distance = Vector2.Distance(pt1, pt2);
            var output_tangent = distance * 0.01f * m_zoom * Vector2.right;
            var input_tangent = distance * 0.01f * m_zoom * Vector2.left;

            var tan1 = pt1 + 50 * inv_room * output_tangent;
            var tan2 = pt2 + 50 * inv_room * input_tangent;
            Handles.DrawBezier(outlet_point, inlet_point, tan1, tan2, color, null, thickness);
        }

        private HashSet<ConnectionView> m_connections = new HashSet<ConnectionView>();

        private Vector2 m_menu_position;
        private GenericMenu m_menu;
        private CreateNodeMenu m_create_node_menu;
        private void pop_single_node_menu(Vector2 gui_pos) {
            var node = m_selected_nodes[0];
            var menu = new GenericMenu();
            if (!node.editor.unique) {
                if (!node.editor.has_create_wizard) {
                    menu.AddItem(new GUIContent("Duplicate"), false, duplicate_selected_nodes);
                } else {
                    menu.AddDisabledItem(new GUIContent("Duplicate"));
                }
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Delete"), false, delete_selected_nodes);
            } else {
                menu.AddDisabledItem(new GUIContent("Duplicate"));
                menu.AddSeparator(string.Empty);
                menu.AddDisabledItem(new GUIContent("Delete"));
            }
            node.editor.on_context_menu(menu);
            m_menu_position = gui_pos;
            m_menu = menu;
            EditorApplication.delayCall += send_pop_menu_event; 
        }

        private void pop_mulit_nodes_menu(Vector2 gui_pos) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Duplicate"), false, duplicate_selected_nodes);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Delete"), false, delete_selected_nodes);
            editor.on_context_menu(menu);
            m_menu_position = gui_pos;
            m_menu = menu;
            EditorApplication.delayCall += send_pop_menu_event; 
        }

        private void pop_create_node_menu(Vector2 gui_pos, PortView port) {
            m_create_node_menu = new CreateNodeMenu(this, gui_to_local(gui_pos), port);
            m_menu_position = gui_pos;
            EditorApplication.delayCall += send_pop_menu_event;
        }

        private void send_pop_menu_event() {
            m_window.SendEvent(EditorGUIUtility.CommandEvent("PopMenu"));
        }

        private void duplicate_selected_nodes() {
            var nodes = new List<NodeView>(m_selected_nodes.Count);
            int count = 0;
            foreach (var node in m_selected_nodes) {
                if (node.editor.unique || node.editor.has_create_wizard) {
                    nodes.Add(null);
                } else {
                    ++count;
                    nodes.Add(node);
                }
            }
            if (count == 0) {
                return;
            }
            undo.begin_group();
            for (int i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                if (node == null) {
                    continue;
                }
                var node_editor = node.editor.clone(editor);
                node_editor.node.position = node.editor.node.position + new Vector2(16, 16);
                nodes[i] = node = new NodeView(this, node_editor);
                add_node(node, true);
            }
            for (int i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                if (node == null) {
                    continue;
                }
                var original = m_selected_nodes[i];
                var iter = original.enumerate_ports();
                while (iter.MoveNext()) {
                    var port = iter.Current;
                    
                    foreach (var conn in port.connections) {
                        PortView peer = conn.input == port ? (PortView)conn.output : conn.input;
                        if (!peer.node.selected) {
                            continue;
                        }
                        var idx = m_selected_nodes.IndexOf(peer.node);
                        if (idx < i) {
                            continue;
                        }
                        var cloned = nodes[idx];
                        if (cloned == null) {
                            continue;
                        }
                        var cloned_port1 = node.find_port(original.editor, port.port);
                        var cloned_port2 = cloned.find_port(peer.node.editor, peer.port);
                        ConnectionView new_conn;
                        if (port.port.io.is_input()) {
                            new_conn = new ConnectionView((InputPortView)cloned_port1, (OutputPortView)cloned_port2);
                        } else {
                            new_conn = new ConnectionView((InputPortView)cloned_port2, (OutputPortView)cloned_port1);
                        }
                        undo.record(new GraphUndo.CreateConnection(new_conn, null, null));
                        add_connection_unchecked(new_conn);
                    }
                }
            }
            foreach (var node in nodes) {
                node?.editor.on_duplicate_done(m_selected_nodes, nodes);
            }
            undo.end_group();

            unselect_all_nodes();
            foreach (var node in nodes) {
                if (node != null) {
                    select_node(node);
                }
            }
        }

        private void delete_selected_nodes() {
            var nodes = new List<NodeView>();
            foreach (var node in m_selected_nodes) {
                if (node.editor.unique) {
                    continue;
                }
                nodes.Add(node);
            }
            if (nodes.Count == 0) {
                return;
            }
            nodes.Reverse();
            undo.begin_group();
            var conns = new HashSet<ConnectionView>();
            foreach (var node in nodes) {
                var iter = node.enumerate_ports();
                while (iter.MoveNext()) {
                    var port = iter.Current;
                    foreach (var conn in port.get_connection_array()) {
                        conns.Add(conn);
                    }
                }
            }
            foreach (var conn in conns) {
                undo.record(new GraphUndo.DestroyConnection(conn));
                remove_connection_unchecked(conn);
            }
            foreach (var node in nodes) {
                undo.record(new GraphUndo.RemoveNode(node));
                remove_node_unchecked(node);
            }
            undo.end_group();
        }

        public string name => m_editor?.graph.name;

        public bool open(Graph graph) {
            close();

            if (graph == null) {
                return false;
            }
            
            m_editor = GraphTypeCache.create_graph_editor(graph.GetType());
            if (m_editor == null) {
                Debug.LogError($"Graph \'{graph.GetType().Name}\': create editor failed");
                return false;
            }
            m_editor.attach(graph, this);

            if (graph.nodes != null) {
                var dict = new Dictionary<Node, NodeView>();
                foreach (var node in graph.nodes) {
                    var node_editor = GraphTypeCache.create_node_editor(node.GetType());
                    if (node_editor == null) {
                        Debug.LogError($"GraphNode \'{node.GetType().Name}\': create editor failed");
                    } else {
                        node_editor.attach(m_editor, node);
                        var node_view = new NodeView(this, node_editor);
                        add_node_without_undo(node_view, false);
                        dict.Add(node, node_view);
                    }
                }

                if (graph.connections != null) {
                    foreach (var conn in graph.connections) {
                        if (dict.TryGetValue(conn.input_node, out var input) && dict.TryGetValue(conn.output_node, out var output)) {
                            var ip = input.find_input_port(conn.input_port);
                            var op = output.find_output_port(conn.output_port);
                            if (ip != null && op != null) {
                                var cv = new ConnectionView((InputPortView)ip, (OutputPortView)op);
                                add_connection_unchecked(cv, false);
                            }
                        }
                    }
                }
            }

            editor.on_open();
            return true;
        }

        public void close() {
            editor?.on_close();

            m_nodes.Clear();
            m_selected_nodes.Clear();
            m_connections.Clear();
            m_editor = null;
            
            zoom = 1;
            local_offset = Vector2.zero;
        }

        public bool save() {
            if (m_editor != null) {
                m_editor.on_graph_saving();
                var graph = m_editor.graph;
                graph.nodes = new Node[m_nodes.Count];
                for (int i = 0; i < graph.nodes.Length; ++i) {
                    var node = m_nodes[i];
                    node.editor.on_node_saving();
                    graph.nodes[i] = node.editor.node;
                }
                var conns = new List<Connection>(m_connections.Count);
                foreach (var conn in m_connections) {
                    conns.Add(new Connection {
                        input_node = conn.input.node.editor.node,
                        input_port = conn.input.port,
                        output_node = conn.output.node.editor.node,
                        output_port = conn.output.port,
                    });
                }
                graph.connections = conns.ToArray();
                m_editor.on_graph_saved();
                return true;
            }
            return false;
        }

        public GraphEditor editor => m_editor;
        GraphEditor m_editor;

        public interface IDiv {
            bool contains(Vector2 point);
            void on_gui();
        }

        private List<IDiv> m_divs = new List<IDiv>();

        public void add_div(IDiv div) {
            if (!m_divs.Contains(div)) {
                m_divs.Add(div);
            }
        }

        public void remove_div(IDiv div) {
            m_divs.Remove(div);
        }

        public IDiv pick_div(Vector2 point) {
            foreach (var div in m_divs) {
                if (div.contains(point)) {
                    return div;
                }
            }
            return null;
        }

        public void on_divs_gui() {
            foreach (var div in m_divs) {
                div.on_gui();
            }
        }
    }

}