using Common;
using Devices;
using Spine.Unity;
using UnityEngine;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;
using static Worlds.Missions.Battles.Caravan.BattleCaravan_Enum;

namespace Worlds.Missions.Battles.Caravan
{
    public class BattleCaravan : IEnemy_Interaction_Target, ITarget
    {
        #region 状态
        public Accstatus acc_status;
        public Liftoffstatus liftoff_status;
        public Glidestatus glide_status;
        #endregion

        #region 基本属性

        public Vector2 collider;
        public Vector2 collider_center => position + collider_offset;
        public Vector2 collider_offset;

        public Vector2 logic_position;

        public Vector2 logic_direction
        {
            get
            {
                if (velocity != Vector2.zero)
                {
                    return velocity.normalized;
                }
                return Vector2.right;
            }
        }

        public int current_hp;
        public int max_hp_limit;

        public Vector2 position
        {
            get
            {
                var pos = logic_position;
                pos.y += owner.body_height_offset;
                pos = Common.Utility.round_to_pixel(pos);
                return pos;
            }
        }

        public Vector2 direction => logic_direction;
        #endregion

        #region 运动
        public Vector2 velocity;
        public int driving_acc;
        public int braking_acc;
        public int driving_speed_limit;
        public int descend_speed_limit;
        public int descend_speed_limit_glide;

        public Main_State main_state;

        public float driving_acc_readonly
        {
            get 
            {
                float min = Config.current.driving_acc_min;
                if (driving_acc > (min * 1000)) return (float)driving_acc / 1000;
                else return min;
            }   
        }
        
        public float braking_acc_readonly => (float)braking_acc / 1000;
        public float driving_speed_limit_readonly
        {
            get 
            {
                float min = Config.current.driving_speed_limit_min;
                if (driving_speed_limit > (min * 1000)) return (float)driving_speed_limit / 1000;
                else return min;
            }   
        }

        public float descend_speed_limit_readonly => (float)descend_speed_limit / 1000;

        public float descend_speed_limit_glide_readonly
        {
            get
            {
                var e = descend_speed_limit > descend_speed_limit_glide ? descend_speed_limit : descend_speed_limit_glide; // 规则：滑翔时下落，必须大于下限
                return (float)e / 1000;
            }
        }

        Vector2 IEnemy_Interaction_Target.collider => collider;

        string IEnemy_Interaction_Target.name => "Caravan Body";

        Vector2 IEnemy_Interaction_Target.velocity => velocity;

        int IEnemy_Interaction_Target.current_hp { get => current_hp; set => current_hp = value; }
        #endregion

        BattleCaravanMgr owner;

        public SkeletonMecanim sm;
        public Vector2 sm_pos_offset;

        internal Animator anm;

        public Vector2 land_velocity;

        //==================================================================================================


        internal void init(BattleCaravanMgr owner)
        {
            this.owner = owner;
        }
    }


}

