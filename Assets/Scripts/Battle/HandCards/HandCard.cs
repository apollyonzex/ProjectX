using UnityEngine;

namespace Battle.HandCards
{
    public class HandCard
    {
        public AutoCode.Tables.Card.Record _desc;

        //==================================================================================================

        public HandCard(uint id)
        {
            World.DB.instance.card.try_get(id, out _desc); 
        }
    }
}

