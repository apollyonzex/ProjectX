using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace World_Formal.Card
{
    public class CardEffect :MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public TextMeshProUGUI description;

        public GameObject descriptionPanel;

        public void init(string str)
        {
            description.text = str;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            descriptionPanel.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            descriptionPanel.SetActive(false);
        }
    }
}
