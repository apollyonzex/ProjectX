using BehaviourFlow;
using System;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Enemies.Actions;
using static Worlds.Missions.Battles.Enemies.BehaviourTrees.EnemyBrain_Enum;


namespace Worlds.Missions.Battles.Enemies.BehaviourTrees
{
    public class EnemyBrain
    {
        public Enemy cell;
        public Moving_State moving_state = Moving_State.None;
        public Battle_State battle_state = Battle_State.Default;
        public Main_State main_state = Main_State.Idle;

        BTExecutor m_exec;
        EnemyBrainProxy m_proxy;
        Enemy_BT_Asset m_asset;
        EnemyAction action;

        bool m_is_colliding = false;
        public bool is_colliding_with_target => m_is_colliding;

        bool m_is_move_with_BT; //是否基于行为树移动
        public bool is_move_with_BT
        {
            get
            {
                if (is_holding || is_jumping || is_landing || is_boarding || is_jump || cell.concrete.no_brain)
                    m_is_move_with_BT = false;
                else
                    m_is_move_with_BT = true;

                return m_is_move_with_BT;
            }
            set
            {
                this.m_is_move_with_BT = value;
            }
        }

        bool m_is_move_with_caravan; //是否跟车移动
        bool is_move_with_caravan
        {
            get
            {
                if (is_holding || is_boarding)
                    m_is_move_with_caravan = true;
                else
                    m_is_move_with_caravan = false;

                return m_is_move_with_caravan;
            }
            set
            {
                this.m_is_move_with_caravan = value;
            }
        }

        public IEnemy_Interaction_Target target;
        public float target_dis_x = 0;
        public float target_dis_y = 0;
        public Vector2 velocity;
        public Vector2 acc;
        public Side_State side = Side_State.None;
        public Vector2 velocity_result => velocity + acc * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;

        public bool is_jump = false;
        public int landing_rest_ticks = 0;
        public int boarding_atk_rest_ticks = 0;

        public bool is_holding => main_state == Main_State.Holding;
        public bool is_jumping => main_state == Main_State.Jumping;
        public bool is_landing => main_state == Main_State.Landing;
        public bool is_boarding => main_state == Main_State.Boarding;

        //==================================================================================================


        public void init(Enemy cell, string bundle, string path)
        {
            m_exec = new BTExecutor();
            m_proxy = new EnemyBrainProxy(this);

            this.cell = cell;
            action = new EnemyAction(this);

            m_asset = Common.Utility.load_asset<Enemy_BT_Asset>(bundle, path);
            m_exec.reset(m_proxy, m_asset, false);
        }


        /// <summary>
        /// 逐帧更新行为
        /// </summary>
        public void on_physics_tick()
        {
            upd_parameters();
            do_behaviourTree();

            do_move();
            upd_parameters();

            check_colliding();
            do_battle();
            do_main();
        }


        /// <summary>
        /// 根据Moving_State执行action
        /// </summary>
        void do_move()
        {
            if (!is_move_with_BT)
            {
                if (is_move_with_caravan)
                    move_with_caravan();
                return;
            }

            switch (moving_state)
            {
                case Moving_State.None:
                    break;

                case Moving_State.Stop:
                    action.stop_moving();
                    break;

                case Moving_State.ToTarget:
                    action.moving_to_target();
                    break;

                case Moving_State.Free:
                    break;
            }
        }


        /// <summary>
        /// 根据Battle_State执行action
        /// </summary>
        void do_battle()
        {
            switch (battle_state)
            {
                case Battle_State.Default:
                    break;

                case Battle_State.Withstand:
                    action.hold();
                    break;

                case Battle_State.Melee:
                    action.melee_attack();
                    break;

                case Battle_State.Board:
                    action.board();
                    break;
            }
        }


        /// <summary>
        /// 根据Main_State执行action
        /// 播放动画可以在这里拓展
        /// </summary>
        void do_main()
        {
            if (main_state == Main_State.Dead) return;

            if (main_state == Main_State.Moving || main_state == Main_State.Idle)
            {
                if (velocity_result.magnitude > 0.1f)
                    main_state = Main_State.Moving;
                else
                    main_state = Main_State.Idle;
            }

            switch (main_state)
            {
                case Main_State.Idle:
                    break;

                case Main_State.Moving:
                    break;

                case Main_State.Holding:
                    break;

                case Main_State.Boarding:
                    action.boarding(ref boarding_atk_rest_ticks);
                    break;

                case Main_State.Jumping:
                    break;

                case Main_State.Landing:
                    action.landing(ref landing_rest_ticks);
                    break;

                case Main_State.Missile:
                    action.missile_attack();
                    break;
            }

            cell.set_anm_state((int)main_state);

            if (main_state == Main_State.Jump)
                main_state = Main_State.Jumping;
        }


        /// <summary>
        /// 更新参数
        /// </summary>
        public void upd_parameters()
        {
            if (target == null) return;
            check_side_state();
            check_dis();
        }


        /// <summary>
        /// 执行行为树
        /// </summary>
        public void do_behaviourTree()
        {
            if (!is_move_with_BT) return;

            var bl = m_exec.exec();
            if (bl)
            {
                m_exec.reset(m_proxy, m_asset);
            }
        }


        /// <summary>
        /// 检查并刷新: 自身相对于目标的位置
        /// </summary>
        void check_side_state()
        {
            var target_x = target.position.x;
            var self_x = cell.position.x;

            if (self_x <= target_x)
            {
                side = Side_State.Behind;
                cell.direction = Vector2.right;
            }
            else
            {
                side = Side_State.Front;
                cell.direction = Vector2.left;
            }
        }


        /// <summary>
        /// 检查并刷新: 自身相对于目标的距离
        /// </summary>
        void check_dis()
        {
            target_dis_x = Mathf.Abs(cell.position.x - target.position.x);
            target_dis_y = Mathf.Abs(cell.position.y - target.position.y);
        }


        /// <summary>
        /// 检查并刷新: 是否正在和目标发生碰撞
        /// </summary>
        void check_colliding()
        {
            if (target == null) return;
            m_is_colliding = Utility.check_contain_two_rectangleArea(cell.collider_center, cell.collider, target.collider_center, target.collider);
        }


        /// <summary>
        /// 进入battle_state
        /// </summary>
        public bool try_enter_battle_state(int id)
        {
            var array = Enum.GetValues(typeof(Battle_State));
            if (id >= array.Length || id < 0)
            {
                Debug.LogWarning($"行为树, 战斗姿态配置错误, 当前序号为{id}");
                return false;
            }

            battle_state = (Battle_State)array.GetValue(id);
            return true;
        }


        /// <summary>
        /// 跟车移动
        /// </summary>
        public void move_with_caravan()
        {
            if (target is not BattleCaravan caravan)
            {
                Debug.LogWarning("跟车移动失败: 当前目标不是车体!");
                return;
            }

            velocity = caravan.velocity;
            acc = Vector2.zero;
        }


        /// <summary>
        /// 执行跳跃
        /// </summary>
        public void jump(float height, float vx)
        {
            is_jump = true;
            action.jump(height, vx);

            main_state = Main_State.Jump;
            is_jump = false;
        }


        /// <summary>
        /// 死亡，改变状态
        /// </summary>
        public void on_dead()
        {
            cell.mgr.leave_all_state(cell);
            main_state = Main_State.Dead;
            moving_state = Moving_State.Stop;
        }
    }
}

