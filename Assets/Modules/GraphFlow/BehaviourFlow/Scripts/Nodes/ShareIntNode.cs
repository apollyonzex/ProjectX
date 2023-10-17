
using BehaviourFlow.Exports;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ShareIntNode : BTChildNode {

        [Display("Name")]
        [ShowInBody(format = "'{0}'")]
        [SortedOrder(1)]
        public string name;

        [Display("Value")]
        [ShowInBody]
        [SortedOrder(2)]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public Expression value;

        public override BTResult exec(BTExecutorBase executor) {
            if (!string.IsNullOrEmpty(name)) {
                if (!value.calc(executor.context, executor.context.context_type, out int ret)) {
                    Debug.LogError("ShareInt: expression failed");
                }
                executor.context.set_shared_int(name, ret);
                return BTResult.success;
            }
            return BTResult.failed;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.ShareInt();
            node.target_index = (Foundation.Packets.cuint)(ulong)exporter.get_shared_int_index(name ?? string.Empty);
            index = exporter.add_node(node);
            return true;
        }
    }
}