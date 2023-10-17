using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ExternalNode : BTChildNode {

        
        [NonProperty]
        public string name;

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            yield return BTResult.external(name);
            if (executor.last_result) {
                yield return BTResult.success;
            }
        }

    }
}