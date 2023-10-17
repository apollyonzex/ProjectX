using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World_Formal.Map
{
    public class MapButton :  MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {

        public Action on_click,mouse_enter,mouse_exit;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            on_click?.Invoke();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            mouse_enter?.Invoke();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            mouse_exit?.Invoke();
        }

    }
}
