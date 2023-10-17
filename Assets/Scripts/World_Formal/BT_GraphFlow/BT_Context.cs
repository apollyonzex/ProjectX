using World_Formal.BT_GraphFlow.DS;
using World_Formal.BT_GraphFlow.Helpers;
using World_Formal.BT_GraphFlow.Nodes;
using GraphNode;
using System;
using System.Collections.Generic;
using UnityEngine;
using CalcExpr;
using System.Linq;
using Common_Formal;
using World_Formal.Enemys;
using World_Formal.Helpers;
using System.Reflection;
using World_Formal.DS;

namespace World_Formal.BT_GraphFlow
{
    [Serializable]
    public class BT_Context : IContext
    {
        Type IContext.context_type => typeof(BT_Context);
        int m_i = 0;
        int m_freeze_time = 0;
        bool m_is_leave_freeze = false;
        bool m_is_freeze => m_freeze_time > 0;

        public List<BT_Graph_Load_Helper.Step> do_list;
        public Dictionary<string, BT_CPN> cpns_dic;
        public object[] static_prms;

        public bool is_last_method_ret = true;

        public Stack<BT_Node> unactive_nodes_stack = new();
        public Stack<LoopNode> loop_nodes_stack = new();

        #region outter_prms
        public ITarget target;
        public Vector2 position;
        public Vector2 direction = Vector2.right;
        public Enemys.Enum.EN_Flip flip = Enemys.Enum.EN_Flip.Right; //翻面朝左/右,默认右
        public bool is_upside_down = false; //是否启用上下倒转

        public Vector2 velocity => v_self + v_enviroment + v_impact;
        public Vector2 v_self;
        public Vector2 v_enviroment;
        public Vector2 v_impact;

        public Enemys.Enum.EN_Main_State main_State;
        public Enemys.Enum.EN_Attack_State attack_State;

        public int hp;
        public bool is_alive => hp > 0;

        public bool is_use_gravity; //是否被重力影响
        public float altitude; //当前位置的地面高度
        public bool is_in_sky => position.y > altitude; //是否滞空

        public bool is_jump = false; //是否处于跳跃

        public Enemy cell;
        #endregion

        //================================================================================================

        public BT_Context()
        {
            do_list = new();
            cpns_dic = new();
            static_prms = new object[] { this };
            
        }


        public BT_Context(BT_GraphAsset asset)
        {
            BT_Graph_Load_Helper.instance.load_to_context(this, asset.graph);
        }


        public void init_data(Enemy cell, Vector2 pos)
        {
            this.cell = cell;
            position = cell.mgr.ctx.caravan_pos + pos;
            hp = cell._desc.f_hp;

            Enemy_Move_Helper.instance.init_move_prms(this);
        }


        public bool try_get_cpn(string name, out BT_CPN cpn)
        {
            return cpns_dic.TryGetValue(name, out cpn);
        }


        #region opr
        public void @do(int times = 1, BT_Node _node = null, MethodInfo _method = null)
        {
            if (m_is_freeze) return;

            var e = do_list[m_i];
            var node = e.self;
            var method = e.method_info;

            bool is_use_outter = _node != null && _method != null;
            if (is_use_outter)
            {
                node = _node;
                method = _method;
            }

            if (!unactive_nodes_stack.Any())
            {
                method.Invoke(node, static_prms);
            }
            else if (unactive_nodes_stack.Peek() == node)
            {
                if (method.Name == "do_back")
                {
                    method.Invoke(node, static_prms);
                    unactive_nodes_stack.Pop();
                }
            }

            if (!is_use_outter)
            {
                if (loop_nodes_stack.Any())
                {
                    foreach (var loop_node in loop_nodes_stack)
                    {
                        loop_node.loop_childs.Add((method, node));
                    }
                }

                m_i++;
            }
            
            times--;

            if (m_i == do_list.Count)
                m_i = 0;

            if (times > 0)
                @do(times);
        }


        public void @reset()
        {
            m_i = 0;
        }


        public void do_all()
        {
            if (m_freeze_time > 0)
            {
                m_freeze_time--;

                if (m_freeze_time == 0)
                    m_is_leave_freeze = true;

                return;
            }

            if (!m_is_leave_freeze)
                @reset();
            else
                m_is_leave_freeze = false;

            @do(do_list.Count - m_i);
        }


        /// <summary>
        /// 查询本节点是否执行结束
        /// </summary>
        public bool query_is_node_end(BT_Node node)
        {
            var behind = do_list[m_i + 1].self;
            return behind != node;
        }


        public void freeze(int time)
        {
            m_freeze_time = time;
        }


        public BT_Node get_next_node()
        {
            var step = do_list[m_i + 1];
            return step.self;
        }
        #endregion


        [ExprConst(".")]
        public string separator => null;//分隔符


        [ExprConst("has_target")]
        public bool has_target => target != null;//是否有目标


        [ExprConst("is_in_sky")]
        public bool v_is_in_sky => is_in_sky;//是否滞空


        [ExprConst("main_state")]
        public int q_main_state => (int)main_State;//查询主状态


        [ExprConst("attack_state")]
        public int q_attack_state => (int)attack_State;//查询攻击状态


        [ExprFunc]
        public float rnd_float(float min, float max)
        {
            return EX_Utility.rnd_float(min, max);
        }


        [ExprFunc]
        public float get_ground_altitude(float x)
        {
            return Road_Info_Helper.try_get_altitude(x);
        }


        [GraphAction]
        public bool do_123(Test test)
        {
            return false;
        }


        [GraphAction]
        public void do_void(Test test)
        {
            Debug.Log("dododo");
        }



    }
}

