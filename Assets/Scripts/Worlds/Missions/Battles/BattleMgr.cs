using CaravanEnhanced;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Worlds.Missions.Battles
{
    public interface IBattleMgrView : IModelView<BattleMgr>
    {
        void notify_add_enemy_birth_icon(float ratio);
    }

    public class BattleMgr : Model<BattleMgr, IBattleMgrView>
    {
        public Caravan.BattleCaravanMgr caravan_mgr;
        public Devices.BattleDeviceMgr device_mgr;
        public Projectiles.ProjectileMgr projectile_mgr;
        public Enemies.EnemyMgr enemyMgr;
        public Effects.BattleEffectMgr effectMgr;
        public CameraShot.Focus focus;

        bool m_battle_result = false;
        event System.Action<bool> m_callback;

        List<float> m_enemy_birth_nodes = new();//怪物刷新点
        float m_next_enemy_birth_node => m_enemy_birth_nodes[0];

        Dictionary<float, ProcessEditor.ProcessAsset.Enemy[]> m_enemy_data = new();

        //==================================================================================================


        internal void init_expand(Action<bool> callback)
        {
            m_callback = callback;
        }


        internal void fini_expand()
        {
            m_callback?.Invoke(m_battle_result);
        }


        internal void start_battle_expand(CaravanMgr caravan_raw_data)
        {
            var root = BattleSceneRoot.instance;

            projectile_mgr = new();

            caravan_mgr = new(caravan_raw_data, this);
            caravan_mgr.init_device_mgr(out device_mgr);

            enemyMgr = new(caravan_mgr.caravan);

            effectMgr = new();

            focus = root.focus;
            focus.init(caravan_mgr.caravan);

            root.start_battle(this);
        }


        public void end_battle_expand(bool bl)
        {
            m_battle_result = bl;

            BattleSceneRoot.instance.battlecardMgr.clear_views();
            BattleSceneRoot.instance.battlecardMgr.EndDuel();

            BattleSceneRoot.instance.end_battle();
            WorldState.instance.mission.end_battle(BattleSceneRoot.instance.gameObject.scene);
        }


        /// <summary>
        /// 逻辑帧的tick管理
        /// 位于物理同步之前
        /// 顺序：车、敌人、设备、子弹、镜头
        /// </summary>
        public void physics_tick()
        {
            caravan_mgr.on_physics_tick();
            enemyMgr.on_physics_tick();
            device_mgr.on_physics_tick();       
            projectile_mgr.on_physics_tick();
            
            focus.on_physics_tick();
        }


        /// <summary>
        /// 从配置文件中，获取战场信息
        /// </summary>
        public void get_battlefield_info(out float length)
        {
            Common.Expand.Utility.try_load_asset("process", "process_data", out ProcessEditor.ProcessAsset asset);
            length = asset.length;

            var groups = asset.groups;
            m_enemy_birth_nodes.Clear();
            m_enemy_data.Clear();
            foreach (var e in groups)
            {
                var trigger_pos = e.trigger_pos;
                m_enemy_birth_nodes.Add(trigger_pos);
                m_enemy_data.Add(trigger_pos, e.enemies);

                var ratio = e.icon_pos / length;
                foreach (var view in views)
                {
                    view.notify_add_enemy_birth_icon(ratio);
                }
            }
            m_enemy_birth_nodes.Sort();
        }


        public Vector2 calc_enemy_icon_pos(float ratio, Vector2 start, Vector2 end)
        {
            var x = Mathf.Abs(end.x - start.x) * ratio + start.x;
            var y = start.y;
            return new Vector2(x, y);
        }
     

        /// <summary>
        /// 检测触发：怪物刷新
        /// </summary>
        public void vaild_enemy_birth_trigger(float lx)
        {
            if (!m_enemy_birth_nodes.Any()) return;
            if (lx < m_next_enemy_birth_node) return;

            var enemies = m_enemy_data[m_next_enemy_birth_node];
            foreach (var cell in enemies)
            {
                enemyMgr.add_cell(cell.id, new UnityEngine.Vector2(cell.x, cell.y));
            }

            m_enemy_birth_nodes.RemoveAt(0);
        }
    }
}


