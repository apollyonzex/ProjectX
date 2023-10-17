using AutoCode.Tables;
using Common_Formal;
using World_Formal.BT_GraphFlow;
using UnityEngine;
using World_Formal.BT_GraphFlow.Helpers;
using World_Formal.Helpers;
using World_Formal.DS;

namespace World_Formal.Enemys
{
    public class Enemy : ITarget
    {
        public MonsterNormal.Record _desc;
        public MonsterNormal.e_moveType move_type;

        public Vector2 view_pos => bctx.position;
        public Quaternion view_dir => calc_view_dir();
        public float scaleX => bctx.flip == Enum.EN_Flip.Right ? 1 : -1;
        public float scaleY => bctx.is_upside_down? -1 : 1;

        Vector2 DS.ITarget.Position => bctx.position;

        Collider2D ITarget.collider => collider;

        public BT_Context bctx;
        public EnemyMgr mgr;
        public Collider2D collider;

        public bool is_collider_with_caravan = false;

        public Vector2 m_catch_pos_offset;
        public Vector2 m_hold_pos_offset;

        //==================================================================================================

        public Enemy(EnemyMgr mgr, uint id, Vector2 pos)
        {
            this.mgr = mgr;
            DB.instance.monster_normal.try_get(id, out var r);

            _desc = r;
            move_type = r.f_move_type;

            bctx = new();
            EX_Utility.try_load_asset(r.f_behaviour_tree.Item1, r.f_behaviour_tree.Item2, out BT_GraphAsset asset);
            BT_Graph_Load_Helper.instance.load_to_context(bctx, asset.graph);

            bctx.init_data(this, pos);
        }

        //==================================================================================================

        public void tick()
        {
            if (!bctx.is_alive)
            {
                mgr.set_del(this);
                return;
            }

            bctx.do_all();

            if (try_catch_caravan()) return; //扒车检定
            if (try_hold_caravan()) return; //顶车检定 

            bctx.altitude = Road_Info_Helper.try_get_altitude(bctx.position.x);
            Enemy_Move_Helper.instance.set_position(bctx);
            Enemy_State_Helper.instance.calc_state(bctx);
        }


        public Quaternion calc_view_dir()
        {
            var v = bctx.direction;
            var q = EX_Utility.quick_look_rotation_from_left(v);

            return q;
        }


        /// <summary>
        /// 尝试扒车
        /// </summary>
        public bool try_catch_caravan()
        {
            if (bctx.attack_State == Enum.EN_Attack_State.Catch && bctx.main_State != Enum.EN_Main_State.Catch) //扒车检定
            {
                if (is_collider_with_caravan)
                {
                    if (!collider.IsTouchingLayers(mgr.catching_mask)) //规则：如果检测到其他扒车中的怪物，则无法成功
                    {
                        bctx.main_State = Enum.EN_Main_State.Catch;
                        m_catch_pos_offset = bctx.position - mgr.ctx.caravan_pos;
                        mgr.change_layer(this, mgr.catching_layer);
                    }
                }
            }

            if (bctx.main_State == Enum.EN_Main_State.Catch) //规则：扒车时跟车移动
            {
                Enemy_Move_Helper.instance.follow_caravan(bctx, m_catch_pos_offset);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 尝试顶车
        /// </summary>
        public bool try_hold_caravan()
        {
            if (bctx.attack_State == Enum.EN_Attack_State.Defense && bctx.main_State != Enum.EN_Main_State.Hold) //顶车检定
            {
                var self_px = bctx.position.x;
                var caravan_px = mgr.ctx.caravan_pos.x;

                //规则：从车前方顶住
                if (!(is_collider_with_caravan && self_px > caravan_px)) return false;

                //规则：车处于贴地行驶
                if (!(mgr.ctx.caravan_liftoff_status == Common_Formal.Enum.EN_caravan_liftoff_status.ground)) return false;

                //规则：乘客最多2个
                ref var count = ref mgr.ctx.be_holded_enemy_count;
                if (!(count < 2)) return false;

                bctx.main_State = Enum.EN_Main_State.Hold;

                ref var offset = ref mgr.ctx.be_holded_first_enemy_offset;
                if (count == 0) //检定：是否为首个乘客
                    offset = self_px - caravan_px;

                m_hold_pos_offset = new(offset + Common.Config.current.between_holding_monster_offset * count ,0); //临时乘客间距
                count++;
            }

            if (bctx.main_State == Enum.EN_Main_State.Hold) //规则：顶车时跟车移动
            {
                Enemy_Move_Helper.instance.follow_caravan_when_hold(bctx, m_hold_pos_offset);
                return true;
            }

            return false;
        }


        void DS.ITarget.hurt(int dmg)
        {
            bctx.hp -= dmg;
            Debug.Log($"恶徒受伤,剩余hp：{bctx.hp}");
        }
    }
}

