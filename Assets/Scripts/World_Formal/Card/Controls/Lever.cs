using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World_Formal.Card.Controls
{
    public class Lever :  MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
    {
        public LeverControls owner;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(owner.GetComponent<RectTransform>(), eventData.position, eventData.enterEventCamera, out var pos);
            owner.update_lever_value(new Vector3(GetComponent<RectTransform>().position.x,pos.y, 0));
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
        }
    }
}
