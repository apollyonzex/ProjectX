
using GraphNode;

namespace InvokeFlow {
    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Collection_FunctionNode : FunctionNode<Collection> {

        [Display("Collection")]
        [SortedOrder(-1)]
        [ShowInBody]
        public CollectionNodeBase collection;

        protected override Collection get_param(IContext context) {
            return context.get_collection(collection);
        }
    }
}