using System.Collections.Generic;
using System.Linq;
using World_Formal.Caravans;
using World_Formal.Caravans.Slots;
using World_Formal.Caravans.Devices;
using UnityEngine;

namespace World_Formal.CaravanBackPack
{
    /// <summary>
    /// 拿下/安装设备的辅助类
    /// </summary>
    public class Change_Device_Helper
    {
        public Dictionary<Slot, Slot_Info_View> views = new();
        public (Slot, Device) holding;

        public CaravanMgr_Formal mgr;
        List<Slot> m_access_slots = new();

        //==================================================================================================

        public Change_Device_Helper(CaravanMgr_Formal mgr)
        {
            this.mgr = mgr;
        }


        /// <summary>
        /// 获得符合条件的所有slot
        /// </summary>
        public bool TryGetSlots(Device device, out List<Slot> slots)
        {
            slots = new();
            foreach (var (type, e1) in device._desc.f_slot_and_prefeb)
            {
                foreach (var (slot, e2) in mgr.slot_device_dic.Where(t => t.Key.type == type))
                {
                    slots.Add(slot);
                }
            }

            if (slots.Any()) return true;
            return false;
        }


        public void on_slot_label_click(Slot slot)
        {
            if (holding.Item2 == null) //拿起设备
            {
                if (!mgr.TryGetDevice(slot, out var d1)) return; //目标无设备，拿起失败
                
                if (slot.type == AutoCode.Tables.Device.e_slotType.i_车轮) //规则：车轮不可拿起
                {
                    Debug.Log("车轮不可拿起");
                    return;
                }

                //拿起成功
                TryGetSlots(d1, out m_access_slots);
                holding = (slot,d1);

                foreach (var (k, view) in views.Where(t => m_access_slots.Contains(t.Key)))
                {
                    view.do_highlight(true);
                }
                return;
            }

            //放下设备
            if (!m_access_slots.Contains(slot)) return; //无效区，无事发生

            if (mgr.TryGetDevice(slot, out var d2)) //目标有设备
            {
                TryGetSlots(d2, out var t_access_slots);
                if (t_access_slots.Contains(holding.Item1)) //如果目标设备可以放到拿取设备的位置，交换
                {
                    mgr.shift_device(slot, d2, holding.Item1, holding.Item2);
                    no_holding();
                }
                else //放下设备，并拿取目标设备
                {
                    mgr.remove_device(slot, d2);
                    mgr.remove_device(holding.Item1, holding.Item2);
                    
                    mgr.add_device(slot, holding.Item2);
                    has_holding(d2);
                }
            }
            else //目标无设备，移动
            {
                mgr.remove_device(holding.Item1, holding.Item2);
                mgr.add_device(slot, holding.Item2);
                no_holding();
            } 
        }


        public void has_holding(Device device)
        {
            holding = (null, device);
            TryGetSlots(device, out m_access_slots);
            foreach (var (k, view) in views)
            {
                if (m_access_slots.Contains(k))
                    view.do_highlight(true);
                else
                    view.do_highlight(false);
            }
        }


        public void no_holding()
        {
            //放下成功，无拿取
            holding.Item1 = null;
            holding.Item2 = null;
            m_access_slots.Clear();
            foreach (var (k, view) in views)
            {
                view.do_highlight(false);
            }
        }
    }
}

