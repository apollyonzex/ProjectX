
using GraphNode.Editor;
using UnityEngine;
using UnityEditorInternal;
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(LogNode))]
    public class LogNodeEditor : ExpressionContextNodeWithInputEditor {

        public LogNode this_node => (LogNode)node;

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is LogNode other) {
                var this_node = this.this_node;
                other.parameters = new LogNode.Parameter[this_node.parameters.Length];
                for (int i = 0; i < other.parameters.Length; ++i) {
                    ref var p = ref this_node.parameters[i];
                    other.parameters[i] = new LogNode.Parameter {
                        name = new VariableName { name = p.name.name },
                        type = 0,
                    };
                }
            }
        }

        public override void on_view_init() {
            var node = this_node;
            if (node.parameters == null) {
                node.parameters = new LogNode.Parameter[0];
            } else {
                for (int i = 0; i < node.parameters.Length; ++i) {
                    var vne = new VariableNameEditor();
                    vne.attach(node.parameters[i].name, null, view.graph.editor, this, $"{{{i}}}", 0);
                    m_parameters.Add(vne);
                }
            }

            m_list = new ReorderableList(m_parameters, typeof(ExpressionEditor));
            //m_list.elementHeight = EditorGUIUtility.singleLineHeight;
            m_list.drawHeaderCallback = draw_header;
            m_list.drawElementCallback = draw_element;
            m_list.onAddCallback = on_add;
            m_list.onRemoveCallback = on_remove;
            m_list.onReorderCallbackWithDetails = on_reorder;


            base.on_view_init();
        }

        private void draw_header(Rect rect) {
            GUI.Label(rect, "Parameters");
        }

        private void draw_element(Rect rect, int index, bool isActive, bool isFocused) {
            rect.y += 2;
            m_parameters[index].on_list_element(rect);
        }

        private void on_add(ReorderableList _) {
            var vne = new VariableNameEditor();
            vne.attach(new VariableName(), null, view.graph.editor, this, $"{{{m_parameters.Count}}}", 0);
            ((IStackChange)vne).on_stack_change(this);

            var cmd = new AddItem { editor = this, vne = vne };
            view.graph.undo.record(cmd);
            cmd.redo();
        }

        private void on_remove(ReorderableList _) {
            var cmd = new RemoveItem { editor = this, index = m_list.index, vne = m_parameters[m_list.index] };
            view.graph.undo.record(cmd);
            cmd.redo();
        }

        private void on_reorder(ReorderableList _, int old_index, int new_index) {
            var cmd = new ReorderItem { editor = this, old_index = old_index, new_index = new_index };
            view.graph.undo.record(cmd);
            build_node_parameters();
        }

        public override void on_inspector_enable() {
            base.on_inspector_enable();
            foreach (var ee in m_parameters) {
                ee.on_inspector_enable();
            }
        }

        public override void on_inspector_disable() {
            base.on_inspector_disable();
            foreach (var ee in m_parameters) {
                ee.on_inspector_disable();
            }
        }

        protected override void on_inspector_gui_inner() {
            m_list.DoLayoutList();
        }

        protected override void on_stack_changed() {
            base.on_stack_changed();
            foreach (var p in m_parameters) {
                ((IStackChange)p).on_stack_change(this);
            }
        }


        public override void notify_variable_name_changed(VariableNameEditor vne) {
            var idx = m_parameters.IndexOf(vne);
            if (idx != -1) {
                ref var vn = ref this_node.parameters[idx];
                if (vne.target != null) {
                    vn.type = vne.target.type;
                } else {
                    vn.type = 0;
                }
            }
        }

        private void build_node_parameters() {
            var node = this_node;
            node.parameters = new LogNode.Parameter[m_parameters.Count];
            for (int i = 0; i < m_parameters.Count; ++i) {
                ref var p = ref node.parameters[i];
                var vne = m_parameters[i];
                p.name = vne.val;
                p.type = vne.target?.type ?? 0;
                vne._set_name($"{{{i}}}");
            }
        }

        List<VariableNameEditor> m_parameters = new List<VariableNameEditor>();

        ReorderableList m_list;

        class AddItem : GraphUndo.ICommand {
            public LogNodeEditor editor;
            public VariableNameEditor vne;

            public void undo() {
                var node = editor.this_node;
                var last = editor.m_parameters.Count - 1;
                editor.m_parameters.RemoveAt(last);
                var parameters = new LogNode.Parameter[last];
                for (int i = 0; i < last; ++i) {
                    parameters[i] = node.parameters[i];
                }
                node.parameters = parameters;
            }

            public void redo() {
                var node = editor.this_node;
                var parameters = new LogNode.Parameter[editor.m_parameters.Count + 1];
                for (int i = 0; i < node.parameters.Length; ++i) {
                    parameters[i] = node.parameters[i];
                }
                var p = new LogNode.Parameter { name = vne.val };
                if (vne.target != null) {
                    p.type = vne.target.type;
                } else {
                    p.type = 0;
                }
                parameters[editor.m_parameters.Count] = p;
                node.parameters = parameters;
                editor.m_parameters.Add(vne);
            }

            public int dirty_count => 1;
        }

        class RemoveItem : GraphUndo.ICommand {
            public LogNodeEditor editor;
            public VariableNameEditor vne;
            public int index;

            public void undo() {
                editor.m_parameters.Insert(index, vne);
                editor.build_node_parameters();
            }

            public void redo() {
                editor.m_parameters.RemoveAt(index);
                editor.build_node_parameters();
            }

            public int dirty_count => 1;
        }

        public class ReorderItem : GraphUndo.ICommand {
            public LogNodeEditor editor;
            public int old_index, new_index;

            public void undo() {
                var vne = editor.m_parameters[new_index];
                editor.m_parameters.RemoveAt(new_index);
                editor.m_parameters.Insert(old_index, vne);
                editor.build_node_parameters();
            }

            public void redo() {
                var vne = editor.m_parameters[old_index];
                editor.m_parameters.RemoveAt(old_index);
                editor.m_parameters.Insert(new_index, vne);
                editor.build_node_parameters();
            }

            public int dirty_count => 1;
        }

    }

}