
using BehaviourFlow.Exports;
using CalcExpr;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class DelayNode : BTChildNode {

        [ExpressionType(ValueType.Integer)]
        [Display("Count")]
        [ShowInBody]
        [SortedOrder(1)]
        public Expression count;

        [ExpressionType(ValueType.Boolean)]
        [Display("Result")]
        [ShowInBody]
        [SortedOrder(2)]
        public Expression result;

        public override BTResult exec(BTExecutorBase executor) {
            if (!count.calc(executor.context, executor.context.context_type, out int cnt)) {
                Debug.LogError("Delay: count failed");
            }
            if (cnt <= 0) {
                if (!result.calc(executor.context, executor.context.context_type, out bool ret)) {
                    Debug.LogError("Delay: result failed");
                    return BTResult.failed;
                }
                if (ret) {
                    return BTResult.success;
                }
                return BTResult.failed;
            }
            return BTResult.enumerator(_exec(executor, cnt));
        }

        private IEnumerator<BTResult> _exec(BTExecutorBase executor, int cnt) {
            do {
                yield return BTResult.pending;
                --cnt;
            } while (cnt > 0);
            if (!result.calc(executor.context, executor.context.context_type, out bool ret)) {
                Debug.LogError("Delay: result failed");
                yield break;
            }
            if (ret) {
                yield return BTResult.success;
            }
        }

        public override bool export(Exporter exporter, out int index) {
            if (result.constant.HasValue) {
                index = exporter.add_node(new AutoCode.Packets.BehaviourFlowExports.Delay {
                    count = count.export_as_expression(exporter),
                });
                if (result.constant.Value == 0) {
                    var node = new AutoCode.Packets.BehaviourFlowExports.Not() {
                        child = (Foundation.Packets.cuint)(ulong)index,
                    };
                    index = exporter.add_node(node);
                }
            } else {
                index = exporter.add_node(new AutoCode.Packets.BehaviourFlowExports.DelayCond {
                    count = count.export_as_expression(exporter),
                    cond = result.export_as_expression(exporter),
                });
            }
            return true;
        }
    }
}