
using System;

namespace GraphNode.Editor {

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GraphEditorAttribute : Attribute {
        public Type graph_type { get; }
        public GraphEditorAttribute(Type graph_type) {
            this.graph_type = graph_type;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NodeEditorAttribute : Attribute {
        public Type node_type { get; }
        public NodeEditorAttribute(Type node_type) {
            this.node_type = node_type;
        }
    }

    
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PropertyEditorAttribute : Attribute {
        public Type property_type { get; }
        public PropertyEditorAttribute(Type property_type) {
            this.property_type = property_type;
        }
    }

}