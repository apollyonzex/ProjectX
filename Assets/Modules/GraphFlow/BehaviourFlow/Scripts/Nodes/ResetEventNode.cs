
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ResetEventNode : BTChildNode {

        [Display("Name")]
        [ShowInBody(format = "'{0}'")]
        public string name;

        public override BTResult exec(BTExecutorBase executor) {
            var name = this.name ?? string.Empty;
            if (executor.try_get_event(name, out var ev)) {
                ev.reset();
            }
            return BTResult.success;
        }
    }
}