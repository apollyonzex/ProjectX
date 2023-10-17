using DeviceGraph;
using Devices;
using Foundation;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Worlds.Missions.Battles.Devices
{
    public interface IBattleDeviceView : IModelView<BattleDeviceMgr>
    {
        void on_modify_physics_tick();
    }


    public class BattleDeviceMgr : Model<BattleDeviceMgr, IBattleDeviceView>
    {
        public Dictionary<BattleDevice, IBattleDeviceView> devices => m_devices;
        Dictionary<BattleDevice, IBattleDeviceView> m_devices;

        public Caravan.BattleCaravanMgr caravan_mgr => m_caravan_mgr;
        Caravan.BattleCaravanMgr m_caravan_mgr;
        Caravan.BattleCaravan caravan;

        public System.Action cell_tick;

        public BattleDeviceView m_wheel;

        //================================================================================================


        public void init(Caravan.BattleCaravanMgr caravan_mgr)
        {
            m_caravan_mgr = caravan_mgr;
            caravan = m_caravan_mgr.caravan;
            m_devices = new();

            var skeleton = caravan.sm.skeleton;

            foreach (var slot in caravan_mgr.slots)
            {
                var item = slot.item;
                if (item == null) continue;     

                BattleDevice cell = new();
                cell.position = caravan.position + slot.position;
                cell.direction = caravan.direction + slot.direction;
                cell.slot_position = slot.position;
                cell.slot_direction = slot.direction;

                if (slot.bone_name != null)
                    cell.bone = skeleton.FindBone(slot.bone_name);

                Device device = new();
                cell.item = item;
                device.default_damage = item.damage;
                cell.device = device;
                cell.init(this);

                var view_path = item.item_battle_paths[slot.type];
                BattleSceneRoot.instance.create_device_view(this, cell, view_path.Item1, view_path.Item2, out var view);

                var view_temp = view.GetComponent<DeviceViews.DeviceView>();//DeviceView待重构
                if (view_temp != null)
                {
                    var graph_asset = Common.Utility.load_asset<DeviceGraphAsset>("caravan", item.graph_path);
                    if (graph_asset == null) 
                        Debug.LogWarning($"设备{view.gameObject.name}的graph路径错误");
                    device.init(graph_asset, view_temp.GetComponents<DeviceConfig>());
                    device.start();
                    view_temp.init(device);
                }

                if (item.tags.Contains("wheel"))
                    m_wheel = view;
            }
        }


        internal void on_physics_tick()
        {
            upd_logic();
            cell_tick?.Invoke();
            upd_view();
        }


        void upd_logic()
        {
            foreach (var info in devices)
            {
                var cell = info.Key;

                var bone = cell.bone;
                if (bone != null)
                {
                    caravan_mgr.bone_pos_dic.TryGetValue(bone.Data.Name, out var pos);
                    cell.position = caravan.position + new UnityEngine.Vector2(bone.WorldX, bone.WorldY) + cell.slot_position - pos;

                    //cell.position = caravan.position + new UnityEngine.Vector2(bone.WorldX, bone.WorldY) + caravan.anm_pos_offset;
                }
                else 
                {
                    cell.position = caravan.position + cell.slot_position;
                }
            }
        }


        void upd_view()
        {
            foreach (var view in views)
            {
                view.on_modify_physics_tick();
            }
        }
    }
}
