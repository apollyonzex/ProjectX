
using GraphNode;


namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_FunctionCNode : FunctionNode<(Element, Collection)> {

        [Display("Target")]
        [SortedOrder(-2)]
        [ShowInBody]
        public ElementNode target;

        [Display("Collection")]
        [SortedOrder(-1)]
        [ShowInBody]
        public CollectionNodeBase param;

        protected override (Element, Collection) get_param(IContext context) {
            return (context.get_element(target), context.get_collection(param));
        }
    }
}