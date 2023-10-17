using BehaviourFlow.Exports;
using CalcExpr;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class LoopNode : BTChildNode {

        public enum Mode {
            While,
            Until,
            Always,
        }

        [Display("Mode")]
        [ShowInBody]
        [SortedOrder(1)]
        public Mode mode;

        [ExpressionType(ValueType.Integer)]
        [Display("Interval")]
        [SortedOrder(2)]
        public Expression interval;

        [ExpressionType(ValueType.Integer)]
        [Display("Max Count")]
        [SortedOrder(3)]
        public Expression max_count;

        [Display("")]
        [Output]
        public BTChildNode child { get; set; }

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            max_count.calc(executor.context, executor.context.context_type, out int cnt);
            interval.calc(executor.context, executor.context.context_type, out int _interval);
            _interval = Mathf.Max(_interval, 1);
            if (cnt > 0) {
                switch (mode) {
                    case Mode.While: return do_while(executor, _interval, cnt);
                    case Mode.Until: return do_until(executor, _interval, cnt);
                    default: return do_always(_interval, cnt);
                }
            }
            switch (mode) {
                case Mode.While: return do_while(executor, _interval);
                case Mode.Until: return do_until(executor, _interval);
                default: return do_always(_interval);
            }
        }

        private IEnumerator<BTResult> do_while(BTExecutorBase executor, int interval) {
        l_again:
            yield return BTResult.child(child);
            if (!executor.last_result) {
                yield break;
            }
            int i = interval;
            do {
                yield return BTResult.pending;
                --i;
            } while (i > 0);
            goto l_again;
        }
        private IEnumerator<BTResult> do_while(BTExecutorBase executor, int interval, int count) {
            for (; ;) {
                yield return BTResult.child(child);
                if (!executor.last_result) {
                    yield break;
                }
                if (--count == 0) {
                    break;
                }
                int i = interval;
                do {
                    yield return BTResult.pending;
                    --i;
                } while (i > 0);   
            }
            yield return BTResult.success;
        }

        private IEnumerator<BTResult> do_until(BTExecutorBase executor, int interval) {
        l_again:
            yield return BTResult.child(child);
            if (executor.last_result) {
                yield return BTResult.success;
            }
            int i = interval;
            do {
                yield return BTResult.pending;
                --i;
            } while (i > 0);
            goto l_again;
        }
        private IEnumerator<BTResult> do_until(BTExecutorBase executor, int interval, int count) {
            for (; ; ) {
                yield return BTResult.child(child);
                if (executor.last_result) {
                    yield return BTResult.success;
                }
                if (--count == 0) {
                    break;
                }
                int i = interval;
                do {
                    yield return BTResult.pending;
                    --i;
                } while (i > 0);
            }
        }

        private IEnumerator<BTResult> do_always(int interval) {
        l_again:
            yield return BTResult.child(child);
            int i = interval;
            do {
                yield return BTResult.pending;
                --i;
            } while (i > 0);
            goto l_again;
        }

        private IEnumerator<BTResult> do_always(int interval, int count) {
            for (; ;) {
                yield return BTResult.child(child);
                if (--count == 0) {
                    break;
                }
                int i = interval;
                do {
                    yield return BTResult.pending;
                    --i;
                } while (i > 0);
            }
            yield return BTResult.success;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.Loop();
            if (child != null && child.export(exporter, out var child_index)) {
                node.child = (Foundation.Packets.cuint)(ulong)child_index;
            }
            switch (mode) {
                case Mode.While:
                    node.mode = 1;
                    break;
                case Mode.Until:
                    node.mode = 2;
                    break;
            }
            node.interval = interval.export_as_expression(exporter);
            node.count = max_count.export_as_expression(exporter);
            index = exporter.add_node(node);
            return true;
        }
    }
}