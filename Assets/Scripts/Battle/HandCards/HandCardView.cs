using Foundation;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Battle.HandCards
{
    public class HandCardView : MonoBehaviour, IHandCardView, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public TextMeshProUGUI title;

        HandCardMgr mgr;
        HandCard cell;

        Common.Helpers.Mouse_Move_Helper mouse_h;

        HandCard IHandCardView.cell => cell;

        //==================================================================================================

        void IModelView<HandCardMgr>.attach(HandCardMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<HandCardMgr>.detach(HandCardMgr mgr)
        {
            this.mgr = null;

            DestroyImmediate(gameObject);
        }


        void IModelView<HandCardMgr>.shift(HandCardMgr old_mgr, HandCardMgr new_mgr)
        {
        }


        void IHandCardView.notify_on_init(HandCard cell)
        {
            this.cell = cell;

            title.text = cell._desc.f_title;

            mouse_h = Common.Helpers.Mouse_Move_Helper.instance;
        }


        void IHandCardView.notify_on_tick1()
        {
        }


        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            mouse_h.calc_offset(transform.position);
        }


        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            mgr.play(cell);

            mouse_h.clear();
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = mouse_h.drag_pos;
        }
    }
}

