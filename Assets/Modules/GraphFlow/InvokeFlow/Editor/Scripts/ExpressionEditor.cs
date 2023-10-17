using GraphNode.Editor;
using CalcExpr;
using System.Collections.Generic;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(Expression))]
    public class ExpressionEditor : ExpressionEditor<Expression>, ExpressionContextNodeEditor.IStackChange {

        protected override bool get_external(string name, out ValueType ty, out IExpressionExternal external) {
            if (m_node is ExpressionContextNodeEditor ecn) {
                if (ecn.variable_dict.TryGetValue(name, out var info)) {
                    ty = (ValueType)info.Item1.type;
                    external = new ContextStackLocal { stack_pos = info.Item2 };
                    return true;
                }

                int pos = name.IndexOf('.');
                if (pos >= 0) {
                    if (ecn.try_get_element_index(name.Substring(0, pos), out var element_index)) {
                        if (try_get_element_field(name.Substring(pos + 1), out var field)) {
                            external = new ContextElementFunction {
                                node = ecn.node as ExpressionContextNode,
                                func = field.m,
                                index = element_index,
                                ty = field.vt,
                            };
                            ty = field.vt;
                            return true;
                        }
                    }
                }
            }
            return base.get_external(name, out ty, out external);
        }

        protected override void notify_changed(bool by_user) {
            base.notify_changed(by_user);
            if (m_node is ExpressionContextNodeEditor nec) {
                nec.notify_expression_changed(this);
            }
        }

        void ExpressionContextNodeEditor.IStackChange.on_stack_change(ExpressionContextNodeEditor node_editor) {
            build();
        }

        static readonly Dictionary<System.Type, Dictionary<string, (GraphNode.ActionMethod<Element>, ValueType)>> s_element_fields = new Dictionary<System.Type, Dictionary<string, (GraphNode.ActionMethod<Element>, ValueType)>>();

        public bool try_get_element_field(string name, out (GraphNode.ActionMethod<Element> m, ValueType vt) field) {
            var ctx_ty = m_graph.graph.context_type;
            if (!s_element_fields.TryGetValue(ctx_ty, out var dict)) {
                dict = new Dictionary<string, (GraphNode.ActionMethod<Element>, ValueType)>();
                foreach (var (n, m) in GraphNode.ActionMethod<Element>.collect_methods(ctx_ty, new GraphNode.TypeTuple<int, float, bool>())) {
                    if (m.parameter_types.Length == 0) {
                        if (m.method_info.ReturnType == typeof(int)) {
                            dict.Add(n, (m, ValueType.Integer));
                        } else if (m.method_info.ReturnType == typeof(float)) {
                            dict.Add(n, (m, ValueType.Floating));
                        } else if (m.method_info.ReturnType == typeof(bool)) {
                            dict.Add(n, (m, ValueType.Boolean));
                        }
                    }
                }
                s_element_fields.Add(ctx_ty, dict);
            }
            return dict.TryGetValue(name, out field);
        }
    }

}