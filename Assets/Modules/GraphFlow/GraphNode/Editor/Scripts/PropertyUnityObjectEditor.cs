
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {

    
    public class PropertyUnityObjectEditor<T> : GenericPropertyEditor where T : Object {
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_enable() {
            
        }

        public override void on_inspector_gui() {

            var old_value = get_value() as T;
            T new_value;

            var value_type = m_fi != null ? m_fi.FieldType : typeof(T);

            EditorGUI.BeginChangeCheck();
            if (m_cmd == null) {
                new_value = EditorGUILayout.ObjectField(name, old_value, value_type, false) as T;
            } else {
                GUI.SetNextControlName(name);
                new_value = EditorGUILayout.ObjectField(name, old_value, value_type, false) as T;
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            }
            if (EditorGUI.EndChangeCheck()) {
                validate(ref new_value);
                if (old_value != new_value) {
                    if (m_cmd != null && m_graph.view.undo.is_last(m_cmd)) {
                        m_cmd.new_value = new_value;
                        set_value(new_value);
                        m_graph.view.undo.begin_group();
                        notify_changed(true);
                        m_cmd.associated = m_graph.view.undo.cancel_group();
                        if (m_cmd.associated != null) {
                            m_cmd = null;
                        }
                    } else {
                        
                        set_value(new_value);
                        m_graph.view.undo.begin_group();
                        notify_changed(true);
                        m_cmd = create_undo_command(old_value, new_value, m_graph.view.undo.cancel_group());
                        m_graph.view.undo.record(m_cmd);
                        if (m_cmd.associated != null) {
                            m_cmd = null;
                        }
                    }

                }
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                string content;
                var obj = get_value() as T;
                if (obj == null) {
                    if (ReferenceEquals(obj, null)) {
                        content = "<None>";
                    } else {
                        content = "<Missing>";
                    }
                } else {
                    content = obj.name;
                }
                if (m_show_in_body.format == null) {
                    GUILayout.Label(content);
                } else {
                    GUILayout.Label(string.Format(m_show_in_body.format, content));
                }
            }
        }

        protected virtual void validate(ref T new_value) {

        }

        ChangeValue<T> m_cmd;
    }

    [PropertyEditor(typeof(Object))]
    public class UnityObjectEditor : PropertyUnityObjectEditor<Object> {

    }
}