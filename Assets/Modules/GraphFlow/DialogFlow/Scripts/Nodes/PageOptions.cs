
using GraphNode;
using System.Collections.Generic;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class PageOptions : PageOptionsBase {

        public List<PageOption> options;

        [Output]
        [Display("")]
        public PageOptionsBase tail { get; set; }

        public override int show_options(IContext context, ref int index) {
            int available_count = 0;
            if (options != null) {
                foreach (var option in options) {
                    if (option.input.value != null) {
                        switch (option.input.value.Invoke(context)) {
                            case OptionState.Enable:
                                option.content.show_option(context, index, true);
                                ++available_count;
                                break;
                            case OptionState.Disable:
                                option.content.show_option(context, index, false);
                                break;
                        }
                    } else {
                        option.content.show_option(context, index, true);
                        ++available_count;
                    }
                    ++index;
                }
            }
            if (tail != null) {
                available_count += tail.show_options(context, ref index);
            }
            return available_count;
        }

        public override void skip_options(IContext context, ref int index) {
            if (options != null) {
                index += options.Count;
            }
            tail?.skip_options(context, ref index);
        }

        public override bool do_option(IContext context, ref int index) {
            if (options != null) {
                if (index < options.Count) {
                    var output = options[index].output;
                    if (output.value != null) {
                        output.value.Invoke(context);
                    } else {
                        context.end();
                    }
                    return true;
                }
                index -= options.Count;                
            }
            if (tail != null) {
                return tail.do_option(context, ref index);
            }
            return false;
        }
    }
}