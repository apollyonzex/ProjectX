
using GraphNode;
using System;
using System.Reflection;
using UnityEngine;

namespace InvokeFlow {

    [Serializable]
    public class ContextMethodNode : ExpressionContextNodeWithInput {

        [Output][Display("")]
        public InvokeWithVariables output { get; set; }

        public virtual Type target_type => typeof(IContext);
        public virtual object get_target(IContext context) { return context; }

        [Serializable]
        public struct Parameter {
            public int index;
            public VariableType type;
            public Expression expression;
        }

        [Serializable]
        public struct Return {
            public int index;
            public VariableType type;
        }

        public Parameter[] parameters;
        public Return[] returns;

        public MethodInfo method_info;

        public int parameter_count { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (method_info != null) {
                    var target = get_target(context);
                    var ps = new object[parameter_count];
                    foreach (var p in parameters) {
                        switch (p.type) {
                            case VariableType.Integer: {
                                if (p.expression.calc(context, context.context_type, out int ret)) {
                                    ps[p.index] = ret;
                                }
                                break;
                            }
                            case VariableType.Floating: {
                                if (p.expression.calc(context, context.context_type, out float ret)) {
                                    ps[p.index] = ret;
                                }
                                break;
                            }
                            case VariableType.Boolean: {
                                if (p.expression.calc(context, context.context_type, out bool ret)) {
                                    ps[p.index] = ret;
                                }
                                break;
                            }
                        }
                    }
                    try {
                        method_info.Invoke(target, ps);
                    } catch (Exception e) {
                        Debug.LogError(e);
                    }
                    if (output != null) {
                        if (returns.Length == 0) {
                            return output.action(context);
                        }
                        foreach (var r in returns) {
                            switch (r.type) {
                                case VariableType.Integer:
                                    context.push_stack((int)ps[r.index]);
                                    break;
                                case VariableType.Floating:
                                    context.push_stack((float)ps[r.index]);
                                    break;
                                case VariableType.Boolean:
                                    context.push_stack((bool)ps[r.index]);
                                    break;
                                default:
                                    context.push_stack();
                                    break;
                            }
                        }
                        var ret = output.action(context);
                        context.pop_stack(returns.Length);
                        return ret;
                    }
                } else if (output != null) {
                    return output.action(context);
                }
                return InvokeState.None;
            }
        }
    }

}