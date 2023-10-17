
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_CheckNode : InvokeNodeWithInput {

        [Display("Target")]
        [ShowInBody]
        public ElementNode target;

        [Output]
        [Display("{Value}")]
        public Invoke true_part { get; set; }

        [Output]
        [Display("{Null}")]
        public Invoke false_part { get; set; }

        [Output]
        [Display("")]
        public Invoke output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                var _t = context.get_element(target);
                if (_t.key != null) {
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