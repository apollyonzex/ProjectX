using CalcExpr;
using Common;
using Devices;
using System.Collections;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;


namespace Worlds.Missions.Battles.Enemies
{
    public interface IEnemy
    {
        void calc_active();
        void calc_impacting();
        void calc_environmet();
        void upd_position(ref Vector2 position);

        public Vector2 environment_v { get; } //环境移速
        public Vector2 impact_v { get; } //击退移速
        public Vector2 active_v { get; } //击退移速

        void notify_on_impact(float dis, Vector2 dir);
        void notify_on_dead(ref Vector2 brain_velocity, ref Vector2 brain_acc);

        //锁定相关功能
        public bool no_gravity { get; }
        public bool no_brain { get; }
    }


    public class Enemy : IEnemy_Interaction_Target, ITarget
    {

        //---       用于图流表达式的属性
        [ExprConst("velocity.x")]
        public float speed_x => velocity.x;
        [ExprConst("velocity.y")]
        public float speed_y => velocity.y;
        [ExprConst("is_dead")]
        public bool die => is_dead;
        [ExprConst("position.x")]
        public float position_x => position.x;
        [ExprConst("position.y")]
        public float position_y => position.y;
        //---
        public int max_hp;
        public int current_hp;
        public AutoCode.Tables.EnemyNormal.e_moveType move_type;
        public float move_speed;
        public int melee_atk;
        public (string, string) view_path;
        public float mass;
        public uint _type; //添加时输入的类型编号，方便debug
        public uint _id; //编号，方便debug

        //================================================================================================

        public EnemyMgr mgr;
        public EnemyBrain brain;
        public Device device;
        public IEnemy concrete;

        public Vector2 collider;
        public Vector2 collider_center => position + collider_offset;
        public Vector2 collider_offset;
        public Vector2 position;
        public Vector2 direction;

        public bool flip_x

        {
            get
            {
                if (direction.x >= 0) return false;
                else return true;
            }
        }

        public float impact_value_delta_decay = -Config.current.impact_a * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;

        public Vector2 velocity => concrete.active_v + concrete.impact_v + concrete.environment_v;

        bool be_showed => mgr.enemies[this] != null;//当前是否拥有外观
        bool in_show_area //当前是否位于显示区
        {
            get
            {
                var caravan_pos = mgr.caravan.logic_position;
                var show_area = Config.current.show_enemy_area;

                var dis = position - caravan_pos;
                dis.x = Mathf.Abs(dis.x);
                dis.y = Mathf.Abs(dis.y);

                return dis.x <= show_area.x && dis.y <= show_area.y;
            }
        }

        public bool is_dead = false;

        Vector2 IEnemy_Interaction_Target.position => position;
        Vector2 IEnemy_Interaction_Target.velocity => velocity;
        Vector2 IEnemy_Interaction_Target.collider => collider;
        string IEnemy_Interaction_Target.name => "Enemy";
        int IEnemy_Interaction_Target.current_hp { get => current_hp; set => current_hp = value; }
        Vector2 ITarget.position => collider_center;

        public EnemyView view;

        //================================================================================================


        public void init(EnemyMgr owner, EnemyBrain brain, Device device)
        {
            this.mgr = owner;
            this.brain = brain;

            switch (move_type)
            {
                case AutoCode.Tables.EnemyNormal.e_moveType.i_ground:
                    concrete = new Ground_E(this);
                    break;

                case AutoCode.Tables.EnemyNormal.e_moveType.i_fly:
                    concrete = new Sky_E(this);
                    break;
            }

            this.device = device;
            this.device.velocity = velocity;
            this.device.position = position;
            this.device.direction = direction;
            this.device.faction = Device.Faction.enemy;
        }


        public void on_reset_x()
        {
            position -= new Vector2(Config.current.reset_pos_intervel, 0);
        }


        void destroy()
        {
            var e = mgr.enemies;
            if (!e.ContainsKey(this)) return;

            mgr.cell_tick -= on_tick;
            BattleCaravanMgr.reset_x -= on_reset_x;

            mgr.destroy_cell(this);

            Debug.Log($"怪物 {_id} , 尸体销毁");
        }


        public void device_tick()
        {
            if (!is_dead) //规则：死亡状态，无法使用设备
            {
                device.position = position;
                device.direction = direction;
                device.velocity = velocity;
                device.tick();
            }
        }


        /// <summary>
        /// 逐帧运行
        /// </summary>
        public void on_tick()
        {
            if (!is_dead) //规则：死亡状态，锁行为树
                concrete.calc_active();

            concrete.calc_impacting();
            concrete.calc_environmet();
            concrete.upd_position(ref position);

            check_stash_or_pop();
        }


        /// <summary>
        /// 检测是否需要暂存或释放
        /// </summary>
        void check_stash_or_pop()
        {
            var be_showed = this.be_showed;
            var in_show_area = this.in_show_area;

            if (be_showed && !in_show_area) //暂存
            {
                mgr.stash_cell(this);
                return;
            }

            if (!be_showed && in_show_area) //释放
            {
                mgr.pop_cell(this);
                return;
            }
        }


        /// <summary>
        /// 死亡过程
        /// </summary>
        /// <returns></returns>
        IEnumerator dead_process(System.Action effect_action = null)
        {
            brain.on_dead();
            concrete.notify_on_dead(ref brain.velocity, ref brain.acc);
            mgr.on_dead(this); // 更新表现           
            Debug.Log($"怪物 {_id} , 已死亡");

            yield return new WaitForSeconds(Config.current.enemy_dead_time);
            
            destroy();
            effect_action?.Invoke();
        }


        /// <summary>
        /// 怪物死亡
        /// </summary>
        public void dead(System.Action effect_action = null)
        {
            is_dead = true;
            BattleSceneRoot.instance.StartCoroutine(dead_process(effect_action));
        }


        public void set_anm_state(int i)
        {
            if (view == null) return;
            view.anm.SetInteger("state", i);
        }


        #region 外部操作

        /// <summary>
        /// 被击退
        /// </summary>
        public void be_impacted(float dis, Vector2 dir)
        {
            if (dis <= 0) return; //规则: 如果击退距离不大于0，无事发生

            if (dir.magnitude != 1)
                dir = dir.normalized;

            concrete.notify_on_impact(dis, dir);
        }


        /// <summary>
        /// 被伤害
        /// </summary>
        public void be_damaged(int damage)
        {
            mgr.on_damage(this);

            current_hp -= damage;
            if (current_hp <= 0 && !is_dead) //规则：hp归0后，水平翻转，x秒后真正死亡
                dead();
        }
        #endregion

    }

}

