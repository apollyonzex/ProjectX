
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Unique]
    public class EntryNode : InvokeNode {

        [Output][Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }

    }

}