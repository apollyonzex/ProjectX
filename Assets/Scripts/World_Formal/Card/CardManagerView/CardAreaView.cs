using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Card
{
    public class CardAreaView : MonoBehaviour
    {
        public CardManager owner;

        public int area_index;

        public List<CardSlotView> slots = new List<CardSlotView>();
        public  void init_area(CardManager owner)
        {
            this.owner = owner;
            foreach (var slot in slots)
            {
                slot.init(this);
            }
        }

        public virtual void set_area(CardArea area)
        {
            if (area.slots.Count != slots.Count)
            {
                Debug.LogError("逻辑slots数量和预制体slots数量不匹配");
                return;
            }

            for(int i = 0; i < area.slots.Count; i++)
            {
                slots[i].set_card(area.slots[i].card);
            }
        }

        public  virtual void tick()
        {

        }
    }
}
