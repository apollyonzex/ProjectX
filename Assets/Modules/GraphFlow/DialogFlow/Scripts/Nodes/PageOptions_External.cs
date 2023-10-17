
using GraphNode;


namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class PageOptions_External : PageOptionsBase {

        [NonProperty]
        public string name;

        [Output]
        [Display("")]
        public PageOptionsBase tail { get; set; }

        public override int show_options(IContext context, ref int index) {
            int available_count = 0;
            var node = context.peek_externals();
            if (node != null) {
                var idx = node.binary_search_options(name);
                if (idx >= 0) {
                    var value = node.options[idx].value;
                    if (value != null) {
                        available_count = value.show_options(context, ref index);
                    }
                }
            }
            if (tail != null) {
                available_count += tail.show_options(context, ref index);
            }
            return available_count;
        }

        public override void skip_options(IContext context, ref int index) {
            var node = context.peek_externals();
            if (node != null) {
                var idx = node.binary_search_options(name);
                if (idx >= 0) {
                    node.options[idx].value?.skip_options(context, ref index);
                }
            }
            tail?.skip_options(context, ref index);
        }

        public override bool do_option(IContext context, ref int index) {
            var node = context.pop_externals();
            if (node != null) {
                var idx = node.binary_search_options(name);
                if (idx >= 0) {
                    var value = node.options[idx].value;
                    if (value != null) {
                        if (value.do_option(context, ref index)) {
                            return true;
                        }
                    }
                }
                context.push_externals(node);
            }
            if (tail != null) {
                return tail.do_option(context, ref index);
            }
            return false;
        }
    }
}