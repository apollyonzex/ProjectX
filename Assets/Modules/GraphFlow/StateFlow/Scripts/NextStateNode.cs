
using GraphNode;
using InvokeFlow;

namespace StateFlow {

    [System.Serializable]
    [Graph(typeof(StateGraph))]
    public class NextStateNode : InvokeNodeWithInput {

        [Display("State")]
        [ShowInBody]
        public StateNodeBase state;

        public override InvokeState invoke(InvokeFlow.IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (context is IContext ctx) {
                    var exec = ctx.executor;
                    if (exec != null) {
                        exec.set_next_state(ctx, state);
                    }
                }
                return InvokeState.Return;
            }
        }

    }
}