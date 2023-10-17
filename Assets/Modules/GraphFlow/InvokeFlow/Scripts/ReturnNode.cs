
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class ReturnNode : InvokeNodeWithInput {

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                return InvokeState.Return;
            }
        }
    }

}