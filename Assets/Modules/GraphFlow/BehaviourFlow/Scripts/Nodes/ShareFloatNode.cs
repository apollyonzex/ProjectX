
using BehaviourFlow.Exports;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ShareFloatNode : BTChildNode {

        [Display("Name")]
        [ShowInBody(format = "'{0}'")]
        [SortedOrder(1)]
        public string name;

        [Display("Value")]
        [ShowInBody]
        [SortedOrder(2)]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public Expression value;

        public override BTResult exec(BTExecutorBase executor) {
            if (!string.IsNullOrEmpty(name)) {
                if (!value.calc(executor.context, executor.context.context_type, out float ret)) {
                    Debug.LogError("ShareFloat: expression failed");
                }
                executor.context.set_shared_float(name, ret);
                return BTResult.success;
            }
            return BTResult.failed;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.ShareFloat();
            node.target_index = (Foundation.Packets.cuint)(ulong)exporter.get_shared_float_index(name ?? string.Empty);
            node.value = value.export_as_expression(exporter);
            index = exporter.add_node(node);
            return true;
        }
    }
}