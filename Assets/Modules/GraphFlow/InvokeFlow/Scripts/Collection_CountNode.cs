
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Collection_CountNode : InvokeNodeWithInput {

        [Display("Collection")]
        [ShowInBody]
        public CollectionNodeBase collection;


        [Output]
        [Display("")]
        [Parameter(VariableType.Integer, "_cnt")]
        public InvokeWithVariables output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (output != null) {
                    var cts = context.get_collection(collection);
                    context.push_stack(cts.items.Count);
                    var r = output.action(context);
                    context.pop_stack();
                    return r;
                }

                return InvokeState.None;
            }
        }
    }
}