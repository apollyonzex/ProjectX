using Foundation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battle.HandCards
{
    public class HandCardView : MonoBehaviour, IHandCardView, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        HandCardMgr mgr;
        HandCard cell;

        Common.Helpers.Mouse_Move_Helper mouse_h;

        //==================================================================================================

        void IModelView<HandCardMgr>.attach(HandCardMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<HandCardMgr>.detach(HandCardMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<HandCardMgr>.shift(HandCardMgr old_mgr, HandCardMgr new_mgr)
        {
        }


        void IHandCardView.notify_on_init(HandCard cell)
        {
            this.cell = cell;

            mouse_h = Common.Helpers.Mouse_Move_Helper.instance;
        }


        void IHandCardView.notify_on_tick1()
        {
            //transform.position = cell.pos;
            Debug.Log(transform.position);
        }


        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            mouse_h.calc_offset(transform.position);
        }


        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            

            mouse_h.clear();
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            cell.pos = mouse_h.drag_pos;
        }
    }
}

