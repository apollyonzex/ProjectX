using Common_Formal;
using Common_Formal.DS;
using Foundation;
using UnityEngine;

namespace World_Formal.Caravans.Devices
{
    public class DeviceView : MonoBehaviour, IDeviceView
    {
        public SpineView spine_view;

        DeviceMgr mgr;

        Device IDeviceView.cell { get; set; }

        //==================================================================================================

        void IModelView<DeviceMgr>.attach(DeviceMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<DeviceMgr>.detach(DeviceMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<DeviceMgr>.shift(DeviceMgr old_owner, DeviceMgr new_owner)
        {
        }


        void IDeviceView.notify_on_tick1(Vector2 pos)
        {
            transform.position = EX_Utility.v2_cover_v3(pos, transform.position);
            transform.localRotation = mgr.caravan_mgr.view_rotation;
        }


        void IDeviceView.notify_on_change_anim(SpineDS ds)
        {
            if (spine_view == null) return;
            spine_view.set_anim(ds);
        }


        private void OnDestroy()
        {
            mgr.remove_cell(this);
        }
    }
}

