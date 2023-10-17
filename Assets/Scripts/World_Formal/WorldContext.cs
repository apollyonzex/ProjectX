using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common_Formal;
using World_Formal.DS;

namespace World_Formal
{
    public class WorldContext : Singleton<WorldContext>
    {
        #region outter
        public bool can_start_tick = false;

        public int caravan_hp;

        public Vector2 caravan_pos;     //已经处理过车轮槽位偏移和高度
        public Vector2 caravan_velocity;
        public Vector2 caravan_acc;

        public float caravan_rad;
        public float caravan_deg => caravan_rad * Mathf.Rad2Deg;
        public Vector2 caravan_dir => new Vector2(1, MathF.Tan(caravan_rad)).normalized;

        public Common_Formal.Enum.EN_caravan_move_status caravan_move_status;
        public Common_Formal.Enum.EN_caravan_acc_status caravan_acc_status;
        public Common_Formal.Enum.EN_caravan_liftoff_status caravan_liftoff_status;
        public Common_Formal.Enum.EN_caravan_glide_status caravan_glide_status;
        public Common_Formal.Enum.EN_caravan_anim_status caravan_anim_status;
        public Common_Formal.Enum.EN_caravan_anim_trigger_status caravan_anim_trigger_status;

        public float speed_max_mod = 0; //speed_max_mod 是速度上限的修正值，默认是0，会被环境、敌人、或是卡牌效果等因素修改。可以是负值。
        public float acc_driving_mod = 0; //acc_driving_mod 是行驶时加速度的修正值，默认是0，会被环境、敌人、或是卡牌效果等因素修改。可以是负值。
        public float acc_braking_mod = 0; 

        public bool is_jump_need_calc_velocity; //跳跃，是否需要通过目标高度，反推竖直速度
        public float input_jump_height;
        public float input_jump_vy;
        public float jump_peak; //小车内置的一个达峰计时器。当小车到达本次跳跃高度的顶峰时，需要“浮空”一小段时间。
        public float jump_floating; //小车内置的另一个浮空计时器，用来计算小车的剩余浮空时长。
        public bool is_enter_jumping_this_delta;

        public float reset_dis = 64f;
        public bool is_need_reset = false;

        //战斗
        public bool is_battle = false;
        public bool is_battle_start_directly;

        public uint world_id; 
        public uint level_id;
        public uint journey_difficult;
        public Dictionary<uint, uint> played_battles = new();
        public BattleDS battleDS;

        public int be_holded_enemy_count; //顶车的怪物数量
        public float be_holded_first_enemy_offset; //首个顶车的怪物，离车的距离

        #endregion

        float m_physics_tick_timer = 0f;
        float m_delta_time;

        List<tick_action> m_change_tick_actions = new();
        Dictionary<string, tick_action> m_tick_actions = new();
        List<tick_action> m_change_tick_after_actions = new();
        Dictionary<string, tick_action> m_tick_after_actions = new();

        List<tick_action> m_change_tick1_actions = new();
        Dictionary<string, tick_action> m_tick1_actions = new();
        List<tick_action> m_change_tick2_actions = new();
        Dictionary<string, tick_action> m_tick2_actions = new();
        List<tick_action> m_change_tick3_actions = new();
        Dictionary<string, tick_action> m_tick3_actions = new();

        //==================================================================================================

        struct tick_action
        {
            public EN_tick_action_state state;
            public string name;
            public Action action;
            public int priority;
        }


        enum EN_tick_action_state
        {
            none,
            add,
            remove
        }

        //==================================================================================================

        internal void init_data()
        {
            m_delta_time = Common.Config.PHYSICS_TICK_DELTA_TIME;
            Physics2D.autoSyncTransforms = false;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }


        /// <summary>
        /// 固定帧率管理
        /// </summary>
        internal void update()
        {
            if (!can_start_tick) return;

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPaused)
            {
                m_physics_tick_timer = 0;
                do_tick();
                return;
            }
#endif
            m_physics_tick_timer += Mathf.Min(Time.deltaTime, m_delta_time * Time.timeScale);
            while (m_physics_tick_timer >= m_delta_time)
            {
                m_physics_tick_timer -= m_delta_time;

                do_tick();
            }
        }


        void do_tick()
        {
            ctx_tick_start();

            tick();
            tick1();
            tick2();
            tick3();
            Physics2D.SyncTransforms();
            Physics2D.Simulate(m_delta_time);
            after_tick();

            ctx_tick_end();
        }


