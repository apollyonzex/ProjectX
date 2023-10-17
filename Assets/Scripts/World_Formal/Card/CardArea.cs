using System;
using System.Collections.Generic;

namespace World_Formal.Card
{
    /// <summary>
    /// 手牌区
    /// </summary>
    public class CardArea
    {
        public List<CardSlot> slots = new List<CardSlot>();

        public CardArea(int slot_num)
        {
            for(int i =0;i<slot_num; ++i)
                slots.Add(new CardSlot());
        }

        public virtual bool put_card(Card card)
        {
            foreach(var slot in slots)
            {
                if (slot.put_card(card))
                {
                    return true;
                }
            }

            return false;
        }
    }
}