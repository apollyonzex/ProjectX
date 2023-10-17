
using GraphNode;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class Action : DialogNode {

        [Display("Method")][ShowInBody]
        public ContextAction method;

        [Output]
        [Display("")]
        public DialogAction output { get; set; }

        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(IContext context) {
            method?.invoke(context.context_type, context, out _);
            if (output != null) {
                output.Invoke(context);
            } else {
                context.end();
            }
        }
    }
}