        void ctx_tick_start()
        {
            if (caravan_pos.x > reset_dis)
            {
                is_need_reset = true;
            }
        }


        void ctx_tick_end()
        {
            is_need_reset = false;
            caravan_anim_trigger_status = Common_Formal.Enum.EN_caravan_anim_trigger_status.none;
        }


        void do_tick_actions(ref List<tick_action> change_list, ref Dictionary<string, tick_action> dic)
        {
            //动态增减physics_tick列表
            if (change_list.Any())
            {
                foreach (var e in change_list)
                {
                    if (e.state == EN_tick_action_state.add)
                        EX_Utility.dic_cover_add(ref dic, e.name, e);
                    else if (e.state == EN_tick_action_state.remove)
                        dic.Remove(e.name);
                }

                dic = dic.OrderBy(e => e.Value.priority).ToDictionary(e => e.Key, e => e.Value);
                change_list.Clear();
            }

            //执行
            foreach (var info in dic)
            {
                var k = info.Key;
                var v = info.Value;

                var action = dic[k].action;
                action?.Invoke();
            }
        }


        void tick()
        {
            do_tick_actions(ref m_change_tick_actions, ref m_tick_actions);
        }


        /// <summary>
        /// 外部调用： 增加physics_tick方法
        /// </summary>
        public void add_tick(int priority, string name, Action action)
        {
            var e = new tick_action()
            {
                name = name,
                action = action,
                priority = priority,
                state = EN_tick_action_state.add
            };
            m_change_tick_actions.Add(e);
        }


        /// <summary>
        /// 外部调用： 去除physics_tick方法
        /// </summary>
        public void remove_tick(string name)
        {
            var e = new tick_action()
            {
                name = name,
                state = EN_tick_action_state.remove
            };
            m_change_tick_actions.Add(e);
        }


        void after_tick()
        {
            do_tick_actions(ref m_change_tick_after_actions, ref m_tick_after_actions);
        }


        /// <summary>
        /// 外部调用： 增加physics_tick_after方法
        /// </summary>
        public void add_tick_after(int priority, string name, Action action)
        {
            var e = new tick_action()
            {
                name = name,
                action = action,
                priority = priority,
                state = EN_tick_action_state.add,
            };
            m_change_tick_after_actions.Add(e);
        }


        /// <summary>
        /// 外部调用： 去除physics_tick_after方法
        /// </summary>
        public void remove_tick_after(string name)
        {
            var e = new tick_action()
            {
                name = name,
                state = EN_tick_action_state.remove
            };
            m_change_tick_after_actions.Add(e);
        }


        //=============================================================================================

        void tick1()
        {
            do_tick_actions(ref m_change_tick1_actions, ref m_tick1_actions);
        }


        public void add_tick1(int priority, string name, Action action)
        {
            var e = new tick_action()
            {
                name = name,
                action = action,
                priority = priority,
                state = EN_tick_action_state.add
            };
            m_change_tick1_actions.Add(e);
        }


        public void remove_tick1(string name)
        {
            var e = new tick_action()
            {
                name = name,
                state = EN_tick_action_state.remove
            };
            m_change_tick1_actions.Add(e);
        }


        void tick2()
        {
            do_tick_actions(ref m_change_tick2_actions, ref m_tick2_actions);
        }


        public void add_tick2(int priority, string name, Action action)
        {
            var e = new tick_action()
            {
                name = name,
                action = action,
                priority = priority,
                state = EN_tick_action_state.add
            };
            m_change_tick2_actions.Add(e);
        }


        public void remove_tick2(string name)
        {
            var e = new tick_action()
            {
                name = name,
                state = EN_tick_action_state.remove
            };
            m_change_tick2_actions.Add(e);
        }


        void tick3()
        {
            do_tick_actions(ref m_change_tick3_actions, ref m_tick3_actions);
        }


        public void add_tick3(int priority, string name, Action action)
        {
            var e = new tick_action()
            {
                name = name,
                action = action,
                priority = priority,
                state = EN_tick_action_state.add
            };
            m_change_tick3_actions.Add(e);
        }


        public void remove_tick3(string name)
        {
            var e = new tick_action()
            {
                name = name,
                state = EN_tick_action_state.remove
            };
            m_change_tick3_actions.Add(e);
        }
    }


}

