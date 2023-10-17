

using GraphNode;


namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class StructDefNode : InvokeNode {

        [Display("Name")]
        [SortedOrder(1)]
        [ShowInBody(format = "'{0}'")]
        public string name;

        [Display("Members")]
        [SortedOrder(2)]
        [ShowInBody]
        public Variables members;


        public int[] stack_frame;
    }
}