
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class WeightNode : BTChildNode {

        [Display("")]
        [Output]
        public BTChildNode child { get; set; }


        [Display("Weight")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public Expression weight;

        public int compute_weight(IContext context) {
            if (!weight.calc(context, context.context_type, out int ret)) {
                Debug.LogError("Weight: expression failed");
            }
            return ret;
        }

        public override BTResult exec(BTExecutorBase executor) {
            if (child == null) {
                return BTResult.success;
            }
            return BTResult.child(child);
        }
    }
}