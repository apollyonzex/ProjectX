
using BehaviourFlow.Exports;
using GraphNode;
using UnityEngine;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class SwitchNode : BTChildNodeWithChildren {

        [Display("Expression")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public Expression expression;

        [Display("_")]
        [Output]
        public BTChildNode default_child { get; set; }

        public override BTResult exec(BTExecutorBase executor) {
            if (!expression.calc(executor.context, executor.context.context_type, out int ret)) {
                Debug.LogError("Switch: expression failed");
            }
            if (ret >= 0 && ret < children.Count) {
                return BTResult.child(children[ret].value);
            }
            if (default_child != null) {
                return BTResult.child(default_child);
            }
            return BTResult.failed;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.BranchEx();
            node.value = expression.export_as_expression(exporter);
            if (default_child != null && default_child.export(exporter, out var def_index)) {
                node.def = (Foundation.Packets.cuint)(ulong)def_index;
            }
            var _children = new System.Collections.Generic.List<Foundation.Packets.cuint>(children.Count);
            foreach (var child in children) {
                if (child.value != null) {
                    if (child.value.export(exporter, out var child_index)) {
                        _children.Add((ulong)child_index);
                    }
                }
            }
            node.children.items = _children.ToArray();
            index = exporter.add_node(node);
            return true;
        }
    }
}