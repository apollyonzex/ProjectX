using Battle.Enemys.BT_GraphFlow;
using Common;
using System.Collections.Generic;
using World;

namespace Battle.HandCards
{
    public class HandCardTrigger : Trigger
    {
        public int count;
        public HandCardView model;

        HandCardMgr mgr;

        //==================================================================================================

        public override void @do(bool is_init)
        {
            if (is_init)
                mgr = new(Config.HandCardMgr_Name);

            add_cells();
        }


        IEnumerable<HandCard> cells()
        {
            var bctx = WorldContext.instance.bctx;
            var hold_count = bctx.handcard_count;

            for (int i = 0; i < count - hold_count; i++)
            {
                var id = (uint)EX_Utility.rnd_int(1,5);
                HandCard cell = new(id);

                yield return cell;
            }
        }


        void add_cells()
        {
            foreach (var cell in cells())
            {
                var view = Instantiate(model, transform);
                mgr.add_cell(cell, view);
            }
        }
    }
}

