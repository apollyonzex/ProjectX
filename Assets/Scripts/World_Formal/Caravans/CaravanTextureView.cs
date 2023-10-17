using Foundation;
using Common_Formal;
using UnityEngine;
using World_Formal.Caravans;
using Common_Formal.DS;
using World_Formal.Caravans.Devices;
using System.Linq;

namespace Assets.Scripts.World_Formal.Caravans
{
    public class CaravanTextureView : MonoBehaviour, ICaravanView
    {
        public BoxCollider2D body_collider;
        public GameObject slot_model;
        public GameObject slots_model;

        CaravanMgr_Formal mgr;
        GameObject m_slots_node;
        Transform body => body_collider.transform;

        public SpineView spine_view;
        void IModelView<CaravanMgr_Formal>.attach(CaravanMgr_Formal mgr) { 
            this.mgr = mgr;
            body_collider.size = mgr.cell.colider_size;
            fresh();
        }

        void IModelView<CaravanMgr_Formal>.detach(CaravanMgr_Formal mgr)
        {
            this.mgr = null;
        }

        void ICaravanView.notify_on_change_anim(SpineDS ds)
        {
            if (spine_view == null) return;
            spine_view.set_anim(ds);
        }

        void ICaravanView.notify_on_device_changed()
        {
            DestroyImmediate(m_slots_node);             //每次刷新所有设备可能有点浪费
            fresh();                        
        }

        void ICaravanView.notify_on_tick()
        {
            
        }

        void IModelView<CaravanMgr_Formal>.shift(CaravanMgr_Formal old_owner, CaravanMgr_Formal new_owner)
        {
            
        }

        void fresh()
        {
            //车体动画
            if (spine_view != null)
                spine_view.set_anim(mgr.cell_spine_info);

            //创建slots节点
            if (m_slots_node == null)
            {
                m_slots_node = Instantiate(slots_model, transform);
                m_slots_node.SetActive(true);
            }

            //创建设备
            var deviceMgr = new DeviceMgr(Common.Config.DeviceMgr_Name, mgr, spine_view.anim.skeleton);

            foreach (var (slot, device) in mgr.slot_device_dic.Where(t => t.Value != null))
            {
                var slot_node = Instantiate(slot_model, m_slots_node.transform);
                var device_path = device.get_pfb_path(slot.type);

                if (EX_Utility.try_load_asset(device_path.Item1, device_path.Item2, out DeviceView e))
                {
                    var device_view = Instantiate(e, slot_node.transform);
                    deviceMgr.add_cell(device, slot, device_view);

                    var device_spine_view = device_view.spine_view;
                    if (device_spine_view != null)
                    {
                        if (mgr.device_spine_info_dic.TryGetValue(device._desc.f_id, out var spine_ds)) //临时，目前不是所有设备都有动画
                            device_spine_view.set_anim(spine_ds);
                    }
                }

                slot_node.transform.localPosition = slot.position;
                slot_node.transform.localRotation = slot.rotation;
                slot_node.gameObject.SetActive(true);
            }
        }
    }
}
