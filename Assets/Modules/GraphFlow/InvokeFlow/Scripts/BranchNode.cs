
using GraphNode;
using CalcExpr;
using UnityEngine;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class BranchNode : ExpressionContextNodeWithInput {

        [Output][Display("{True}")]
        public Invoke true_part { get; set; }

        [Output][Display("{False}")]
        public Invoke false_part { get; set; }

        [Output][Display("")]
        public Invoke output { get; set; }

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(ValueType.Boolean)]
        public Expression condition;

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            { 
                if (condition == null) {
                    Debug.LogError("Branch: invalid expression");
                } else if (!condition.calc(context, context.context_type, out bool ret)) {
                    Debug.LogError("Branch: expression failed!");
                } else {
                    Invoke part = ret ? true_part : false_part;
                    if (part != null) {
                        var s = part.Invoke(context);
                        if (s != InvokeState.None) {
                            return s;
                        }
                    }
                }
                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }

    }

}