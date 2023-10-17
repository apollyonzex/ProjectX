
using Common;
using Devices;
using Foundation;
using Foundation.Tables;
using System;
using System.Collections.Generic;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Enemies.BehaviourTrees;


namespace Worlds.Missions.Battles.Enemies
{
    public interface IEnemyView : IModelView<EnemyMgr>
    {
        void on_modify_physics_tick();
        void on_destroy();
        Enemy cell { get; }
        void notify_on_damaged();
        void notify_on_dead_process();
    }


    public class EnemyMgr : Model<EnemyMgr, IEnemyView>
    {
        public System.Action cell_tick;

        public Dictionary<uint, AutoCode.Tables.EnemyNormal.Record> raw_data => m_raw_data;
        Dictionary<uint, AutoCode.Tables.EnemyNormal.Record> m_raw_data;

        public Dictionary<Enemy, IEnemyView> enemies = new();

        public Caravan.BattleCaravan caravan;

        List<Enemy> m_holding_cells = new();
        List<Enemy> m_boarding_cells = new();

        public List<Enemy> holding_cells => m_holding_cells;
        public List<Enemy> boarding_cells => m_boarding_cells;

        public uint max_id = 0;//当前最大的编号

        //================================================================================================


        public EnemyMgr(Caravan.BattleCaravan caravan)
        {
            this.caravan = caravan;
            tryGetData_from_db(ref m_raw_data);
        }


        /// <summary>
        /// 添加敌人
        /// </summary>
        /// <param name="id">对应raw_data的f_id</param>
        /// <param name="pos">相对于车的坐标的位置</param>
        public string add_cell(uint id, Vector2 pos)
        {
            var data = m_raw_data[id];

            Enemy cell = new();
            cell._id = max_id++;
            cell._type = id;
            cell.max_hp = data.f_hp;
            cell.current_hp = cell.max_hp;
            cell.move_type = data.f_move_type;
            cell.move_speed = data.f_move_speed;
            cell.melee_atk = data.f_atk;
            cell.mass = data.f_mass;

            pos += caravan.logic_position;
            var move_type = data.f_move_type.value;
            if (move_type == AutoCode.Tables.EnemyNormal.e_moveType.i_ground)//地面单位，出生在地面
                pos.y = 0;
            else//其他单位，出生在地面之上
            {
                var y = pos.y;
                y = y < 0 ? 0 : y;
                pos.y = y;
            }
            cell.position = pos;

            if (pos.x <= 0)//简单设定朝向，目前只分左右
                cell.direction = Vector2.right;
            else
                cell.direction = Vector2.left;

            var view_path = cell.view_path = data.f_view;
            BattleSceneRoot.instance.create_enemy_view(this, cell, view_path.Item1, view_path.Item2, out var view);

            //怪物设备
            Device device = new();
            var graph_path = data.f_missile_atk_graph;
            if (graph_path.Item1.Length > 0)//允许不配置设备
            {
                var view_temp = view.GetComponent<DeviceViews.DeviceView>();

                var graph_asset = Common.Utility.load_asset<DeviceGraph.DeviceGraphAsset>(graph_path.Item1, graph_path.Item2);
                if (graph_asset == null) Debug.LogWarning($"怪物{view.gameObject.name}的graph路径错误");

                device.init(graph_asset, view_temp.GetComponents<DeviceConfig>());
                device.start();
                view_temp.init(device);
            }

            //怪物brain
            EnemyBrain brain = new();
            cell.init(this, brain, device);
            brain.init(cell, data.f_behaviour_tree.Item1, data.f_behaviour_tree.Item2);

            cell_tick += cell.on_tick;
            BattleCaravanMgr.reset_x += cell.on_reset_x;

            return cell._id.ToString();
        }


        /// <summary>
        /// 销毁敌人
        /// </summary>
        public void destroy_cell(Enemy cell)
        {
            if (!try_get_view(cell, out var view)) return;

            if (view != null)
            {
                remove_view(view);
                view.on_destroy();
            }           

            if (enemies.ContainsKey(cell))
                enemies.Remove(cell);
        }


        /// <summary>
        /// 暂存敌人: 超出指定范围时触发
        /// 去除外观, 仅保留数据
        /// </summary>
        public void stash_cell(Enemy cell)
        {
            var view = enemies[cell];
            remove_view(view);
            view.on_destroy();

            enemies[cell] = null;
            Debug.Log($"怪物 {cell._id} , 离开视野");
        }


        /// <summary>
        /// 释放敌人: 进入指定范围时触发
        /// </summary>
        public void pop_cell(Enemy cell)
        {
            var view_path = cell.view_path;
            BattleSceneRoot.instance.create_enemy_view(this, cell, view_path.Item1, view_path.Item2, out var view);
            Debug.Log($"怪物 {cell._id} , 进入视野");
        }


