
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GraphNode.Editor {

    public abstract class PropertyEditor {
        public abstract void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node);
        public abstract void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null);
        public abstract string name { get; }
        public abstract string raw_name { get; }
        public abstract int order { get; }
        public abstract object obj { get; }
        public abstract FieldInfo fi { get; }
        public abstract void clone_to(object target);
        public abstract void on_body_gui();
        public abstract void on_inspector_enable();
        public abstract void on_inspector_disable();
        public abstract void on_inspector_gui();

        public abstract bool enabled { get; set; }

        public virtual void on_graph_open() { }
        public virtual void on_node_add_to_graph() { }
        public virtual void on_node_remove_from_graph() { }
        public virtual void on_node_duplicated_done(List<NodeView> source_nodes, List<NodeView> duplicated_nodes) { }

        public event System.Action<PropertyEditor, bool> on_changed;

        protected virtual void notify_changed(bool by_user) {
            on_changed?.Invoke(this, by_user);
        }
    }

    public abstract class GenericPropertyEditor : PropertyEditor {

        public override object obj => m_obj;
        public override FieldInfo fi => m_fi;

        public override bool enabled { get; set; } = true;

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            m_graph = graph;
            m_node = node;
            m_obj = obj;
            m_fi = fi;
            var name_attr = fi.GetCustomAttribute<DisplayAttribute>();
            if (name_attr != null) {
                m_name = string.Format(name_attr.name, m_fi.Name);
            } else {
                m_name = m_fi.Name;
            }
            var order_attr = fi.GetCustomAttribute<SortedOrderAttribute>();
            if (order_attr != null) {
                m_order = order_attr.order;
            }
            m_show_in_body = fi.GetCustomAttribute<ShowInBodyAttribute>();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            m_graph = graph;
            m_node = node;
            m_obj = obj;
            m_fi = fi;
            m_name = name;
            m_order = order;
            m_show_in_body = show_in_body;
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, m_fi.GetValue(m_obj));
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                if (m_show_in_body.format == null) {
                    GUILayout.Label(get_value()?.ToString());
                } else {
                    GUILayout.Label(string.Format(m_show_in_body.format, get_value()));
                }
            }
        }

        public override string name => m_name;
        protected string m_name;

        public void _set_name(string name) {
            m_name = name;
        }

        public override string raw_name => m_fi != null ? m_fi.Name : string.Empty;

        public override int order => m_order;
        protected int m_order = 0;


        protected object m_obj;
        protected FieldInfo m_fi;

        protected GraphEditor m_graph;
        protected NodeEditor m_node;

        public virtual object get_value() {
            return m_fi != null ? m_fi.GetValue(m_obj) : m_obj;
        }

        public virtual void set_value(object value) {
            if (m_fi != null) {
                m_fi.SetValue(m_obj, value);
            } else {
                m_obj = value;
            }
        }

        public class ChangeValue<T> : GraphUndo.ICommand {

            public T old_value;
            public T new_value;
            public GenericPropertyEditor editor;
            public GraphUndo.CommandGroup associated;

            
            public virtual void undo() {
                editor.set_value(old_value);
                associated?.undo();
                editor.notify_changed(false);
            }

            public virtual void redo() {
                editor.set_value(new_value);
                associated?.redo();
                editor.notify_changed(false);
            }

            public int dirty_count => 1;
        }

        public ChangeValue<T> create_undo_command<T>(T old_value, T new_value, GraphUndo.CommandGroup associated) {
            return new ChangeValue<T> { editor = this, old_value = old_value, new_value = new_value, associated = associated };
        }

        protected override void notify_changed(bool by_user) {
            base.notify_changed(by_user);
            if (m_show_in_body != null) {
                m_node.view.size_changed = true;
            }
        }

        protected ShowInBodyAttribute m_show_in_body;
    }

    [PropertyEditor(typeof(bool))]
    public class PropertyBoolEditor : GenericPropertyEditor {
        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            var value = (bool)get_value();
            bool new_value;
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value = EditorGUILayout.Toggle(name, value);
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value = EditorGUILayout.Toggle(name, value);
            }
            if (new_value != value) {
                if (m_cmd != null && graph.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
        }

        ChangeValue<bool> m_cmd = null;
    }

    [PropertyEditor(typeof(int))]
    public class PropertyIntEditor : GenericPropertyEditor {

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            var value = (int)get_value();
            int new_value;
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value = EditorGUILayout.IntField(name, value);
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value = EditorGUILayout.IntField(name, value);
            }
            if (new_value != value) {
                if (m_cmd != null && graph.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
        }

        ChangeValue<int> m_cmd = null;
    }

    [PropertyEditor(typeof(float))]
    public class PropertyFloatEditor : GenericPropertyEditor {

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            var value = (float)get_value();
            float new_value;
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value = EditorGUILayout.FloatField(name, value);
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value = EditorGUILayout.FloatField(name, value);
            }
            if (new_value != value) {
                if (m_cmd != null && graph.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
        }

        ChangeValue<float> m_cmd = null;
    }

    [PropertyEditor(typeof(string))]
    public class PropertyStringEditor : GenericPropertyEditor {

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            var value = (string)get_value();
            string new_value;
            EditorGUI.BeginChangeCheck();
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value = EditorGUILayout.TextField(name, value);
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value = EditorGUILayout.TextField(name, value);
            }
            if (EditorGUI.EndChangeCheck() && validate(new_value)) {
                if (m_cmd != null && graph.window.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.window.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
        }

        protected virtual bool validate(string new_value) {
            return true;
        }

        ChangeValue<string> m_cmd = null;
    }

    [PropertyEditor(typeof(System.Enum))]
    public class PropertyEnumEditor : GenericPropertyEditor {

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            var value = (System.Enum)get_value();
            var new_value = EditorGUILayout.EnumPopup(name, value);
            if (!new_value.Equals(value)) {
                set_value(new_value);
                graph.undo.begin_group();
                notify_changed(true);
                var cmd = create_undo_command(value, new_value, graph.undo.cancel_group());
                graph.undo.record(cmd);
            }
        }
    }


    [PropertyEditor(typeof(Comment))]
    public class PropertyCommentEditor : GenericPropertyEditor {

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            m_node = node;
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order, ShowInBodyAttribute show_in_body) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            m_node = node;
        }

        public override void on_body_gui() {
            var value = (Comment)get_value();
            if (!string.IsNullOrEmpty(value.content)) {
                GUILayout.Label(value.content, GraphResources.styles.comment);
            }
        }

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            EditorGUILayout.PrefixLabel(name);
            var value = (Comment)get_value();
            Comment new_value;
            EditorGUI.BeginChangeCheck();
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value.content = EditorGUILayout.TextArea(value.content, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2), GUILayout.MaxWidth(GraphInspector.current.max_width));
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value.content = EditorGUILayout.TextArea(value.content, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2), GUILayout.MaxWidth(GraphInspector.current.max_width));
            }
            if (EditorGUI.EndChangeCheck()) {
                if (m_cmd != null && graph.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
        }

        ChangeValue<Comment> m_cmd = null;
    }


    [PropertyEditor(typeof(LongString))]
    public class PropertyLongStringEditor : GenericPropertyEditor {

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            if (!string.IsNullOrEmpty(name)) {
                EditorGUILayout.PrefixLabel(name);
            }
            var value = (LongString)get_value();
            LongString new_value;
            EditorGUI.BeginChangeCheck();
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value.content = EditorGUILayout.TextArea(value.content, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2), GUILayout.MaxWidth(GraphInspector.current.max_width));
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value.content = EditorGUILayout.TextArea(value.content, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2), GUILayout.MaxWidth(GraphInspector.current.max_width));
            }
            if (EditorGUI.EndChangeCheck()) {
                if (m_cmd != null && graph.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
        }

        ChangeValue<LongString> m_cmd = null;
    }
}