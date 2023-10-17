
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Collection_RemoveNode : InvokeNodeWithInput {

        [Display("Collection")]
        [SortedOrder(1)]
        [ShowInBody]
        public CollectionNodeBase collection;

        [Display("Element")]
        [SortedOrder(2)]
        [ShowInBody]
        public ElementNode element;


        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                var ele = context.get_element(element);
                if (ele.key != null) {
                    var cts = context.get_collection(collection);
                    if (cts.read_count == 0) {
                        cts.items.Remove(ele.key);
                    }
                }
                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }
    }
}