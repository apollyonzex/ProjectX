
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class ForEachNode : InvokeNodeWithInput {

        [Display("Element")]
        [SortedOrder(1)]
        [ShowInBody]
        public ElementNode element;

        [Display("Collection")]
        [SortedOrder(2)]
        [ShowInBody(format = "in {0}")]
        public CollectionNode collection;

        [Output]
        [Display("Loop")]
        public Invoke loop { get; set; }

        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (collection != null && loop != null) {
                    var cts = context.get_collection(collection);
                    var element_type = cts.prototype?.get_element_type();
                    var ele = context.get_element(element);
                    ++cts.read_count;
                    foreach (var kvp in cts.items) {
                        ele.reset(kvp.Key, element_type, kvp.Value);
                        switch (loop(context)) {
                            case InvokeState.Return:
                                --cts.read_count;
                                ele.reset(null, null, null);
                                return InvokeState.Return;
                            case InvokeState.Break:
                                goto done;
                        }
                    }
                done:
                    --cts.read_count;
                    ele.reset(null, null, null);
                }

                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }
    }
}