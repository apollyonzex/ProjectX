
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class SetVariableNode : ExpressionContextNodeWithInput {

        [Output][Display("")]
        public Invoke output { get; set; }


        [Display("Target")][SortedOrder(1)]
        public VariableName target;


        [Display("Expression")][SortedOrder(2)]
        public Expression expression;

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (target == null || target.stack_pos < 0 || expression == null) {
                    Debug.LogError("SetVariable: invalid data");
                } else if (!expression.calc(context, context.context_type, out int ret)) {
                    Debug.LogError("SetVariable: expression failed!");
                } else {
                    context.set_stack_int(target.stack_pos, ret);
                }
                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }

    }

}