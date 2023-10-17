
using GraphNode;

namespace DialogFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class JumpToPage : DialogNode {

        [Display("Target")][ShowInBody]
        public PageNodeBase target;


        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(IContext context) {
            if (target != null) {
                context.jump_to(target);
            } else {
                context.end();
            }
        }
    }
}