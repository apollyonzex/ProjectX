using CaravanEnhanced;
using UnityEngine;
using Foundation;
using System.Collections.Generic;
using Common;
using Devices;
using Worlds.CardSpace;

namespace Worlds.Missions.Battles.Caravan
{
    public enum Accstatus
    {
        driving,
        braking,
    }

    public enum Liftoffstatus
    {
        ground,
        sky,
    }

    public enum Glidestatus
    {
        ready,
        not_ready,
    }


    public interface IBattleCaravanView : IModelView<BattleCaravanMgr>
    {
        void on_modify_physics_tick();

        void on_modify_init();

        BattleCaravan cell { get; }
    }


    public class BattleCaravanMgr : Model<BattleCaravanMgr, IBattleCaravanView>
    {
        BattleMgr m_battleMgr;

        public BattleCaravan caravan => m_caravan;
        BattleCaravan m_caravan;
        CaravanMgr m_rawdata;

        public float body_height_offset;
        public List<Slot> slots = new();

        double m_lx = 0; //逻辑坐标的x
        int m_lcount = 0;

        float m_ux = 0; //u3d坐标的x

        int t_peak;
        int t_floating;

        bool is_land => caravan.logic_position.y == 0;
        bool no_gravity => is_land || (t_peak == 0);

        public static event System.Action reset_x;

        public Dictionary<string, Vector2> bone_pos_dic = new();

        bool m_is_spurt = false;

        //==================================================================================================


        /// <summary>
        /// 初始化战斗大篷车数据
        /// 从外围数据加载
        /// </summary>
        /// <param name="data"></param>
        public BattleCaravanMgr(CaravanMgr raw, BattleMgr battleMgr)
        {
            m_rawdata = raw;

            m_caravan = new();
            m_caravan.logic_position = Vector2.zero;
            m_caravan.max_hp_limit = raw.body_data.hp;
            m_caravan.current_hp = m_caravan.max_hp_limit;

            m_caravan.acc_status = Accstatus.driving;
            m_caravan.liftoff_status = Liftoffstatus.ground;
            m_caravan.glide_status = Glidestatus.not_ready;

            m_caravan.velocity = Vector2.zero;
            m_caravan.driving_speed_limit = raw.driving_speed_limit;
            m_caravan.descend_speed_limit = raw.descend_speed_limit;
            m_caravan.driving_acc = raw.driving_acc;
            m_caravan.braking_acc = raw.braking_acc;

            try_get_data(out body_height_offset);
            m_caravan.init(this);

            slots = raw.slots;

            BattleSceneRoot.instance.create_caravan_view(this, m_caravan, "caravan", raw.body_prefab_path, out var view);
            view.on_modify_init();

            m_battleMgr = battleMgr;
        }


        bool try_get_data(out float body_height_offset)
        {
            foreach (var slot in m_rawdata.slots)
            {
                if (slot.type == AutoCode.Tables.Item.e_slotType.i_车轮)
                {
                    //高度补正 = 轮子半径 + 车中心点到轮子slot的相对y的长度
                    body_height_offset = slot.item.height + Mathf.Abs(slot.position.y);
                    return true;
                }
            }

            body_height_offset = 0;
            return false;
        }


        public void init_device_mgr(out Devices.BattleDeviceMgr device_mgr)
        {
            device_mgr = new();
            device_mgr.init(this);

            foreach (var slot in slots)
            {
                var item = slot.item;
                if (item == null) continue;

                int e = item.descend_speed_limit;
                if (e < 0)
                {
                    m_caravan.descend_speed_limit_glide = e; //规则：滑翔时，下落下限取对应设备的值
                    break;
                }
            }
        }


        /// <summary>
        /// 逻辑帧执行
        /// </summary>
        public void on_physics_tick()
        {
            upd_logic();
            upd_view();
        }


        /// <summary>
        /// 更新逻辑
        /// </summary>
        void upd_logic()
        {
            upd_pos();
            upd_environment();
            m_battleMgr.vaild_enemy_birth_trigger((float)m_lx);

            
        }


        #region 更新位置 
        void upd_pos()
        {
            if (caravan.velocity == Vector2.zero)
                set_anm_state(BattleCaravan_Enum.Main_State.Idle);

            if (caravan.liftoff_status == Liftoffstatus.ground) //位置：地面
            {
                if (caravan.acc_status == Accstatus.driving) //加速：行驶
                {
                    set_anm_state(BattleCaravan_Enum.Main_State.Run);
                    upd_x(caravan.driving_acc_readonly, true, vx_opr_driving);
                }

                if (caravan.acc_status == Accstatus.braking) //加速：制动
                {
                    set_anm_state(BattleCaravan_Enum.Main_State.Brake);
                    upd_x(caravan.braking_acc_readonly, true);
                }
            }

            if (caravan.liftoff_status == Liftoffstatus.sky) //位置：飞行
            {
                if (caravan.glide_status == Glidestatus.ready) //滑翔：已就绪
                {
                    upd_x(caravan.driving_acc_readonly, false);
                    upd_y(caravan.descend_speed_limit_glide_readonly);
                }

                if (caravan.glide_status == Glidestatus.not_ready) //滑翔：未就绪
                {
                    upd_x(caravan.driving_acc_readonly, false);
                    upd_y(caravan.descend_speed_limit_readonly);
                }
            }

            if (m_is_spurt)
            {
                set_anm_state(BattleCaravan_Enum.Main_State.Spurt);
                m_is_spurt = false;
            }
        }


