
using System.Collections.Generic;
using GraphNode;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class DynamicSelectorNode : BTChildNodeWithChildren {

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            var children = new (BTChildNode c, int w)[this.children.Count];

            bool has_weights = false;
            for (int i = 0; i < children.Length; ++i) {
                var child = this.children[i].value;
                if (child is WeightNode node) {
                    has_weights = true;
                    children[i] = (child, node.compute_weight(executor.context));
                } else {
                    children[i] = (child, 0);
                }
            }

            if (has_weights) {
                for (int i = 0; i < children.Length; ++i) {
                    var w = children[i].w;
                    var idx = i;
                    for (int j = i + 1; j < children.Length; ++j) {
                        var t = children[j].w;
                        if (t > w) {
                            idx = j;
                            w = t;
                        }
                    }
                    var best = children[idx].c;
                    if (idx != i) {
                        children[idx] = children[i];
                    }
                    yield return BTResult.child(best);
                    if (executor.last_result) {
                        yield return BTResult.success;
                        break;
                    }
                }
            } else {
                foreach (var child in children) {
                    yield return BTResult.child(child.c);
                    if (executor.last_result) {
                        yield return BTResult.success;
                        break;
                    }
                }
            }
        }

    }
}