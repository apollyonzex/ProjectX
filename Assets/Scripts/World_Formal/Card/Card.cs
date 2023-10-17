using Foundation.Tables;

namespace World_Formal.Card
{
    public class Card
    {
        public AutoCode.Tables.Cards.Record desc;

        public Card(AutoCode.Tables.Cards.Record data)
        {
            desc = data;
        }

        public Card()
        {

        }
    }
}
