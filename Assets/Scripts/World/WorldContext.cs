using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common;

namespace World
{
    public class WorldContext : Singleton<WorldContext>
    {
        #region outter
        public bool is_battle;

        public BattleContext bctx;
        #endregion

        //==================================================================================================

        public bool can_start_tick = false; 

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

        internal void init()
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
        }


        void ctx_tick_end()
        {
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

