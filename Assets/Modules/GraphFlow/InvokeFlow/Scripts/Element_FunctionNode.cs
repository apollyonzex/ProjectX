
using GraphNode;

namespace InvokeFlow {
    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_FunctionNode : FunctionNode<Element> {

        [Display("Target")]
        [SortedOrder(-1)]
        [ShowInBody]
        public ElementNode target;

        protected override Element get_param(IContext context) {
            return context.get_element(target);
        }
    }
}