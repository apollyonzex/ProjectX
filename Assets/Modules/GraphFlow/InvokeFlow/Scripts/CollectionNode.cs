
using GraphNode;

namespace InvokeFlow {
    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class CollectionNode : CollectionNodeBase {

        [Display("Element")]
        [ShowInBody(format = "<{0}>")]
        [SortedOrder(2)]
        public StructDefNode element_type;


        public override StructDefNode get_element_type() {
            return element_type;
        }
    }
}