
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Collection_ContainsNode : InvokeNodeWithInput {

        [Display("Collection")]
        [SortedOrder(1)]
        [ShowInBody]
        public CollectionNodeBase collection;

        [Display("Element")]
        [SortedOrder(2)]
        [ShowInBody]
        public ElementNode element;

        [Output]
        [Display("{True}")]
        public Invoke true_part { get; set; }

        [Output]
        [Display("{False}")]
        public Invoke false_part { get; set; }

        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                var ele = context.get_element(element);
                if (ele.key != null && context.get_collection(collection).items.ContainsKey(ele.key)) {
                    if (true_part != null) {
                        var r = true_part.Invoke(context);
                        if (r != InvokeState.None) {
                            return r;
                        }
                    }
                } else if (false_part != null) {
                    var r = false_part.Invoke(context);
                    if (r != InvokeState.None) {
                        return r;
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