
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    public class InvokeGraph : Graph {

        public override System.Type context_type => typeof(IContext);

        [Display("Variables")]
        public Variables variables;

        public int[] stack_frame;

        [NonProperty]
        public int obj_count = 0;

        public virtual void init_stack(IContext context) {
            if (stack_frame != null) {
                context.push_stack(stack_frame);
            }
            context.push_obj_stack(obj_count + 1);
        }

        public virtual void fini_stack(IContext context) {
            if (stack_frame != null) {
                context.pop_stack(stack_frame.Length);
            }
            context.pop_obj_stack(obj_count + 1);
        }

    }

}