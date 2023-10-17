
using BehaviourFlow.Exports;
using GraphNode;
using UnityEngine;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ConditionNode : BTChildNode {

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;

        public override BTResult exec(BTExecutorBase executor) {
            if (!condition.calc(executor.context, executor.context.context_type, out bool result)) {
                Debug.LogError("Condition: expression failed");
                return BTResult.failed;
            }
            if (result) {
                return BTResult.success;
            }
            return BTResult.failed;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.Condition();
            node.cond = condition.export_as_expression(exporter);
            index = exporter.add_node(node);
            return true;
        }
    }
}