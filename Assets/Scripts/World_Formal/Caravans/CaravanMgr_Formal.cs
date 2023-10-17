using Common_Formal;
using Foundation;
using System.Collections.Generic;
using World_Formal.Caravans.Slots;
using UnityEngine;
using System.Linq;
using World_Formal.Caravans.Devices;
using Common;
using World_Formal.Helpers;
using Common_Formal.DS;
using World_Formal.CaravanBackPack;

namespace World_Formal.Caravans
{
    public interface ICaravanView : IModelView<CaravanMgr_Formal>
    {
        void notify_on_device_changed();

        void notify_on_tick();

        void notify_on_change_anim(SpineDS ds);
    }


    public class CaravanMgr_Formal : Model<CaravanMgr_Formal, ICaravanView>, IMgr
    {
        public WorldContext ctx;

        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public uint cell_id;
        Caravan m_cell;
        public Caravan cell => m_cell;

        public SpineDS cell_spine_info;
        public Vector2 scene_node_offset;

        public Dictionary<Slot, Device> slot_device_dic = new();
        public Dictionary<uint, SpineDS> device_spine_info_dic = new();
        public Dictionary<string, CaravanProperty> caravan_property  = new Dictionary<string, CaravanProperty>();

        public Collider2D collider;

        public BackPackMgr backpack;
        
        Caravan_Move_Helper m_move_helper;

        public Vector2 view_position
        {
            get
            {
                var pos = scene_node_offset;
                if (ctx == null) return pos;

                EX_Utility.calc_pos_from_rotate(ref pos, ctx.caravan_rad);
                return pos + ctx.caravan_pos;
            }
        }

        public Quaternion view_rotation
        {
            get
            {
                if (ctx == null) return Quaternion.identity;

                return EX_Utility.quick_look_rotation_from_left(ctx.caravan_dir);
            }
        }

        //==================================================================================================

        public CaravanMgr_Formal(string name, params object[] objs)
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
        }

        //==================================================================================================

        public void init(CarBody car)
        {
            m_cell = car.caravan;
            m_cell.mgr = this;
            cell_id = m_cell._desc.f_id;           

            foreach (var (k ,v) in car.caravan._desc.f_custom_float)
            {
                caravan_property.Add(k, new CaravanProperty()
                {
                    max = v.Item3,
                    value = v.Item1,
                    min = v.Item2,
                });
            }

            slot_device_dic.Clear();
            device_spine_info_dic.Clear();

            slot_device_dic = car.slot_device_dic;

            backpack = car.backPack;
            backpack.owner = this;
        }


        /// <summary>
        /// 增减设备，flag = 1 or -1
        /// </summary>
        void change_device(Slot slot, Device device, int flag = 1)
        {
            if (!device.is_valid) return;

            if (flag > 0)
            {
                slot_device_dic[slot] = device;
                device.type = slot.type;
            }
            else
            {
                if (slot == null) return;
                slot_device_dic[slot] = null;
            }
        }


        /// <summary>
        /// 加载默认view
        /// </summary>
        /// <param name="parent">view的父节点</param>
        /// <param name="cell_spine_info">车体spine信息</param>
        /// <param name="device_spine_info">设备spine信息</param>
        /// <param name="scene_offset">场景设定造成的偏移值(作用于设备)</param>
        /// <param name="is_offset_whole_view">偏移值是否作用于整个view</param>
        /// <returns>view</returns>
        public CaravanView init_default_view(Transform parent, SpineDS cell_spine_info, Dictionary<uint, SpineDS> device_spine_info, Vector3 scene_offset, bool is_offset_whole_view = false)
        {
            this.cell_spine_info = cell_spine_info;
            this.device_spine_info_dic = device_spine_info;
            this.scene_node_offset = scene_offset;

            EX_Utility.create_cell_in_scene<CaravanMgr_Formal, ICaravanView, CaravanView>(this, ("caravan", "prefabs/CaravanView"), parent, out var view);

            if (is_offset_whole_view)
                view.transform.localPosition += scene_offset;

            return view;
        }


