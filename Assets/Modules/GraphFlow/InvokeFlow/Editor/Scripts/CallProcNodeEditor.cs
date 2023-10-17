
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(CallProcNode))]
    public class CallProcNodeEditor : ExpressionContextNodeWithInputEditor {

        public new CallProcNode node => (CallProcNode)m_node;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("proc_entry", out var pe)) {
                m_proc_entry_editor = pe as PropertyProcEntryNodeEditor;
                m_proc_entry_editor.on_proc_entry_stack_changed += on_proc_entry_stack_changed;
            }
            var this_node = this.node;
            if (this_node.parameters != null) {
                foreach (var e in this_node.parameters) {
                    var ee = new ExpressionEditor();
                    ee.attach(e, null, graph, this, string.Empty, 0);
                    m_param_editors.Add(ee);
                }
            }
        }

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is CallProcNode other) {
                var self = this.node;
                if (self.parameters != null) {
                    other.parameters = new Expression[self.parameters.Length];
                    for (int i = 0; i < self.parameters.Length; ++i) {
                        other.parameters[i] = self.parameters[i].clone() as Expression;
                    }
                }
            }
        }

        public override void on_view_init() {
            m_port_view = view.find_output_port(new NodePropertyPort_InvokeWithVariables(NodePort.IO.Output, typeof(CallProcNode).GetProperty("output"), false));
        }

        public override void on_body_gui() {
            base.on_body_gui();
            if (m_proc_entry_editor.value_editor != null) {
                bool error = false;
                foreach (var e in m_param_editors) {
                    if (e.err_msg != null) {
                        error = true;
                        break;
                    }
                }
                if (error) {
                    GUILayout.Label("Error", GraphResources.styles.red_label);
                }
            }
        }

        public override void on_inspector_enable() {
            base.on_inspector_enable();
            foreach (var e in m_param_editors) {
                e.on_inspector_enable();
            }
        }

        public override void on_inspector_disable() {
            base.on_inspector_disable();
            foreach (var e in m_param_editors) {
                e.on_inspector_disable();
            }
        }

        protected override void on_inspector_gui_inner() {
            foreach (var e in m_param_editors) {
                e.on_inspector_gui();
            }
        }

        protected override IEnumerator<Variable> enumerate_port_variables(InvokeWithVariables.IPort iv_port) {
            if (m_proc_entry_editor.value_editor != null && m_port_view.port == iv_port) {
                return m_proc_entry_editor.value_editor.node.returns.enumerate_valid_variables();
            }
            return null;
        }

        private void on_proc_entry_stack_changed(bool by_user) {
            if (!view.graph.undo.operating) {
                build_data(by_user);
            }
            build_port_stack_frame(m_port_view, true);
        }

        protected override void on_stack_changed() {
            base.on_stack_changed();
            foreach (var e in m_param_editors) {
                e.build();
            }
        }

        private void build_data(bool by_user) {
            var node = this.node;
            List<Variable> arguments = null;
            int count = 0;
            if (m_proc_entry_editor.value_editor != null) {
                arguments = new List<Variable>();
                var iter = m_proc_entry_editor.value_editor.node.arguments.enumerate_valid_variables();
                while (iter.MoveNext()) {
                    arguments.Add(iter.Current);
                }
                count = arguments.Count;
            }
            var old_count = m_param_editors.Count;
            if (count > old_count) {
                node.parameters = new Expression[count];
                for (int i = 0; i < old_count; ++i) {
                    node.parameters[i] = m_param_editors[i].target;
                }
                for (int i = old_count; i < count; ++i) {
                    var ee = new ExpressionEditor();
                    var item = arguments[i];
                    ee.excepted_type = (CalcExpr.ValueType)item.type;
                    ee.attach(new Expression(), null, view.graph.editor, this, string.Empty, 0);
                    if (inspector_enabled) {
                        ee.on_inspector_enable();
                    }
                    m_param_editors.Add(ee);
                    node.parameters[i] = ee.target;
                }
                if (by_user) {
                    var list = new ExpressionEditor[count - old_count];
                    for (int i = 0; i < list.Length; ++i) {
                        list[i] = m_param_editors[old_count + i];
                    }
                    view.graph.undo.record(new CreateParams {
                        editor = this,
                        ees = list,
                    });
                }
            } else if (count < old_count) {
                if (by_user) {
                    var list = new ExpressionEditor[old_count - count];
                    for (int i = 0; i < list.Length; ++i) {
                        list[i] = m_param_editors[count + i];
                    }
                    view.graph.undo.record(new RemoveParams {
                        editor = this,
                        ees = list,
                    });
                } 
                for (int i = old_count - 1; i >= count; --i) {
                    var ee = m_param_editors[i];
                    if (inspector_enabled) {
                        ee.on_inspector_disable();
                    }
                    m_param_editors.RemoveAt(i);
                }
                
                if (count == 0) {
                    node.parameters = null;
                } else {
                    node.parameters = new Expression[count];
                    for (int i = 0; i < count; ++i) {
                        node.parameters[i] = m_param_editors[i].target;
                    }
                }
            }

            for (int i = 0; i < count; ++i) {
                var ee = m_param_editors[i];
                var item = arguments[i];
                ee._set_name($"{item.name}: {item.type.to_string()}");
                var ty = (CalcExpr.ValueType)item.type;
                if (ee.excepted_type != ty) {
                    ee.excepted_type = ty;
                    if (!stack_changed) {
                        ee.build();
                    }
                } 
            }
        }

        private OutputPortView m_port_view;
        private PropertyProcEntryNodeEditor m_proc_entry_editor;
        private List<ExpressionEditor> m_param_editors = new List<ExpressionEditor>();

        #region undo commands
        class CreateParams : GraphUndo.ICommand {
            public CallProcNodeEditor editor;
            public ExpressionEditor[] ees;

            public int dirty_count => 1;

            public virtual void redo() {
                foreach (var ee in ees) {
                    editor.m_param_editors.Add(ee);
                }
                ref var parameters = ref editor.node.parameters;
                parameters = new Expression[editor.m_param_editors.Count];
                for (int i = 0; i < parameters.Length; ++i) {
                    parameters[i] = editor.m_param_editors[i].target;
                }
            }

            public virtual void undo() {
                var count = editor.m_param_editors.Count;
                var old_count = count - ees.Length;
                for (int i = count - 1; i >= old_count; --i) {
                    editor.m_param_editors.RemoveAt(i);
                }
                ref var parameters = ref editor.node.parameters;
                if (old_count == 0) {
                    parameters = null;
                } else {
                    parameters = new Expression[old_count];
                    for (int i = 0; i < old_count; ++i) {
                        parameters[i] = editor.m_param_editors[i].target;
                    }
                }
            }
        }

        class RemoveParams : CreateParams {

            public override void redo() {
                base.undo();
            }

            public override void undo() {
                base.redo();
            }
        }
        #endregion
    }
}