        internal void on_physics_tick()
        {
            upd_logic();
            cell_tick?.Invoke();
            upd_view();
        }


        void upd_logic()
        {
            

        }


        void upd_view()
        {
            foreach (var view in views)
            {
                view.on_modify_physics_tick();
            }
        }


        /// <summary>
        /// 读表获取
        /// </summary>
        void tryGetData_from_db(ref Dictionary<uint, AutoCode.Tables.EnemyNormal.Record> raw_data)
        {
            raw_data = new();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "enemy_normal", out var asset);

            AutoCode.Tables.EnemyNormal enemy = new();
            enemy.load_from(asset);

            foreach (var rec in enemy.records)
            {
                raw_data.Add(rec.f_id, rec);
            }
        }


        /// <summary>
        /// 解除所有状态
        /// </summary>
        public void leave_all_state(Enemy cell)
        {
            leave_holding(cell);
            leave_boarding(cell);
        }


        /// <summary>
        /// 解除顶车状态
        /// </summary>
        public void leave_holding(Enemy cell = null)
        {
            if (cell == null)
            {
                var count = m_holding_cells.Count;
                caravan.driving_speed_limit += count * Config.current.passenger_driving_speed_limit_modify;
                caravan.driving_acc += count * Config.current.passenger_driving_acc_modify;

                foreach (var e in m_holding_cells)
                {
                    e.brain.main_state = EnemyBrain_Enum.Main_State.Idle;
                }
                m_holding_cells.Clear();
            }
            else
            {
                if (!m_holding_cells.Contains(cell)) return;

                caravan.driving_speed_limit += Config.current.passenger_driving_speed_limit_modify;
                caravan.driving_acc += Config.current.passenger_driving_acc_modify;

                m_holding_cells.Remove(cell);
                for (int i = 0; i < m_holding_cells.Count; i++)
                {
                    var e = m_holding_cells[i];
                    var pos = caravan.logic_position;
                    pos.x += Config.current.passenger_queue_padding + i * Config.current.passenger_queue_spacing;
                    e.position = pos;
                }

                cell.brain.main_state = EnemyBrain_Enum.Main_State.Idle;
            }
        }


        /// <summary>
        /// 增加顶车状态
        /// </summary>
        public void add_holding(Enemy cell)
        {
            if (m_holding_cells.Contains(cell)) return;

            var count = m_holding_cells.Count;
            if (count >= Config.current.passenger_num_max) return;

            var pos = caravan.logic_position;
            pos.x += Config.current.passenger_queue_padding + count * Config.current.passenger_queue_spacing;
            cell.position = pos;

            caravan.driving_speed_limit -= Config.current.passenger_driving_speed_limit_modify;
            caravan.driving_acc -= Config.current.passenger_driving_acc_modify;

            m_holding_cells.Add(cell);
            cell.brain.main_state = EnemyBrain_Enum.Main_State.Holding;
        }


        /// <summary>
        /// 解除扒车状态
        /// </summary>
        public void leave_boarding(Enemy cell)
        {
            if (!m_boarding_cells.Contains(cell)) return;

            caravan.driving_speed_limit += Config.current.boarder_driving_speed_limit_modify;
            caravan.driving_acc += Config.current.boarder_driving_acc_modify;

            m_boarding_cells.Remove(cell);
            cell.brain.main_state = EnemyBrain_Enum.Main_State.Idle;
        }


        /// <summary>
        /// 尝试扒车
        /// </summary>
        public bool try_add_boarding(Enemy cell)
        {
            foreach (var other in m_boarding_cells)
            {
                var bl = Utility.check_contain_two_rectangleArea(cell.position, cell.collider, other.position, other.collider);
                if (bl) return false;//检测到其他已经扒车的敌人，所以无法扒车
            }

            caravan.driving_speed_limit -= Config.current.boarder_driving_speed_limit_modify;
            caravan.driving_acc -= Config.current.boarder_driving_acc_modify;
            m_boarding_cells.Add(cell);

            return true;
        }


        internal bool try_get_view(Enemy cell,out IEnemyView view)
        {
            view = null;
            if (!enemies.ContainsKey(cell)) return false;

            view = enemies[cell];
            return true;
        }


        internal void on_damage(Enemy cell)
        {
            if (!try_get_view(cell, out var view)) return;
            view.notify_on_damaged();
        }


        public bool try_get_cell_from_id(int id, out Enemy enemy)
        {
            foreach (var e in enemies)
            {
                if (e.Key._id == (uint)id)
                {
                    enemy = e.Key;
                    return true;
                }
            }

            enemy = null;
            return false;
        }


        public void on_dead(Enemy cell)
        {
            if (!try_get_view(cell, out var view)) return;
            if (view == null) return;
            view.notify_on_dead_process();
        }        

    }

}

