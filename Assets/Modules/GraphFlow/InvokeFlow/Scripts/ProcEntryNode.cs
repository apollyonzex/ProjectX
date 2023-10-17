
using GraphNode;
using System;

namespace InvokeFlow {

    [Serializable]
    [Graph(typeof(InvokeGraph))]
    public class ProcEntryNode : InvokeNode {

        [Display("Name")][ShowInBody]
        public string name;

        [Display("Arguments")]
        public Parameters arguments;

        [Display("Returns")]
        public Variables returns;

        [Output][Display("")]
        public Invoke output { get; set; }

        public int[] stack_frame_returns;


        public void init(IContext context) {
            if (stack_frame_returns != null) {
                context.push_stack(stack_frame_returns);
            }
        }

        public void fini(IContext context) {
            if (stack_frame_returns != null) {
                context.pop_stack(stack_frame_returns.Length);
            }
        }

        public void call(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (output != null) {
                    output.Invoke(context);
                }
            }
        }

    }

}
