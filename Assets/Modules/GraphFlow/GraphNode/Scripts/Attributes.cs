
using System;

namespace GraphNode {

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class NonPropertyAttribute : Attribute {

    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NodePropertyPortAttribute : Attribute {
        public Type property_type { get; }
        public NodePropertyPortAttribute(Type property_type) {
            this.property_type = property_type;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
    public class InputAttribute : Attribute {
        public bool can_multi_connect { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
    public class OutputAttribute : Attribute {
        public bool can_multi_connect { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UniqueAttribute : Attribute {

    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GraphAttribute : Attribute {
        public Type[] graphs { get; }

        public GraphAttribute(params Type[] graphs) {
            this.graphs = graphs;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public class DisplayAttribute : Attribute {
        public string name { get; }
        public DisplayAttribute(string name) {
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public class SortedOrderAttribute : Attribute {
        public int order { get; }
        public SortedOrderAttribute(int order) {
            this.order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ShowInBodyAttribute : Attribute {
        public string format;
    }

}