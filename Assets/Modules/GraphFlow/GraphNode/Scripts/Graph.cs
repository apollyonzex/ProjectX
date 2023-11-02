
using System.Collections.Generic;

namespace GraphNode {

    [System.Serializable]
    public abstract class Graph {

        public virtual string name => "Root";

        public virtual void before_serialize(GraphAsset asset, List<UnityEngine.Object> referenced_objects) {
            foreach (var node in nodes) {
                node.on_serializing(referenced_objects);
            }
        }
        public virtual void after_deserialize(GraphAsset asset, UnityEngine.Object[] referenced_objects) {
            foreach (var node in nodes) {
                node.on_deserialized(referenced_objects);
            }
        }

        public abstract System.Type context_type { get; }

        public Node[] nodes { get; set; }
        public Connection[] connections { get; set; }
    }

}