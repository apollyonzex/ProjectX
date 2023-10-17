
using GraphNode;


namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class OptionState_External : OptionStateNodeBase {

        [NonProperty]
        public string name;

        public override OptionState get_option_state(IContext context) {
            var node = context.peek_externals();
            if (node != null) {
                var idx = node.binary_search_option_state(name);
                if (idx >= 0) {
                    var port = node.option_states[idx];
                    if (port.value != null) {
                        return port.value.Invoke(context);
                    }
                }
            }
            return OptionState.Enable;
        }
    }
}