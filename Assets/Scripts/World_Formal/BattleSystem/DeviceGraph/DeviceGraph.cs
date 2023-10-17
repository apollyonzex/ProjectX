
using CalcExpr;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.BattleSystem.DeviceGraph
{

    [System.Serializable]
    public class DeviceGraph : Graph {
        public override System.Type context_type => typeof(DeviceContext);

        [Display("Constants")]
        public List<Constant> constants;

    }
}
