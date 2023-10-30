using Battle.HandCards.Funcs;
using Foundation.Tables;
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
           
            Utility.expr_convert(_desc.f_use_func, out var obj, out var err_msg);
            (obj as IFunc).exec();
        }
    }
}

