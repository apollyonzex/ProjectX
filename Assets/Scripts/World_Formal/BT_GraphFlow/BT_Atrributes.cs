using System;

namespace World_Formal.BT_GraphFlow
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public class DisplayAttribute : GraphNode.DisplayAttribute
    {
        public DisplayAttribute(string name) : base(name)
        {
        }

        public int seq { get; set; } = 0;
    }
}

