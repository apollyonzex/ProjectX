
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace DialogFlow.Editor {

    [PropertyEditor(typeof(DialogText))]
    public class DialogTextEditor : GenericPropertyEditor {

        public DialogText target { get; set; }

        public DialogTextEditor() {
            m_content_editor.on_changed += (_, by_user) => notify_changed(by_user);
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init();
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, this.target.clone());
            }
        }

        public override void on_inspector_disable() {
            m_content_editor.on_inspector_disable();
            if (m_list.index >= 0 && m_list.index < m_list.count) {
                var (ty, val) = m_parameter_editors[m_list.index];
                ty.on_inspector_disable();
                val.on_inspector_disable();
            }
            m_list.index = -1;
            m_last_parameter_index = -1;
        }

        public override void on_inspector_enable() {
            m_content_editor.on_inspector_enable();
        }

        public override void on_inspector_gui() {
            m_content_editor.on_inspector_gui();
            m_list.DoLayoutList();
            if (m_list.index >= 0 && m_list.index < m_list.count) {
                var (ty, val) = m_parameter_editors[m_list.index];
                ty.on_inspector_gui();
                val.on_inspector_gui();
            }
        }

        public override void on_body_gui() {
            m_content_editor.on_body_gui();
        }

        public void build_parameters() {
            foreach (var pe in m_parameter_editors) {
                pe.Item2.build();
            }
        }

        private void init() {
            var value = get_value();
            var reset = false;
            if (value is DialogText dt) {
                target = dt;
            } else if (value is LongString s) {
                target = s;
                reset = true;
            }
            if (target == null) {
                target = new DialogText();
                reset = true;
            }
            if (reset) {
                set_value(target);
            }
            m_content_editor.attach(target, typeof(DialogText).GetField("_content"), m_graph, m_node, name, 0, m_show_in_body);

            m_list = new ReorderableList(target.parameters, typeof(DialogText.Parameter));
            m_list.drawHeaderCallback = rect => GUI.Label(rect, "Parameters");
            m_list.onAddCallback = on_add_item;
            m_list.onRemoveCallback = on_remove_item;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = on_reorder_item;
            m_list.onSelectCallback = on_select_item;

            foreach (var parameter in target.parameters) {
                m_parameter_editors.Add(create_parameter_editor(parameter));
            }
        }

        private (PropertyEnumEditor, ExpressionEditor) create_parameter_editor(DialogText.Parameter parameter) {
            var type_editor = new PropertyEnumEditor();
            type_editor.attach(parameter, typeof(DialogText.Parameter).GetField("type"), m_graph, m_node, "Parameter");

            var value_editor = new ExpressionEditor();
            value_editor.excepted_type = (CalcExpr.ValueType)parameter.type;
            if (parameter.type == ActionParameterType.Content) {
                value_editor.build_option = ExpressionEditorBase.BuildOption.Skip;
            }
            value_editor.attach(parameter, typeof(DialogText.Parameter).GetField("value"), m_graph, m_node, null);
            type_editor.on_changed += (p, b) => {
                value_editor.excepted_type = (CalcExpr.ValueType)parameter.type;
                if (parameter.type == ActionParameterType.Content) {
                    value_editor.build_option = ExpressionEditorBase.BuildOption.Skip;
                    value_editor.build();
                } else {
                    value_editor.build_option = ExpressionEditorBase.BuildOption.Auto;
                    value_editor.build();
                }
            };
            return (type_editor, value_editor);
        }

        private void on_reorder_item(ReorderableList list, int oldIndex, int newIndex) {
            m_graph.view.undo.record(new ReorderItem {
                editor = this,
                old_index = oldIndex,
                new_index = newIndex,
            });
            var ee = m_parameter_editors[oldIndex];
            m_parameter_editors.RemoveAt(oldIndex);
            m_parameter_editors.Insert(newIndex, ee);
            m_last_parameter_index = newIndex;
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            var (_, val) = m_parameter_editors[index];
            if (val.err_msg != null) {
                GUI.Label(rect, $"{index}: Error", GraphResources.styles.red_label);
            } else {
                GUI.Label(rect, $"{index}: {val.display(false)}");
            }
        }

        private void on_remove_item(ReorderableList list) {
            var parameter = target.parameters[m_list.index];
            var (ty, val) = m_parameter_editors[m_list.index];
            var cmd = new RemoveItem {
                editor = this,
                parameter = parameter,
                type = ty,
                value = val,
                index = m_list.index,
            };
            m_graph.view.undo.record(cmd);
            cmd.redo();
        }

        private void on_add_item(ReorderableList list) {
            var parameter = new DialogText.Parameter();
            var (ty, val) = create_parameter_editor(parameter);
            var cmd = new AddItem {
                editor = this,
                parameter = parameter,
                type = ty,
                value = val,
            };
            m_graph.view.undo.record(cmd);
            cmd.redo();
        }

        private void on_select_item(ReorderableList list) {
            if (m_node.inspector_enabled) {
                if (m_last_parameter_index >= 0 && m_last_parameter_index <= m_parameter_editors.Count) {
                    var (ty, val) = m_parameter_editors[m_last_parameter_index];
                    ty.on_inspector_disable();
                    val.on_inspector_disable();
                }
                m_last_parameter_index = list.index;
                if (m_last_parameter_index >= 0 && m_last_parameter_index <= m_parameter_editors.Count) {
                    var (ty, val) = m_parameter_editors[m_last_parameter_index];
                    ty.on_inspector_enable();
                    val.on_inspector_enable();
                }
            }
        }

        private void change_index(int old_index, int new_index) {
            var p = target.parameters[old_index];
            var (ty, val) = m_parameter_editors[old_index];
            if (m_node.inspector_enabled && old_index == m_list.index) {
                ty.on_inspector_disable();
                val.on_inspector_disable();
            }
            target.parameters.RemoveAt(old_index);
            m_parameter_editors.RemoveAt(old_index);
            target.parameters.Insert(new_index, p);
            m_parameter_editors.Insert(new_index, (ty, val));
            if (m_node.inspector_enabled && new_index == m_list.index) {
                ty.on_inspector_enable();
                val.on_inspector_enable();
            }
        }

        private PropertyLongStringEditor m_content_editor = new PropertyLongStringEditor();
        private ReorderableList m_list;
        private List<(PropertyEnumEditor, ExpressionEditor)> m_parameter_editors = new List<(PropertyEnumEditor, ExpressionEditor)>();
        private int m_last_parameter_index = -1;


        private class AddItem : GraphUndo.ICommand {
            public DialogTextEditor editor;
            public DialogText.Parameter parameter;
            public PropertyEnumEditor type;
            public ExpressionEditor value;

            int GraphUndo.ICommand.dirty_count => 1;

            public void undo() {
                var index = editor.m_parameter_editors.Count - 1;
                if (editor.m_list.index == index) {
                    editor.m_last_parameter_index = --editor.m_list.index;
                    if (editor.m_node.inspector_enabled) {
                        type.on_inspector_disable();
                        value.on_inspector_disable();
                    }
                }
                editor.target.parameters.RemoveAt(index);
                editor.m_parameter_editors.RemoveAt(index);
            }

            public void redo() {
                editor.target.parameters.Add(parameter);
                editor.m_parameter_editors.Add((type, value));
            }
        }

        private class RemoveItem : GraphUndo.ICommand {
            public DialogTextEditor editor;
            public DialogText.Parameter parameter;
            public PropertyEnumEditor type;
            public ExpressionEditor value;
            public int index;

            int GraphUndo.ICommand.dirty_count => 1;

            public void undo() {
                if (editor.m_list.index == index && editor.m_node.inspector_enabled) {
                    var ee = editor.m_parameter_editors[editor.m_list.index];
                    ee.Item1.on_inspector_disable();
                    ee.Item2.on_inspector_disable();
                    type.on_inspector_enable();
                    value.on_inspector_enable();
                }
                editor.target.parameters.Insert(index, parameter);
                editor.m_parameter_editors.Insert(index, (type, value));
            }

            public void redo() {
                if (editor.m_list.index == index) {
                    editor.m_last_parameter_index = --editor.m_list.index;
                    if (editor.m_node.inspector_enabled) {
                        type.on_inspector_disable();
                        value.on_inspector_disable();
                    }
                }
                editor.target.parameters.RemoveAt(index);
                editor.m_parameter_editors.RemoveAt(index);
            }
        }

        public class ReorderItem : GraphUndo.ICommand {
            public DialogTextEditor editor;
            public int old_index;
            public int new_index;

            int GraphUndo.ICommand.dirty_count => 1;

            public void undo() {
                editor.change_index(new_index, old_index);
            }

            public void redo() {
                editor.change_index(old_index, new_index);
            }
        }
    }
}