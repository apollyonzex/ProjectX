
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {

    [System.Serializable]
    public class ExpressionContextNode : InvokeNode {

        [Display("Referenced Elements")]
        public ElementNode[] referenced_elements;
    }

    [System.Serializable]
    public class ExpressionContextNodeWithInput : ExpressionContextNode {

        [Input][Display("")]
        public override InvokeState invoke(IContext context) {
            return InvokeState.None;
        }
    }

}