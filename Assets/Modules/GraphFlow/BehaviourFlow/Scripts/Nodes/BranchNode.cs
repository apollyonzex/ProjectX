
using BehaviourFlow.Exports;
using GraphNode;
using UnityEngine;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class BranchNode : BTChildNode {

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;

        [Display("{ true }")]
        [Output]
        [SortedOrder(2)]
        public BTChildNode true_part { get; set; }

        [Display("{ false }")]
        [Output]
        [SortedOrder(3)]
        public BTChildNode false_part { get; set; }

        public override BTResult exec(BTExecutorBase executor) {
            if (!condition.calc(executor.context, executor.context.context_type, out bool result)) {
                Debug.LogError("Branch: expression failed");
            }
            if (result) {
                if (true_part != null) {
                    return BTResult.child(true_part);
                }
                return BTResult.success;
            }
            if (false_part != null) {
                return BTResult.child(false_part); 
            }
            return BTResult.failed;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.Branch();
            node.cond = condition.export_as_expression(exporter);
            if (true_part != null && true_part.export(exporter, out var ti)) {
                node.true_part = (Foundation.Packets.cuint)(ulong)ti;
            }
            if (false_part != null && false_part.export(exporter, out var fi)) {
                node.false_part = (Foundation.Packets.cuint)(ulong)fi;
            }
            index = exporter.add_node(node);
            return true;
        }
    }
}