        void upd_x(float acc, bool need_cul_acc, System.Func<float, float> vx_opr = null)
        {
            var dv = acc * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;
            if (!need_cul_acc) dv = 0;

            var v = caravan.velocity.x + dv;

            if (vx_opr != null)
                v = vx_opr.Invoke(v);

            var ds = v * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;
            lock_logic_back(ds, ref v);

            m_ux = (float)(m_lx % Config.current.reset_pos_intervel);
            var count = Mathf.CeilToInt((float)(m_lx / Config.current.reset_pos_intervel));
            if (count != m_lcount) reset_x?.Invoke();
            m_lcount = count;

            caravan.velocity.x = v;
            caravan.logic_position.x = m_ux;
        }


        void upd_y(float descend_speed_limit)
        {
            if (t_peak > 0)
            {
                t_peak--;
            }
            else if (t_peak == 0)
            {
                t_floating--;
                if (t_floating > 0) return;
                t_peak = -1;
            }
            else
            {
                caravan.velocity.y = Mathf.Max(caravan.velocity.y, descend_speed_limit);
            }

            var dy = caravan.velocity.y * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;
            caravan.logic_position.y += dy;

            var y = caravan.logic_position.y;
            vaild_land(ref y);
            caravan.logic_position.y = y;
        }


        /// <summary>
        /// dirving时，对速度的x进行处理
        /// </summary>
        float vx_opr_driving(float x)
        {
            return Mathf.Clamp(x, Config.current.driving_speed_limit_min, caravan.driving_speed_limit_readonly);
        }


        /// <summary>
        /// focus永远不会后退
        /// </summary>
        /// <param name="ds"></param>
        void lock_logic_back(float ds, ref float v)
        {
            var old_lx = m_lx;
            m_lx += ds;
            if (m_lx < old_lx)
            {
                m_lx = old_lx;
                v = 0;
            }
        }


        /// <summary>
        /// 着陆检查
        /// </summary>
        void vaild_land(ref float y)
        {
            if (y <= 0)//着陆
            {
                y = 0;
                m_caravan.liftoff_status = Liftoffstatus.ground;
                m_caravan.glide_status = Glidestatus.not_ready;
                set_anm_state(BattleCaravan_Enum.Main_State.Land);

                //设备相关
                foreach (var device in m_battleMgr.device_mgr.devices)
                {
                    var battle_device = device.Key;
                    battle_device.device.try_get_component<CaravanTrigger>("landing", out var component);
                    if (component != null)
                    {
                        component.trigger(battle_device.device.ctx);
                    }
                }

                //镜头震动
                CameraShot.Focus.set_shake_level(Mathf.Max(0, Mathf.Abs(caravan.velocity.y) - 10));
                m_caravan.land_velocity = m_caravan.velocity;
                m_caravan.velocity.y = 0;

                //范围击杀
                var results = Physics2D.OverlapAreaAll(caravan.position + m_caravan.collider / 2, caravan.position - m_caravan.collider / 2);
                foreach (var r in results)
                {
                    if (r.TryGetComponent<Enemies.IEnemyView>(out var view))
                    {
                        var cell = view.cell;
                        cell.dead(() => {
                            var effect_mgr = WorldState.instance.mission.battleMgr.effectMgr;
                            effect_mgr.add_blood_effect(cell.position);
                        });
                    }
                }
            }
        }
        #endregion


        /// <summary>
        /// 更新环境作用力
        /// </summary>
        void upd_environment()
        {
            if (no_gravity) return;

            caravan.velocity.y += Config.current.gravity * BattleSceneRoot.PHYSICS_TICK_DELTA_TIME;
        }


        /// <summary>
        /// 更新外观
        /// </summary>
        void upd_view()
        {
            foreach (var view in views)
            {
                view.on_modify_physics_tick();
            }
        }


        /// <summary>
        /// 获取进度
        /// </summary>
        public void get_process(out double now ,out double all, out string percentage)
        {
            now = m_lx;
            all = BattleSceneRoot.instance.battlefield_length;

            var e = string.Format("{0:F}", m_lx / all * 100);
            percentage = $"{e}%";
        }


        /// <summary>
        /// 设置动画状态
        /// </summary>
        public void set_anm_state(BattleCaravan_Enum.Main_State state)
        {
            m_caravan.main_state = state;
            m_caravan.anm.SetInteger("state", (int)state);
        }


        #region 外部行为
        public void jump(float height)
        {            
            caravan.velocity.y = Mathf.Sqrt(2 * height * -Config.current.gravity);
            t_peak = (int)Mathf.Floor(Mathf.Sqrt(2 * height / -Config.current.gravity) * BattleSceneRoot.PHYSICS_TICKS_PER_SECOND);
            t_floating = Config.current.t_floating;
            m_caravan.liftoff_status = Liftoffstatus.sky;
            set_anm_state(BattleCaravan_Enum.Main_State.Jump);         

            //解除所有顶住的怪物
            var enemy_mgr = WorldState.instance.mission.battleMgr.enemyMgr;
            enemy_mgr.leave_holding();
        }


        public void glide(bool bl)
        {
            if (bl)
                caravan.glide_status = Glidestatus.ready;
            else
                caravan.glide_status = Glidestatus.not_ready;
        }


        public void brake()
        {
            caravan.acc_status = Accstatus.braking;
        }


        public void drive()
        {
            caravan.acc_status = Accstatus.driving;
        }


        public void add_driving_limit(int add)
        {
            caravan.driving_speed_limit += add;
        }


        public void add_driving_acc(int acc)
        {
            caravan.driving_acc += acc;
        }


        public void add_braking_acc(int acc)
        {
            caravan.braking_acc += acc;
        }


        public void add_velocity(float x)
        {
            caravan.velocity.x += x;
        }

        public void is_spurt()
        {
            m_is_spurt = true;
        }
        #endregion
    }

}

