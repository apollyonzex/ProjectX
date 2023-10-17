
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class RiseEventNode : BTChildNode {

        [Display("Name")]
        [SortedOrder(1)]
        [ShowInBody(format = "'{0}'")]
        public string name;

        [Display("Auto Reset")]
        [SortedOrder(2)]
        public bool auto_reset = true;

        public override BTResult exec(BTExecutorBase executor) {
            var name = this.name ?? string.Empty;
            if (executor.try_get_event(name, out var ev)) {
                ev.rise(auto_reset);
            } else if (!auto_reset) {
                ev = new BTEvent(true);
                executor.add_event(name, ev);
            }
            return BTResult.success;
        }
    }
}