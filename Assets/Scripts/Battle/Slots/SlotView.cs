using Foundation;
using UnityEngine;

namespace Battle.Slots
{
    public class SlotView : MonoBehaviour, ISlotView
    {
        SlotMgr mgr;
        Slot cell;

        //==================================================================================================

        void IModelView<SlotMgr>.attach(SlotMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<SlotMgr>.detach(SlotMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<SlotMgr>.shift(SlotMgr old_mgr, SlotMgr new_mgr)
        {
        }


        void ISlotView.notify_on_init(Slot cell)
        {
            this.cell = cell;

            transform.localPosition = cell.view_pos;
        }
    }
}

