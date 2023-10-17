

using DialogFlow;
using GraphNode;

namespace Worlds.Missions.Dialogs.Nodes {

    [System.Serializable]
    [Graph(typeof(MissionDialogGraph))]
    public class Waiting : DialogNode {

        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(DialogFlow.IContext context) {
            if (context is IMissionDialogContext ctx) {
                ctx.push_waiting(this);
            }
            if (action != null) {
                action.Invoke(context);
            } else {
                context.end();
            }
        }

        [Output]
        [Display("{Action}")]
        public DialogAction action { get; set; }

        [Output]
        [Display("{Success}")]
        public DialogAction success { get; set; }

        [Output]
        [Display("{Failed}")]
        public DialogAction failed { get; set; }

    }
}
