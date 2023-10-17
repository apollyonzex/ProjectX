
using GraphNode;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class OptionState_Value : OptionStateNodeBase {


        [Display("State")][ShowInBody()]
        public OptionState state;


        public override OptionState get_option_state(IContext context) {
            return state;
        }
    }

}