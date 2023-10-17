using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Set_Attack_StateNode : BT_Node
    {
        [ShowInBody(format = "[state] -> {0}")]
        public Enemys.Enum.EN_Attack_State state;

        Dictionary<Enemys.Enum.EN_Attack_State, System.Action<BT_Context>> m_state_dic = new()
        {
            { Enemys.Enum.EN_Attack_State.Default, change_to_default },
            { Enemys.Enum.EN_Attack_State.Catch, change_to_catch },
            { Enemys.Enum.EN_Attack_State.Defense, change_to_defense },
        };

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            m_state_dic.TryGetValue(state, out var action);

            action?.Invoke(ctx);
            ctx.is_last_method_ret = true;
        }

        //================================================================================================

        static void change_to_default(BT_Context bt_ctx)
        {
            bt_ctx.attack_State = Enemys.Enum.EN_Attack_State.Default;
        }


        static void change_to_catch(BT_Context bt_ctx)
        {
            bt_ctx.attack_State = Enemys.Enum.EN_Attack_State.Catch;
        }


        static void change_to_defense(BT_Context bt_ctx)
        {
            bt_ctx.attack_State = Enemys.Enum.EN_Attack_State.Defense;
        }
    }
}

