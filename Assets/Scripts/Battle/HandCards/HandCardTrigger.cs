using Common;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace Battle.HandCards
{
    public class HandCardTrigger : Trigger
    {
        public int count;
        public HandCardView model;

        HandCardMgr mgr;

        //==================================================================================================

        public override void @do()
        {
            if (!Mission.instance.try_get_mgr(Config.HandCardMgr_Name, out mgr))
                mgr = new(Config.HandCardMgr_Name);

            add_cells();
        }


        IEnumerable<HandCard> cells()
        {
            var bctx = WorldContext.instance.bctx;
            var hold_count = bctx.handcard_count;

            for (int i = 0; i < count - hold_count; i++)
            {
                HandCard cell = new();

                yield return cell;
            }
        }


        void add_cells()
        {
            foreach (var cell in cells())
            {
                var view = Instantiate(model, transform);
                //cell.init(view.gameObject.transform.position);

                mgr.add_cell(cell, view);
            }
        }


        void remove_cells()
        {

        }
    }
}

