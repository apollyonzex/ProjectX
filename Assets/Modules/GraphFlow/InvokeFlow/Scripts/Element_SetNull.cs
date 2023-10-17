
using GraphNode;

namespace InvokeFlow {
    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_SetNull : InvokeNodeWithInput {

        [Display("Target")]
        [ShowInBody]
        public ElementNode target;

        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                var _t = context.get_element(target);
                _t.reset(null, null, null);

                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }
    }
}