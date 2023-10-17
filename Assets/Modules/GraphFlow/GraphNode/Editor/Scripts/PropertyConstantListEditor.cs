using CalcExpr;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GraphNode.Editor {

    [PropertyEditor(typeof(List<Constant>))]
    public class PropertyConstantListEditor : GenericPropertyEditor, GraphView.IDiv {

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init();
        }

        private void init() {
            System.Type ty = null;
            if (m_fi != null) {
                ty = m_fi.FieldType;
            } else if (m_obj != null) {
                ty = m_obj.GetType();
            } else {
                throw new System.ArgumentException();
            }
            var value = get_value() as System.Collections.IList;
            if (value == null) {
                value = System.Activator.CreateInstance(ty) as System.Collections.IList;
                set_value(value);
            }
            for (; ; ) {
                if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(List<>)) {
                    m_element_type = ty.GetGenericArguments()[0];
                    break;
                }
                ty = ty.BaseType;
            }


            m_list = new ReorderableList(value, m_element_type, true, true, true, true);
            m_list.elementHeight = EditorGUIUtility.singleLineHeight;
            m_list.drawHeaderCallback = draw_header;
            m_list.onAddCallback = on_add;
            m_list.onRemoveCallback = on_remove;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = reorder_with_details;
        }



        public override void clone_to(object target) {
            if (m_fi != null) {
                var value = System.Activator.CreateInstance(m_fi.DeclaringType) as System.Collections.IList;
                foreach (Constant item in m_list.list) {
                    var e = System.Activator.CreateInstance(m_element_type) as Constant;
                    e.type = item.type;
                    e.name = item.name;
                    e.value = item.value;
                    value.Add(e);
                }
                m_fi.SetValue(target, value);
            }
        }

        public override void on_body_gui() {

        }

        public override void on_inspector_enable() {
            m_graph.view.add_div(this);
        }

        public override void on_inspector_disable() {
            m_editing = 0;
            m_cmd = null;

            m_graph.view.remove_div(this);
        }



        public override void on_inspector_gui() {
            m_list.DoLayoutList();
            EditorGUILayout.Space(0);
            if (Event.current.type == EventType.Repaint) {
                var pos = GUILayoutUtility.GetLastRect().position;

                pos.y -= m_list.GetHeight() - m_list.headerHeight;

                var inspector_rect = GraphInspector.current.rect;
                pos.x += inspector_rect.x - m_edit_rect.width - GraphEditorWindow.INSPECTOR_SEPARATOR;
                pos.y += inspector_rect.y + m_list.index * EditorGUIUtility.singleLineHeight;

                if (m_edit_rect.position != pos) {
                    m_edit_rect.position = pos;
                    m_graph.view.window.Repaint();
                }
            }
        }

        private System.Type m_element_type;
        private ReorderableList m_list;

        private int m_editing = 0;
        private GraphUndo.ICommand m_cmd;

        private void draw_header(Rect rect) {
            GUI.Label(rect, m_name);
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.LabelField(rect, get_variable_display(m_list.list[index] as Constant));
        }

        private void reorder_with_details(ReorderableList list, int oldIndex, int newIndex) {
            m_graph.view.undo.record(new ReorderItem {
                obj = this,
                old_value = oldIndex,
                new_value = newIndex,
            });
            notify_changed(true);
        }

        private void on_add(ReorderableList _) {
            var item = System.Activator.CreateInstance(m_element_type) as Constant;
            item.type = ValueType.Integer;
            m_list.list.Add(item);
            m_graph.view.undo.record(new AddItem(this, item));
            m_list.index = m_list.list.Count - 1;
            m_list.GrabKeyboardFocus();
            notify_changed(true);
        }

        private void on_remove(ReorderableList _) {
            var item = m_list.list[m_list.index] as Constant;
            m_graph.view.undo.record(new RemoveItem {
                obj = this,
                index = m_list.index,
                item = item,
            });
            m_list.list.RemoveAt(m_list.index);
            m_list.index = -1;
            notify_changed(true);
        }

        private static bool check_name(string name) {
            if (name.Length != 0) {
                var first = name[0];
                if (first != '_' && (first < 'A' || first > 'Z') && (first < 'a' || first > 'z')) {
                    return false;
                }
                for (int i = 1; i < name.Length; ++i) {
                    var c = name[i];
                    if (c != '_' && (c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9')) {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string get_variable_display(Constant item) {
            return $"{(string.IsNullOrEmpty(item.name) ? "<unnamed>" : item.name)}: {CalcExpr.Utility.name(item.type)} = {CalcExpr.Utility.convert_from(item.type, item.value)}";
        }

        enum ConstantType {
            Integer = ValueType.Integer,
            Floating = ValueType.Floating,
            Boolean = ValueType.Boolean,
        }

        Rect m_edit_rect = new Rect(0, 0, 192, 0);

        bool GraphView.IDiv.contains(Vector2 point) {
            return m_list.index != -1 && m_edit_rect.Contains(point);
        }

        void GraphView.IDiv.on_gui() {
            if (m_list.index == -1) {
                return;
            }
            GUILayout.BeginArea(m_edit_rect);
            GUILayout.BeginVertical("Constant", GUI.skin.window);
            EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth;
            var index = m_list.index;
            var item = m_list.list[index] as Constant;
            var graph = m_graph.view;
            GUI.SetNextControlName("VariableType");
            var ty = (ValueType)EditorGUILayout.EnumPopup("Type", (ConstantType)item.type);
            if (ty != item.type) {
                graph.undo.record(new ChangeType(this, item, item.type, ty));
                item.type = ty;
                notify_changed(true);
            }

            GUI.SetNextControlName("VariableName");
            var new_name = EditorGUILayout.TextField("Name", item.name);
            if (new_name != item.name && check_name(new_name)) {
                if (m_cmd is ChangeName cmd && graph.undo.is_last(cmd)) {
                    cmd.new_value = new_name;
                } else {
                    m_cmd = new ChangeName(this, item, item.name, new_name);
                    graph.undo.record(m_cmd);
                }
                item.name = new_name;
                notify_changed(true);
            }

            GUI.SetNextControlName("VariableValue");
            switch (ty) {
                case ValueType.Integer: {
                    var v = item;
                    var value = (uint)EditorGUILayout.IntField("Value", (int)v.value);
                    if (value != v.value) {
                        if (m_cmd is ChangeValue cmd && graph.undo.is_last(cmd)) {
                            cmd.new_value = value;
                        } else {
                            m_cmd = new ChangeValue {
                                item = v,
                                old_value = v.value,
                                new_value = value,
                            };
                            graph.undo.record(m_cmd);
                        }
                        v.value = value;
                        notify_changed(true);
                    }
                    break;
                }

                case ValueType.Floating: {
                    var v = item;
                    var value = CalcExpr.Utility.convert_to(EditorGUILayout.FloatField("Value", CalcExpr.Utility.convert_float_from(v.value)));
                    if (value != v.value) {
                        if (m_cmd is ChangeValue cmd && graph.undo.is_last(cmd)) {
                            cmd.new_value = value;
                        } else {
                            m_cmd = new ChangeValue {
                                item = v,
                                old_value = v.value,
                                new_value = value,
                            };
                            graph.undo.record(m_cmd);
                        }
                        v.value = value;
                        notify_changed(true);
                    }
                    break;
                }

                case ValueType.Boolean: {
                    var v = item;
                    var value = EditorGUILayout.Toggle("Value", v.value != 0) ? 1u : 0;
                    if (value != v.value) {
                        if (m_cmd is ChangeValue cmd && graph.undo.is_last(cmd)) {
                            cmd.new_value = value;
                        } else {
                            m_cmd = new ChangeValue {
                                item = v,
                                old_value = v.value,
                                new_value = value,
                            };
                            graph.undo.record(m_cmd);
                        }
                        v.value = value;
                        notify_changed(true);
                    }
                    break;
                }
            }

            switch (GUI.GetNameOfFocusedControl()) {
                case "VariableType":
                    if (m_editing != 1) {
                        m_editing = 1;
                        m_cmd = null;
                    }
                    break;
                case "VariableName":
                    if (m_editing != 2) {
                        m_editing = 2;
                        m_cmd = null;
                    }
                    break;
                case "VariableValue":
                    if (m_editing != 3) {
                        m_editing = 3;
                        m_cmd = null;
                    }
                    break;
                default:
                    if (m_editing != 0) {
                        m_cmd = null;
                        m_editing = 0;
                    }
                    break;
            }
            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) {
                var last = GUILayoutUtility.GetLastRect();
                if (last.height != m_edit_rect.height) {
                    m_edit_rect.height = last.height;
                }
            }
            GUILayout.EndArea();
            if (m_editing == 0 && !m_list.HasKeyboardControl()) {
                m_list.index = -1;
                m_graph.view.window.Repaint();
            }
        }

        private class AddItem : GraphUndo.ICommand {
            public PropertyConstantListEditor obj;
            public Constant item;

            public AddItem(PropertyConstantListEditor obj, Constant item) {
                this.obj = obj;
                this.item = item;
            }

            public void undo() {
                obj.m_list.list.RemoveAt(obj.m_list.list.Count - 1);
                obj.notify_changed(false);
            }

            public void redo() {
                obj.m_list.list.Add(item);
                obj.notify_changed(false);
            }

            public int dirty_count => 1;
        }

        private class RemoveItem : GraphUndo.ICommand {
            public PropertyConstantListEditor obj;
            public Constant item;
            public int index;

            public void undo() {
                obj.m_list.list.Insert(index, item);
                obj.notify_changed(false);
            }

            public void redo() {
                obj.m_list.list.RemoveAt(index);
                obj.notify_changed(false);
            }

            public int dirty_count => 1;
        }

        private class ChangeType : GraphUndo.ChangeValue<ValueType> {
            public PropertyConstantListEditor obj;
            Constant item;

            protected override void set_value(ref ValueType old_value, ref ValueType value) {
                item.type = value;
                obj.notify_changed(false);
            }

            public ChangeType(PropertyConstantListEditor obj, Constant item, ValueType old_value, ValueType new_value) {
                this.obj = obj;
                this.item = item;
                this.old_value = old_value;
                this.new_value = new_value;
            }

        }

        private class ChangeName : GraphUndo.ChangeValue<string> {
            public PropertyConstantListEditor obj;
            public Constant item;

            protected override void set_value(ref string old_value, ref string value) {
                item.name = value;
                obj.notify_changed(false);
            }

            public ChangeName(PropertyConstantListEditor obj, Constant item, string old_value, string new_value) {
                this.obj = obj;
                this.item = item;
                this.old_value = old_value;
                this.new_value = new_value;

            }
        }

        private class ChangeValue : GraphUndo.ChangeValue<uint> {
            public Constant item = null;

            protected override void set_value(ref uint _, ref uint value) {
                item.value = value;
            }
        }

        private class ReorderItem : GraphUndo.ChangeValue<int> {
            public PropertyConstantListEditor obj;

            protected override void set_value(ref int old_value, ref int new_value) {
                var list = obj.m_list.list;
                var item = list[old_value];
                list.RemoveAt(old_value);
                list.Insert(new_value, item);
                if (obj.m_list.index == old_value) {
                    obj.m_list.index = new_value;
                }
                obj.notify_changed(false);
            }
        }
    }
}