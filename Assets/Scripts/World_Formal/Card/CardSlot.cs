

namespace World_Formal.Card
{
    public class CardSlot
    {
        public Card card = null;

        public virtual  bool put_card(Card card)
        {
            if(this.card == null)
            {
                this.card = card;
                return true;
            }
            return false;
        }
    }
}
