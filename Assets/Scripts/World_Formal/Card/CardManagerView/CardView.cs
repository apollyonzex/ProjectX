using Foundation;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace World_Formal.Card
{
    public class CardView : MonoBehaviour,IPointerClickHandler
    {

        public CardSlotView owner;

        public Image card_sprite;

        public TextMeshProUGUI card_cost;

        public TextMeshProUGUI card_name;

        public CardEffect use_effect, drop_effect;

        public void init_card_view(Card card,int slot_index, CardSlotView owner)
        {
            this.owner = owner;
            var desc = card.desc;
            AssetBundleManager.instance.load_asset<Sprite>(desc.f_image.Item1, desc.f_image.Item2,out var s);
            card_sprite.sprite = s;
            card_cost.text = $"{desc.f_cost}";
            card_name.text = $"{desc.f_name}";
            use_effect.init($"{desc.f_use_desc}");
            drop_effect.init($"{desc.f_drop_desc}");
            owner.has_card = true;
        }

        public void use()
        {
            if(owner.owner.owner.UseCard(owner.owner.area_index, owner.index))
            {
                Debug.Log($"area index {owner.owner.area_index} 的 slot index {owner.index} 的card被使用了");
                gameObject.SetActive(false);
                owner.has_card = false;
            }
        }

        public void drop()
        {
            if (owner.owner.owner.DropCard(owner.owner.area_index, owner.index))
            {
                Debug.Log($"area index {owner.owner.area_index} 的 slot index {owner.index} 的card被丢弃了");
                gameObject.SetActive(false);
                owner.has_card = false;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                use();
            }
            else if(eventData.button == PointerEventData.InputButton.Right)
            {
                drop();
            }
        }
    }
}
