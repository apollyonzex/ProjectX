
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow {

    [System.Serializable]
    public class BTNode : Node {

        public virtual BTResult exec(BTExecutorBase executor) {
            return BTResult.enumerator(_exec(executor));
        }
        protected virtual IEnumerator<BTResult> _exec(BTExecutorBase executor) { return null; }
    }

    [System.Serializable]
    [Input]
    public class BTChildNode : BTNode {
        public virtual bool export(Exports.Exporter exporter, out int index) {
            index = default;
            return false;
        }
    }

    [System.Serializable]
    public class BTChildNodePort : NodeDynamicNodePort<BTChildNode> {

        public override IO io => IO.Output;
        public override string name => index.ToString();

        [System.NonSerialized]
        public int index;
    }

    [System.Serializable]
    public class BTChildNodeWithChildren : BTChildNode {
        [Display("Children")]
        [SortedOrder(99)]
        public List<BTChildNodePort> children;
    }
}