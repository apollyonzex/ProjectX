
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class CollectionNodeBase : InvokeNode {

        [Display("Name")]
        [ShowInBody(format = "'{0}'")]
        [SortedOrder(1)]
        public string name;

        [NonProperty]
        public int stack_pos;
        
        public virtual StructDefNode get_element_type() {
            return null;
        }
    }
}