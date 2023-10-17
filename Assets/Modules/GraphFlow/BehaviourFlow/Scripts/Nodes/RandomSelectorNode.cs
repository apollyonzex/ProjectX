
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class RandomSelectorNode : BTChildNodeWithChildren {

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            var children = new (BTChildNode c, int w)[this.children.Count];

            bool has_weights = false;


            for (int i = 0; i < children.Length; ++i) {
                var child = this.children[i].value;
                if (child is WeightNode node) {
                    has_weights = true;
                    children[i] = (child, UnityEngine.Mathf.Max(0, node.compute_weight(executor.context)));
                } else {
                    children[i] = (child, 1);
                }
            }

            if (has_weights) {
                var total = 0;
                for (int i = 0; i < children.Length; ++i) {
                    total += children[i].w;
                }
                int index = 0;
                while (total > 0) {
                    var roll = random(0, total);
                    for (int i = index; ; ++i) {
                        var child = children[i];
                        if (roll < child.w) {
                            yield return BTResult.child(child.c);
                            if (executor.last_result) {
                                yield return BTResult.success;
                            }
                            if (i != index) {
                                children[i] = children[index];
                            }
                            total -= child.w;
                            ++index;
                            break;
                        }
                        roll -= child.w;
                    }
                }
            } else {
                for (int i = 0; i < children.Length; ++i) {
                    var roll = random(i, children.Length);
                    var child = children[roll].c;
                    if (roll != i) {
                        children[roll].c = children[i].c;
                    }
                    yield return BTResult.child(child);
                    if (executor.last_result) {
                        yield return BTResult.success;
                        break;
                    }
                }
            }
        }

        protected int random(int min, int max) {
            return UnityEngine.Random.Range(min, max);
        }
    }
}