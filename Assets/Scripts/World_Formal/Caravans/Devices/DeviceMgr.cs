using Common_Formal;
using Foundation;
using System.Collections.Generic;
using World_Formal.Caravans.Slots;
using UnityEngine;
using Common;
using Common_Formal.DS;
using Common_Formal.Helpers;
using World_Formal.Helpers;
using World_Formal.BattleSystem.DeviceGraph;
using World_Formal.BattleSystem.Device;

namespace World_Formal.Caravans.Devices
{
    public interface IDeviceView : IModelView<DeviceMgr>
    {
        void notify_on_tick1(Vector2 pos);

        void notify_on_change_anim(SpineDS ds);

        Device cell { get; set; }
    }


    public class DeviceMgr : Model<DeviceMgr, IDeviceView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public WorldContext ctx;
        public CaravanMgr_Formal caravan_mgr;

        Dictionary<Device, GraphDevice> m_device_graph = new();

        Spine.Skeleton m_sk;
        Dictionary<Device, (Slot, IDeviceView)> m_cell_dic = new();

        //==================================================================================================

        public DeviceMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }


        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }


        void IMgr.init(object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            caravan_mgr = (CaravanMgr_Formal)objs[0];
            m_sk = (Spine.Skeleton)objs[1];

        }


        public void load_in_world_scene()
        {
            ctx = WorldContext.instance;
            ctx.add_tick(Config.DeviceMgr_Priority, Config.DeviceMgr_Name, tick);
            ctx.add_tick1(Config.DeviceMgr_Priority, Config.DeviceMgr_Name, tick1);
        }


        Vector2 calc_view_pos(Slot slot)
        {
            var bone = m_sk.FindBone(slot.bone_name);
            var pos = new Vector2(bone.WorldX, bone.WorldY) - slot.bone_pos + slot.position + caravan_mgr.cell.body_spine_offset + caravan_mgr.scene_node_offset;

            if (ctx != null)
                EX_Utility.calc_pos_from_rotate(ref pos, ctx.caravan_rad);

            return pos;
        }


        public void un_use_tick()
        {
            foreach (var (device, (slot, view)) in m_cell_dic)
            {
                var pos = calc_view_pos(slot);
                view.notify_on_tick1(pos);
            }
        }


        void tick()
        {
            foreach(var (device,graphdevice)  in m_device_graph)
            {
                graphdevice.device.direction = device.direction;
                graphdevice.device.position = device.position;
                graphdevice.tick();
            }
        }

        void tick1()
        {
            foreach (var (device, (slot, view)) in m_cell_dic)
            {
                var pos = calc_view_pos(slot) + ctx.caravan_pos;
                view.notify_on_tick1(pos);
                device.position = pos;

                //根据状态切换动画
                SpineDS ds;
                var cell_id = device._desc.f_id;
                if (ctx.is_battle)
                {
                    if (!DB.instance.spineBattleDevice_info.TryGetValue(cell_id, out ds)) continue;
                }
                else
                {
                    if (!DB.instance.spineWorldDevice_info.TryGetValue(cell_id, out ds)) continue;
                }

                ds = Anim_State_Helper.instance.choose_and_load(ds, ctx);
                view.notify_on_change_anim(ds);
            }
        }


        public void add_cell(Device device, Slot slot, IDeviceView view)
        {
            m_cell_dic.Add(device, (slot, view));
            view.cell = device;
            add_view(view);
        }


        public void remove_cell(Device device)
        {
            var view = m_cell_dic[device].Item2; 
            m_cell_dic.Remove(device);
            remove_view(view);
        }


        public void remove_cell(IDeviceView view)
        {
            m_cell_dic.Remove(view.cell);
            remove_view(view);
        }

        public void start_battle()      //只有在开始战斗的时候才会初始化图流
        {
            foreach (var (device, _) in m_cell_dic)
            {

                EX_Utility.try_load_asset(device._desc.f_graph_path.Item1, device._desc.f_graph_path.Item2, out DeviceGraphAsset asset);
                if (asset == null)
                    continue;
                var gd = new GraphDevice();
                gd.init(asset);
                gd.start();
                m_device_graph.Add(device, gd);
            }
        }
    }
}