        public bool TryGetDevice(Slot slot, out Device device)
        {
            slot_device_dic.TryGetValue(slot, out device);
            return device != null;
        }


        public bool TryGetSlot(Device device, out Slot slot)
        {
            slot = null;
            foreach (var (k, v) in slot_device_dic.Where(t => t.Value == device))
            {
                slot = k;
            }

            return slot != null;
        }


        public void add_device(Slot slot, Device device)
        {
            change_device(slot, device);

            foreach (var view in views)
            {
                view.notify_on_device_changed();
            }
        }


        public void remove_device(Slot slot, Device device)
        {
            change_device(slot, device, -1);

            foreach (var view in views)
            {
                view.notify_on_device_changed();
            }
        }


        public void shift_device(Slot t_slot, Device t_device, Slot s_slot, Device s_device)
        {
            change_device(t_slot, t_device, -1);
            change_device(s_slot, s_device, -1);
            change_device(t_slot, s_device);
            change_device(s_slot, t_device);

            foreach (var view in views)
            {
                view.notify_on_device_changed();
            }
        }


        /// <summary>
        /// 创建设备 - 加载到场景
        /// </summary>
        public void create_device_on_scene(CaravanView view)
        {
            var deviceMgr = new DeviceMgr(Config.DeviceMgr_Name, this, view.spine_view.anim.skeleton);

            foreach (var (slot, device) in slot_device_dic.Where(t => t.Value != null))
            {
                var slot_node = view.create_slot_node();
                var device_path = device.get_pfb_path(slot.type);

                if (EX_Utility.try_load_asset(device_path.Item1, device_path.Item2, out DeviceView e))
                {
                    var device_view = view.create_device_view(e, slot_node);
                    deviceMgr.add_cell(device, slot, device_view);

                    var device_spine_view = device_view.spine_view;
                    if (device_spine_view != null)
                    {
                        if (device_spine_info_dic.TryGetValue(device._desc.f_id, out var spine_ds)) //临时，目前不是所有设备都有动画
                            device_spine_view.set_anim(spine_ds);
                    }
                }

                slot_node.transform.localPosition = slot.position;
                slot_node.transform.localRotation = slot.rotation;
                slot_node.SetActive(true);
            }

            if (WorldSceneRoot.instance != null)
                deviceMgr.load_in_world_scene();
        }


        public void load_in_world_scene()
        {
            ctx = WorldContext.instance;
            ctx.add_tick(Config.CaravanMgr_Priority, Config.CaravanMgr_Name, tick);
            ctx.add_tick1(Config.CaravanMgr_Priority, Config.CaravanMgr_Name, tick1);

            m_move_helper = Caravan_Move_Helper.instance;
            m_move_helper.init(this);

            ctx.caravan_acc_status = Enum.EN_caravan_acc_status.braking; //规则：进入world时，刹车
            ctx.caravan_hp = cell._desc.f_hp;
        }


        void tick()
        {
            m_move_helper.move(ctx, this);
            m_move_helper.calc_and_set_caravan_leap_rad(ctx);

            //如果需要，重置pos
            if (ctx.is_need_reset)
            {
                ctx.caravan_pos.x -= ctx.reset_dis;
            }

            if (ctx.is_battle)
                ctx.battleDS.update_progress(ctx);
        }


        void tick1()
        {
            Anim_State_Helper.instance.set_state(ctx);

            foreach (var view in views)
            {
                view.notify_on_tick();

                //根据状态切换动画
                SpineDS ds;
                if (ctx.is_battle)
                    DB.instance.spineBattleCaravan_info.TryGetValue(cell_id, out ds);
                else
                    DB.instance.spineWorldCaravan_info.TryGetValue(cell_id, out ds);
                
                ds = Anim_State_Helper.instance.choose_and_load(ds, ctx);

                view.notify_on_change_anim(ds);
            }
        }

    }
}

