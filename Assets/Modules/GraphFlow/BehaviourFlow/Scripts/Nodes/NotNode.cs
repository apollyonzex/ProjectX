
using BehaviourFlow.Exports;
using Foundation.Packets;
using GraphNode;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class NotNode : BTChildNode {

        [Display("")]
        [Output]
        public BTChildNode child { get; set; }

        public override BTResult exec(BTExecutorBase executor) {
            if (child == null) {
                return BTResult.failed;
            }
            return BTResult.inversed_child(child);
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.Not();
            if (child != null && child.export(exporter, out var child_index)) {
                node.child = (cuint)(ulong)child_index;
            }
            index = exporter.add_node(node);
            return true;
        }
    }
}