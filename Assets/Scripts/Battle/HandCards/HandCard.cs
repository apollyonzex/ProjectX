using Battle.HandCards.Funcs;

namespace Battle.HandCards
{
    public class HandCard
    {
        public AutoCode.Tables.Card.Record _desc;
        public IFunc use_func;

        //==================================================================================================

        public HandCard(uint id)
        {
            World.DB.instance.card.try_get(id, out _desc);
           
            Utility.expr_convert(_desc.f_use_func, out use_func, out var _);
        }
    }
}

