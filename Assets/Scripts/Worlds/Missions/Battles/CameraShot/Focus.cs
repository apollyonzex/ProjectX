using Common;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;

namespace Worlds.Missions.Battles.CameraShot
{
    public class Focus : MonoBehaviour
    {
        [Header("最大镜头移动距离")]
        public float max_focus_move_limit = 10;
        [Header("小于该值，镜头位置开始复原")]
        public float speed_change_vel_lower_limit = 7.5f;
        [Header("小于该值，镜头位置结束复原")]
        public float speed_change_dis_lower_limit = 1e-2f;
        [Header("镜头被甩开的速度")]
        public float focus_move_vel = 25;
        [Header("镜头复原的速度")]
        public float focus_recover_vel = 50;

        Caravan.BattleCaravan m_caravan;
        static float shake_level;

        internal Vector2 pos;

        float m_speed_change_dis;
        float m_speed_change_dis_delta;

        float m_speed_change_vel;
        float m_speed_change_vel_delta;

        bool is_back = false;

        //==================================================================================================

        public void init(Caravan.BattleCaravan caravan)
        {
            m_caravan = caravan;
        }


        public void on_physics_tick()
        {
            //镜头在y轴并不会严格跟随大篷车运动
            //config_pos.y -= Mathf.Max(0f, m_caravan.position.y) * 0.7f;

            //根据“震动等级”晃动镜头
            var camera_shake = new Vector3(Random.value, Random.value, 0f) * shake_level * 0.7f;

            //“震动等级”会逐渐降低至0
            if (shake_level != 0)
            {
                shake_level -= 0.02f;
                shake_level = Mathf.Max(shake_level, 0f);
            }

            // 车子加减速导致镜头移动
            if (is_back)
            {
                m_speed_change_vel = Mathf.SmoothDamp(m_speed_change_vel, 0, ref m_speed_change_vel_delta, focus_move_vel * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME);
                m_speed_change_dis -= m_speed_change_vel * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;
                m_speed_change_dis = Mathf.Clamp(m_speed_change_dis, -max_focus_move_limit, 0);

                if (m_speed_change_vel < speed_change_vel_lower_limit)
                {
                    recover();
                }
                    
            }  
            
            if (!is_back && m_speed_change_dis != 0)
            {
                m_speed_change_dis = Mathf.SmoothDamp(m_speed_change_dis, 0, ref m_speed_change_dis_delta, focus_recover_vel * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME);

                if (m_speed_change_dis > -speed_change_dis_lower_limit)
                {
                    m_speed_change_dis = 0;
                    m_speed_change_dis_delta = 0;
                }                   
            }  

            //最终设置
            Vector3 car_pos = m_caravan.position;
            pos = car_pos + camera_shake + Config.current.focus_offset;
            pos.x += m_speed_change_dis;
        }


        public static void set_shake_level(float sl)
        {
            shake_level = 1 - Mathf.Exp(-sl / 18);
        }


        public void back()
        {
            is_back = true;
            m_speed_change_vel = m_caravan.driving_speed_limit_readonly;

            m_speed_change_vel_delta = 0;
            m_speed_change_dis_delta = 0;
        }


        public void recover()
        {
            is_back = false;
            m_speed_change_vel = 0;
            m_speed_change_vel_delta = 0;
        }
    }

}

