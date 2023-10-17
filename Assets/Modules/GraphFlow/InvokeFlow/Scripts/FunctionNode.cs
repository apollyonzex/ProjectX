
using GraphNode;


namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class FunctionNode : ExpressionContextNodeWithInput {

        [Display("Method")]
        [ShowInBody]
        public Function method;

        [Output]
        [Display("")]
        public InvokeWithVariables output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (method != null && method.method != null) {
                    if (method.invoke(context.context_type, context, out _, out var outputs)) {
                        if (output != null) {
                            if (outputs.Count == 0) {
                                return output.action(context);
                            }
                            int index = 0;
                            int count = 0;
                            foreach (var r in outputs) {
                                if (!string.IsNullOrEmpty(method.out_names[index])) {
                                    context.push_stack((int)r);
                                    ++count;
                                }
                                ++index;
                            }
                            var ret = output.action(context);
                            context.pop_stack(count);
                            return ret;
                        }
                    } else if (output != null) {
                        if (method.method.output_indices.Length == 0) {
                            return output.action(context);
                        }
                        int count = 0;
                        for (int i = 0; i < method.method.output_indices.Length; ++i) {
                            if (!string.IsNullOrEmpty(method.out_names[i])) {
                                context.push_stack();
                                ++count;
                            }
                        }
                        var ret = output.action(context);
                        context.pop_stack(count);
                        return ret;
                    }
                } else if (output != null) {
                    return output.action(context);
                }
                return InvokeState.None;
            }
        }
    }

    [System.Serializable]
    public class FunctionNode<T> : ExpressionContextNodeWithInput {

        [Display("Method")]
        [ShowInBody]
        public Function<T> method;

        [Output]
        [Display("")]
        public InvokeWithVariables output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (method != null && method.method != null) {
                    if (method.invoke(context.context_type, context, get_param(context), out _, out var outputs)) {
                        if (output != null) {
                            if (outputs.Count == 0) {
                                return output.action(context);
                            }
                            int output_count = 0;
                            for (int i = 0; i < outputs.Count; ++i) {
                                if (string.IsNullOrEmpty(method.out_names[i])) {
                                    continue;
                                }
                                context.push_stack((int)outputs[i]);
                                ++output_count;
                            }
                            var ret = output.action(context);
                            context.pop_stack(output_count);
                            return ret;
                        }
                    } else if (output != null) {
                        if (method.method.output_indices.Length == 0) {
                            return output.action(context);
                        }
                        int output_count = 0;
                        foreach (var out_name in method.out_names) {
                            if (string.IsNullOrEmpty(out_name)) {
                                continue;
                            }
                            context.push_stack();
                            ++output_count;
                        }
                        var ret = output.action(context);
                        context.pop_stack(output_count);
                        return ret;
                    }
                } else if (output != null) {
                    return output.action(context);
                }
                return InvokeState.None;
            }
        }

        protected virtual T get_param(IContext context) {
            return default;
        }
    }
}