
using System.Collections.Generic;
using GraphNode;


namespace DialogFlow {

    [System.Serializable]
    public class OptionStatePort : NodeDelegatePort<OptionStateFunc> {
        public sealed override IO io => IO.Input;
        public sealed override bool can_mulit_connect => false;
        public override string name => option.content != null ? option.content.content : string.Empty;

        public PageOption option;
    }

    [System.Serializable]
    public class DialogActionPort : NodeDelegatePort<DialogAction> {
        public sealed override IO io => IO.Output;
        public sealed override bool can_mulit_connect => false;
        public override string name => string.Empty;
    }

    [System.Serializable]
    public class PageOption {
        public PageOption() {
            input = new OptionStatePort() { option = this };
        }

        public DialogText content;

        public OptionStatePort input { get; }
        public DialogActionPort output { get; } = new DialogActionPort();
    }


    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class PageNode : PageNodeBase {

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

        public List<PageOption> options;

        public override void show(IContext context) {
            content?.show_text(context);

            if (options != null && options.Count != 0) {
                int available_count = 0;
                for (int i = 0; i < options.Count; ++i) {
                    var option = options[i];
                    if (option.input.value != null) {
                        switch (option.input.value.Invoke(context)) {
                            case OptionState.Enable:
                                option.content.show_option(context, i, true);
                                ++available_count;
                                break;
                            case OptionState.Disable:
                                option.content.show_option(context, i, false);
                                break;
                        }
                    } else {
                        option.content.show_option(context, i, true);
                        ++available_count;
                    }
                    
                }

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
            var output = options[index].output;
            if (output.value != null) {
                output.value.Invoke(context);
            } else {
                context.end();
            }
        }


    }
}