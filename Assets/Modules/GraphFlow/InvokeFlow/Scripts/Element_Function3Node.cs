
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_Function3Node : FunctionNode<(Element, Element, Element)> {

        [Display("Target")]
        [SortedOrder(-3)]
        [ShowInBody]
        public ElementNode target;

        [Display("Param1")]
        [SortedOrder(-2)]
        [ShowInBody]
        public ElementNode param1;

        [Display("Param2")]
        [SortedOrder(-1)]
        [ShowInBody]
        public ElementNode param2;


        protected override (Element, Element, Element) get_param(IContext context) {
            return (context.get_element(target), context.get_element(param1), context.get_element(param2));
        }
    }
}