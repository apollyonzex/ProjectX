
using GraphNode;

namespace InvokeFlow {
    [System.Serializable]
    [Graph(typeof(InvokeGraph))]    
    public class ElementNode : InvokeNode {

        [Display("Name")]
        [ShowInBody(format = "'{0}'")]
        public string name;

        [NonProperty]
        public int stack_pos;
    }
}