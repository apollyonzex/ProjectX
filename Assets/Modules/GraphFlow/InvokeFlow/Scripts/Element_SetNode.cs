
using GraphNode;


namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class Element_SetNode : InvokeNodeWithInput {

        [Display("Target")]
        [SortedOrder(1)]
        [ShowInBody]
        public ElementNode target;

        [Display("Source")]
        [SortedOrder(2)]
        [ShowInBody(format = " = {0}")]
        public ElementNode source;


        [Output]
        [Display("")]
        public Invoke output { get; set; }


        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (target != source) {
                    var _t = context.get_element(target);
                    var _s = context.get_element(source);
                    _t.reset(_s.key, _s.def, _s.elements);
                }

                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }
    }
}