using Foundation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battle.HandCards
{
    public class HandCardView : MonoBehaviour, IHandCardView, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        HandCardMgr mgr;
        HandCard cell;

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
        }


        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("up");
        }


        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("down");
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Debug.Log("ing");
        }
    }
}

