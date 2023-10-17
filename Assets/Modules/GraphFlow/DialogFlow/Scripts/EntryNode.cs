
using GraphNode;

namespace DialogFlow {
    [System.Serializable]
    [Unique]
    public class EntryNode : DialogNode {

        [Output]
        [Display("")]
        public DialogAction action { get; set; }


        public void invoke(IContext context) {
            if (action != null) {
                action(context);
            } else {
                context.end();
            }
        }

    }
}