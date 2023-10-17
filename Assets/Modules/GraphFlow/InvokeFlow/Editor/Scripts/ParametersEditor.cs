﻿
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using GraphNode;
using GraphNode.Editor;


namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(Parameters))]
    public class ParametersEditor : GenericPropertyEditor, GraphView.IDiv {

        public readonly List<Variable> available_variables = new List<Variable>();

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            m_node = node;
            m_val = (Parameters)fi.GetValue(obj);
            if (m_val == null) {
                m_val = new Parameters();
                fi.SetValue(obj, m_val);
            }
            init_list();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            m_node = node;
            m_val = (Parameters)obj ?? new Parameters();
            init_list();
        }

        private void init_list() {
            m_list = new ReorderableList(m_val, typeof(Parameter), true, true, true, true);
            m_list.elementHeight = EditorGUIUtility.singleLineHeight;
            m_list.drawHeaderCallback = draw_header;
            m_list.onAddCallback = on_add;
            m_list.onRemoveCallback = on_remove;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = reorder_with_details;

            var iter = m_val.enumerate_valid_variables();
            while (iter.MoveNext()) {
                available_variables.Add(iter.Current);
            }
        }



        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, m_val?.clone());
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

        private Parameters m_val;

        private ReorderableList m_list;

        private int m_editing = 0;
        private GraphUndo.ICommand m_cmd;

        protected override void notify_changed(bool by_user) {
            available_variables.Clear();
            var iter = m_val.enumerate_valid_variables();
            while (iter.MoveNext()) {
                available_variables.Add(iter.Current);
            }

            base.notify_changed(by_user);
            if (m_node is InvokeNodeEditor node) {
                node.build_stack_frame(true);
            } else if (m_graph is InvokeGraphEditor ge) {
                ge.build_stack_frame(true);
            }
        }

        private void draw_header(Rect rect) {
            GUI.Label(rect, m_name);
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            EditorGUI.LabelField(rect, get_parameter_display(m_val[index]));
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
            var item = new Parameter(VariableType.Integer, null);
            m_val.Add(item);
            m_list.index = m_val.Count - 1;
            m_list.GrabKeyboardFocus();

            var undo = m_graph.view.undo;
            undo.begin_group();
            notify_changed(true);
            undo.record(new AddItem(this, item, undo.cancel_group()));
        }

        private void on_remove(ReorderableList _) {
            var index = m_list.index;
            var item = m_val[index];
            m_val.RemoveAt(index);
            m_list.index = -1;

            var undo = m_graph.view.undo;
            undo.begin_group();
            notify_changed(true);
            undo.record(new RemoveItem(this, item, index, undo.cancel_group()));
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

        public static string get_parameter_display(Parameter item) {
            return $"{(string.IsNullOrEmpty(item.name) ? "<unnamed>" : item.name)}: {item.type.to_string()}";
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
            GUILayout.BeginVertical("Parameter", GUI.skin.window);
            EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth;
            var index = m_list.index;
            var item = m_val[index];
            var graph = m_graph.view;
            GUI.SetNextControlName("VariableType");
            var ty = (VariableType)EditorGUILayout.EnumPopup("Type", item.type);
            if (ty != item.type) {
                graph.undo.record(new ChangeType { 
                    obj = this,
                    item = item,
                    old_value = item.type,
                    new_value = ty,
                });
                item.change_type(ty);
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
            public ParametersEditor obj;
            public Parameter item;
            public GraphUndo.CommandGroup associated;

            public AddItem(ParametersEditor obj, Parameter item, GraphUndo.CommandGroup associated) {
                this.obj = obj;
                this.item = item;
                this.associated = associated;
            }

            public void undo() {
                obj.m_val.RemoveAt(obj.m_val.Count - 1);
                associated?.undo();
                obj.notify_changed(false);
            }

            public void redo() {
                obj.m_val.Add(item);
                associated?.redo();
                obj.notify_changed(false);
            }

            public int dirty_count => 1;
        }

        private class RemoveItem : GraphUndo.ICommand {
            public ParametersEditor obj;
            public Parameter item;
            public int index;
            public GraphUndo.CommandGroup associated;

            public RemoveItem(ParametersEditor obj, Parameter item, int index, GraphUndo.CommandGroup associated) {
                this.obj = obj;
                this.item = item;
                this.index = index;
                this.associated = associated;
            }
            public void undo() {
                obj.m_val.Insert(index, item);
                associated?.undo();
                obj.notify_changed(false);
            }

            public void redo() {
                obj.m_val.RemoveAt(index);
                associated?.redo();
                obj.notify_changed(false);
            }

            public int dirty_count => 1;
        }

        private class ChangeName : GraphUndo.ChangeValue<string> {
            public ParametersEditor obj;
            public Parameter item;

            protected override void set_value(ref string old_value, ref string value) {
                item.name = value;
                obj.notify_changed(false);
            }

            public ChangeName(ParametersEditor obj, Parameter item, string old_value, string new_value) {
                this.obj = obj;
                this.item = item;
                this.old_value = old_value;
                this.new_value = new_value;

            }
        }

        private class ChangeType : GraphUndo.ChangeValue<VariableType> {
            public ParametersEditor obj;
            public Parameter item;

            protected override void set_value(ref VariableType _, ref VariableType type) {
                item.change_type(type);
                obj.notify_changed(false);
            }
        }

        private class ReorderItem : GraphUndo.ChangeValue<int> {
            public ParametersEditor obj;

            protected override void set_value(ref int old_value, ref int new_value) {
                var item = obj.m_val[old_value];
                obj.m_val.RemoveAt(old_value);
                obj.m_val.Insert(new_value, item);
                if (obj.m_list.index == old_value) {
                    obj.m_list.index = new_value;
                }
                obj.notify_changed(false);
            }
        }
    }
}