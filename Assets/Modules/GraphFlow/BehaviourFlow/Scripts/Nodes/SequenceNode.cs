
using BehaviourFlow.Exports;
using GraphNode;
using System.Collections.Generic;
using Foundation.Packets;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class SequenceNode : BTChildNodeWithChildren {

        public override BTResult exec(BTExecutorBase executor) {
            if (children.Count == 0) {
                return BTResult.failed;
            }
            return base.exec(executor);
        }

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            foreach (var child in children) {
                yield return BTResult.child(child.value);
                if (!executor.last_result) {
                    yield break;
                }
            }
            yield return BTResult.success;
        }

        public override bool export(Exporter exporter, out int index) {
            var _children = new System.Collections.Generic.List<cuint>(children.Count);
            var node = new AutoCode.Packets.BehaviourFlowExports.Sequence();
            foreach (var child in children) {
                if (child.value != null) {
                    if (child.value.export(exporter, out var child_index)) {
                        _children.Add((ulong)child_index);
                    }
                }
            }
            node.children.items = _children.ToArray();
            index = exporter.add_node(node);
            return true;
        }
    }
}