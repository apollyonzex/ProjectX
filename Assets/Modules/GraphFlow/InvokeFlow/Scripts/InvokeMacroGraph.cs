
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {

    [System.Serializable]
    public class InvokeMacroGraph : InvokeGraph {

        [Display("Returns")]
        public Variables outputs;

        [Display("Arguments")]
        public Parameters inputs;

        public EntryNode entry;

        public int argument_count { get; set; }
        public int return_count { get; set; }
        public int variable_count { get; set; }

        public List<ExternalElementNode> external_elements;
        public List<ExternalCollectonNode> external_collections;

        public override void fini_stack(IContext context) {
            context.pop_stack(argument_count + variable_count);
            context.pop_obj_stack(obj_count + 1);
        }

    }

}