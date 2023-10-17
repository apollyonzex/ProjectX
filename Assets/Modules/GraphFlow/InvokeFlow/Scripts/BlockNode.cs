
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class BlockNode : ExpressionContextNodeWithInput {

        [Output][Display("")]
        public Invoke output { get; set; }

        [Display("Variables")]
        public Variables variables;

        public int[] stack_frame;

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (output != null) {
                    if (stack_frame != null) {
                        context.push_stack(stack_frame);
                        var ret = output.Invoke(context);
                        context.pop_stack(stack_frame.Length);
                        return ret;
                    }
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }
    }

}