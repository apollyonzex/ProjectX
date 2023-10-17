
using GraphNode;
using UnityEngine;
using System.Collections.Generic;
using BehaviourFlow.Exports;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class StateNode : BTChildNode {

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;

        [Display("")]
        [Output]
        public BTChildNode child { get; set; }

        public override BTResult exec(BTExecutorBase executor) {
            if (!_check(executor) || child == null) {
                return BTResult.failed;
            }
            return BTResult.enumerator(_exec(executor));
        }

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            var child_exec = new BTChildExecutor(executor);
#if UNITY_EDITOR
            child_exec.debug_info("state", executor.game_object);
#endif
            executor.on_abort(() => child_exec.dispose());

            if (child_exec.reset(child, executor.last_asset, true)) {
                goto l_child_done;
            }
            yield return BTResult.pending;
        
            while (!child_exec.exec()) {
                yield return BTResult.pending;
                if (!_check(executor)) {
                    goto l_failed;
                }
            }

        l_child_done:
            var child_ret = child_exec.last_result;
            child_exec.dispose();
            if (child_ret) {
                yield return BTResult.success;
            }
            yield break;

        l_failed:
            child_exec.dispose();
        }

        private bool _check(BTExecutorBase executor) {
            if (!condition.calc(executor.context, executor.context.context_type, out bool ret)) {
                Debug.LogError("Condition: expression failed");
            }
            return ret;
        }

        
        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.State();
            node.cond = condition.export_as_expression(exporter);
            if (child != null && child.export(exporter, out var child_index)) {
                node.child = (Foundation.Packets.cuint)(ulong)child_index;
            }
            index = exporter.add_node(node);
            return true;
        }
    }
}