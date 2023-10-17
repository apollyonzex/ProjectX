
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Collection_ClearNode : InvokeNodeWithInput {

        [Display("Collection")]
        [ShowInBody]
        public CollectionNodeBase collection;


        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                var cts = context.get_collection(collection);
                if (cts.read_count == 0) {
                    cts.items.Clear();
                }


                if (output != null) {
                    return output.Invoke(context);
                }

                return InvokeState.None;
            }
        }
    }
}