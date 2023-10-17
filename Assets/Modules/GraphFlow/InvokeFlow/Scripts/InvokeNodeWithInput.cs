
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    public class InvokeNodeWithInput : InvokeNode {


        [Input]
        [Display("")]
        public override InvokeState invoke(IContext context) { return InvokeState.None; }
    }

}