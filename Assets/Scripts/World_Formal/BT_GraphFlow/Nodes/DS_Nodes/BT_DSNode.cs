using GraphNode;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes.DS_Nodes
{
    [Serializable]
    public class BT_DSNode : BT_Node
    {
        [ShowInBody(format = "[{0}]")]
        public string module_name;

        public virtual Type cpn_type { get; } = null;

        //================================================================================================

        public virtual T init_cpn<T>()
        {
            var cpn = (T)Activator.CreateInstance(cpn_type);
            return cpn;
        }
    }
}

