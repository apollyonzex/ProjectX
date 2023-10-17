
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_EqualsNode : InvokeNodeWithInput {

        [Display("Target")]
        [SortedOrder(1)]
        [ShowInBody]
        public ElementNode target;

        [Display("Source")]
        [SortedOrder(2)]
        [ShowInBody(format = " == {0}")]
        public ElementNode source;

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
                bool ok;
                if (target == source) {
                    ok = true;
                } else {
                    var _t = context.get_element(target);
                    var _s = context.get_element(source);
                    ok = _t.key == _s.key;
                }
                if (ok) {
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

                return InvokeState.None;
            }
        }
    }
}