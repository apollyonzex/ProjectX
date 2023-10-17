
using CalcExpr;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class WaitEventTimeoutNode : WaitEventNode {

        [ExpressionType(ValueType.Integer)]
        [Display("Timeout Count")]
        [ShowInBody]
        [SortedOrder(2)]
        public Expression count;

        [ExpressionType(ValueType.Boolean)]
        [Display("Timeout Result")]
        [ShowInBody]
        [SortedOrder(3)]
        public Expression result;


        public override BTResult exec(BTExecutorBase executor) {
            var name = this.name ?? string.Empty;
            if (!executor.try_get_event(name, out var ev)) {
                ev = new BTEvent(false);
                executor.add_event(name, ev);
            } else if (ev.rised) {
                return BTResult.success;
            }

            count.calc(executor.context, executor.context.context_type, out int cnt);
            if (cnt <= 0) {
                result.calc(executor.context, executor.context.context_type, out bool ret);
                return ret ? BTResult.success : BTResult.failed;
            }
            return BTResult.enumerator(_exec(executor, ev, cnt));
        }

        private IEnumerator<BTResult> _exec(BTExecutorBase executor, BTEvent ev, int cnt) {
            var ticket = ev.ticket;
            do {
                yield return BTResult.pending;
                if (ev.poll_wait(ticket)) {
                    yield return BTResult.success;
                    yield break;
                }
                --cnt;
            } while (cnt > 0);
            result.calc(executor.context, executor.context.context_type, out bool ret);
            if (ret) {
                yield return BTResult.success;
            }
        }
    }
}