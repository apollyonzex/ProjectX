
using GraphNode;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class External : DialogNode {

        [NonProperty]
        public string name;

        [Input]
        [Display("")]
        public void invoke(IContext context) {
            var node = context.pop_externals();
            if (node != null) {
                var idx = node.binary_search_port(name);
                if (idx >= 0) {
                    var port = node.ports[idx];
                    if (port.value != null) {
                        port.value.Invoke(context);
                        return;
                    }
                }
            }
            context.end();
        }

    }
}