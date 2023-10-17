
using System.Collections.Generic;
using GraphNode;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class PageContent : PageNodeBase {

        [Output]
        [Display("")]
        public DialogAction next { get; set; }

        [Display("Timeout")]
        [SortedOrder(2)]
        public float timeout = 0;


        [Display("Content")]
        [SortedOrder(3)]
        [ShowInBody]
        public DialogText content;


        [Display("Options")]
        [Output]
        public PageOptionsBase options { get; set; }

        public override void show(IContext context) {
            if (content.parameters.Count == 0) {
                context.show_text(content.content ?? string.Empty);
            } else {
                var args = new object[content.parameters.Count];
                for (int i = 0; i < args.Length; ++i) {
                    var p = content.parameters[i];
                    switch (p.type) {
                        case ActionParameterType.Content:
                            args[i] = context.translate_argument(p.value.content);
                            break;
                        case ActionParameterType.Integer: {
                            if (p.value.calc(context, context.context_type, out int val)) {
                                args[i] = val;
                            }
                            break;
                        }
                        case ActionParameterType.Floating: {
                            if (p.value.calc(context, context.context_type, out float val)) {
                                args[i] = val;
                            }
                            break;
                        }
                        case ActionParameterType.Boolean: {
                            if (p.value.calc(context, context.context_type, out bool val)) {
                                args[i] = val;
                            }
                            break;
                        }
                    }
                }
                context.show_text(content.content ?? string.Empty, args);
            }

            if (options != null) {
                int index = 0;
                int available_count = options.show_options(context, ref index);
                if (next != null || available_count == 0) {
                    context.show_continue(timeout);
                }
            } else {
                context.show_continue(timeout);
            }
        }

        public override void do_continue(IContext context) {
            if (next != null) {
                next.Invoke(context);
            } else {
                context.end();
            }
        }

        public override void do_option(IContext context, int index) {
            options.do_option(context, ref index);
        }
    }
}