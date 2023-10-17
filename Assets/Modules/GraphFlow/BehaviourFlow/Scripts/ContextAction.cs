
using GraphNode;

namespace BehaviourFlow {
    [System.Serializable]
    public class ContextAction : Action<BTExecutorBase> {
        public override Action<BTExecutorBase> clone() {
            return clone_to(new ContextAction());
        }
    }
}