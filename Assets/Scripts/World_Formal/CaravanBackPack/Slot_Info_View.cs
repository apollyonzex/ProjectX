using Common_Formal;
using Common_Formal.DS;
using Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using World_Formal.Caravans;
using World_Formal.Caravans.Slots;

namespace World_Formal.CaravanBackPack
{
    public class Slot_Info_View : MonoBehaviour, ICaravanView, IPointerClickHandler
    {
        public Image icon;
        public TextMeshProUGUI text;
        public GameObject highlight;

        public Slot slot;

        CaravanMgr_Formal owner;
        Change_Device_Helper helper;

        //==================================================================================================

        void IModelView<CaravanMgr_Formal>.attach(CaravanMgr_Formal owner)
        {
            this.owner = owner;
        }


        void IModelView<CaravanMgr_Formal>.detach(CaravanMgr_Formal owner)
        {
            if (owner != null)
            {
                owner = null;
            }

            if (this != null)
                Destroy(gameObject);
        }


        void IModelView<CaravanMgr_Formal>.shift(CaravanMgr_Formal old_owner, CaravanMgr_Formal new_owner)
        {
        }


        void ICaravanView.notify_on_device_changed()
        {
            if (!owner.TryGetDevice(slot, out var device))
                text.text = "";
            else
                text.text = EX_Utility.get_uint_localization(device._desc.f_name);            
        }


        public void init(Change_Device_Helper helper, Slot slot, Sprite icon)
        {
            this.helper = helper;
            this.slot = slot;
            this.icon.sprite = icon;

            (this as ICaravanView).notify_on_device_changed();
        }


        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            helper.on_slot_label_click(slot);
        }


        public void do_highlight(bool bl)
        {
            highlight.SetActive(bl);
        }


        void ICaravanView.notify_on_tick()
        {
        }


        void ICaravanView.notify_on_change_anim(SpineDS ds)
        {
        }
    }
}

