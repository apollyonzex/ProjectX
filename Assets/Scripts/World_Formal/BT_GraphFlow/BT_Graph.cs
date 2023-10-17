using GraphNode;
using System;

namespace World_Formal.BT_GraphFlow
{
    [System.Serializable]
    public class BT_Graph : Graph
    {
        public override Type context_type => typeof(BT_Context);
    }
}

