
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class WhileNode : ExpressionContextNodeWithInput {

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;

        [Output]
        [Display("Loop")]
        public Invoke loop { get; set; }

        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (loop != null) {
                l_loop:
                    condition.calc(context, context.context_type, out bool cond);
                    if (cond) {
                        switch (loop(context)) {
                            case InvokeState.Return:
                                return InvokeState.Return;
                            case InvokeState.Break:
                                break;
                            default:
                                goto l_loop;
                        }
                    }
                }
                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }
    }
}