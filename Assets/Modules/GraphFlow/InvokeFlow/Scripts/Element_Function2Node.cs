
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_Function2Node : FunctionNode<(Element, Element)> {

        [Display("Target")]
        [SortedOrder(-2)]
        [ShowInBody]
        public ElementNode target;

        [Display("Param")]
        [SortedOrder(-1)]
        [ShowInBody]
        public ElementNode param;


        protected override (Element, Element) get_param(IContext context) {
            return (context.get_element(target), context.get_element(param));
        }
    }
}