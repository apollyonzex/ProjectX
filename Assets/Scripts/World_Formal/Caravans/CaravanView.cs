using Common_Formal;
using Common_Formal.DS;
using Foundation;
using UnityEngine;
using World_Formal.Caravans.Devices;

namespace World_Formal.Caravans
{
    public class CaravanView : MonoBehaviour, ICaravanView
    {
        public Transform body;
        public GameObject slot_model;
        public GameObject slots_model;

        CaravanMgr_Formal mgr;
        GameObject m_slots_node;

        public SpineView spine_view;
        public Collider2D collider_box;

        //==================================================================================================

        void IModelView<CaravanMgr_Formal>.attach(CaravanMgr_Formal mgr)
        {
            this.mgr = mgr;
            mgr.collider = collider_box;

            fresh();
        }


        void IModelView<CaravanMgr_Formal>.detach(CaravanMgr_Formal mgr)
        {
            this.mgr = null;
        }


        void IModelView<CaravanMgr_Formal>.shift(CaravanMgr_Formal old_mgr, CaravanMgr_Formal new_mgr)
        {
        }


        void ICaravanView.notify_on_device_changed()
        {
            DestroyImmediate(m_slots_node);
            fresh();
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
            mgr.create_device_on_scene(this);
        }


        void ICaravanView.notify_on_tick()
        {
            body.position = EX_Utility.v2_cover_v3(mgr.view_position, body.position);
            body.localRotation = mgr.view_rotation;
        }


        void ICaravanView.notify_on_change_anim(SpineDS ds)
        {
            if (spine_view == null) return;
            spine_view.set_anim(ds);
        }


        public void destroy()
        {
            DestroyImmediate(gameObject);
        }


        public GameObject create_slot_node()
        {
            return Instantiate(slot_model, m_slots_node.transform);
        }


        public DeviceView create_device_view(DeviceView view, GameObject slot_node)
        {
            return Instantiate(view, slot_node.transform);
        }
    }
}

