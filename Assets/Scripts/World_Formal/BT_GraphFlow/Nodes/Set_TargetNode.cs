using Common;
using Common_Formal;
using GraphNode;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.Caravans;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Set_TargetNode : BT_Node
    {
        [ShowInBody(format = "[target] -> {0}")]
        public Enum.EN_target_type target;

        Dictionary<Enum.EN_target_type, System.Action<BT_Context>> m_target_dic = new()
        {
            { Enum.EN_target_type.caravan, find_caravan },
            { Enum.EN_target_type.device, null },
            { Enum.EN_target_type.enemy, null },
        };

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            m_target_dic.TryGetValue(target, out var action);

            if (action != null)
            {
                action.Invoke(ctx);
                ctx.is_last_method_ret = true;
            }
            else
            {
                ctx.target = null;
                ctx.is_last_method_ret = false;
            }  
        }

        //================================================================================================

        static void find_caravan(BT_Context bt_ctx)
        {
            if (bt_ctx.target is Caravan) return;

            Mission.instance.try_get_mgr(Config.CaravanMgr_Name, out CaravanMgr_Formal mgr);
            bt_ctx.target = mgr.cell;
        }
    }
}

