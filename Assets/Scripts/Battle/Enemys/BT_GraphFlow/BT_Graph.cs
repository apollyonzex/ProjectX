﻿using GraphNode;
using System;

namespace Battle.Enemys.BT_GraphFlow
{
    [System.Serializable]
    public class BT_Graph : Graph
    {
        public override Type context_type => typeof(BT_Context);
    }
}
