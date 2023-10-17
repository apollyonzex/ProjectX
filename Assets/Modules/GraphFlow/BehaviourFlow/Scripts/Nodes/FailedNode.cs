
using BehaviourFlow.Exports;
using GraphNode;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class FailedNode : BTChildNode {

        public override BTResult exec(BTExecutorBase executor) {
            return BTResult.failed;
        }

        public override bool export(Exporter exporter, out int index) {
            index = exporter.add_node(new AutoCode.Packets.BehaviourFlowExports.Failed());
            return true;
        }
    }
}