
using GraphNode;

namespace DialogFlow {

    [System.Serializable]
    public class ContextAction : Action {
        public override Action clone() {
            return clone_to(new ContextAction());
        }
    }
}