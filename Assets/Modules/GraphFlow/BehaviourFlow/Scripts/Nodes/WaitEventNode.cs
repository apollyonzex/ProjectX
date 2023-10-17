
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class WaitEventNode : BTChildNode {

        [Display("Name")]
        [ShowInBody(format = "'{0}'")]
        [SortedOrder(1)]
        public string name;

        public override BTResult exec(BTExecutorBase executor) {
            var name = this.name ?? string.Empty;
            if (!executor.try_get_event(name, out var ev)) {
                ev = new BTEvent(false);
                executor.add_event(name, ev);
            } else if (ev.rised) {
                return BTResult.success;
            }
            return BTResult.enumerator(_exec(ev));
        }

        private IEnumerator<BTResult> _exec(BTEvent ev) {
            var ticket = ev.ticket;
            do {
                yield return BTResult.pending;
            } while (!ev.poll_wait(ticket));
            yield return BTResult.success;
        }
    }
}