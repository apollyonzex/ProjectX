
using GraphNode;
using System;

namespace InvokeFlow {

    [Serializable]
    [Graph(typeof(InvokeGraph))]
    public class ContinueNode : InvokeNodeWithInput {
        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                return InvokeState.Continue;
            }
        }
    }



}