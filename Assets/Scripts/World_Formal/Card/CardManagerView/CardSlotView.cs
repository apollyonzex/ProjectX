using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Card
{
    public class CardSlotView: MonoBehaviour
    {

        public bool has_card = false;

        public CardAreaView owner;

        public int index;

        public CardView cardView = null;

        public void init(CardAreaView owner)
        {
            this.owner = owner;
        }

        public void set_card(Card card)
        {
            if (card != null)
            {
                cardView.init_card_view(card, index, this);
                cardView.gameObject.SetActive(true);
            }
        }

        public void use_card()
        {
            cardView.use();
        }

    }
}
