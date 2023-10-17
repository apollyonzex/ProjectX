
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Collection_InsertNode : InvokeNodeWithInput {

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
                    if (cts.read_count == 0 && !cts.items.ContainsKey(ele.key)) {
                        int[] val = null;
                        var element_type = cts.prototype?.get_element_type();
                        if (ele.def == element_type) {
                            if (ele.elements != null) {
                                val = (int[])ele.elements.Clone();
                            }
                        } else if (element_type != null && element_type.stack_frame.Length != 0) {
                            val = (int[])element_type.stack_frame.Clone();
                        }
                        cts.items.Add(ele.key, val);
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