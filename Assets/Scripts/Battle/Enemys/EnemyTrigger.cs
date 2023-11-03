using Battle.Enemys.BT_GraphFlow;
using Common;
using System.Collections;
using System.Collections.Generic;

namespace Battle.Enemys
{
    public class EnemyTrigger : Trigger
    {
        //==================================================================================================

        public override void @do(bool is_init)
        {
            if (is_init)
            {
                EX_Utility.try_load_asset(("enemys", "bt_graphs/mbt_001"), out BT_GraphAsset asset);

                var mgr = new EnemyMgr(Config.EnemyMgr_Name);

                foreach (var cell in cells(asset))
                {
                    mgr.add_cell(cell, null);
                }
            }
        }


        IEnumerable<Enemy> cells(BT_GraphAsset asset)
        {
            for (int i = 0; i < 2; i++)
            {
                Enemy cell = new()
                {
                    id = (uint)i,
                    bctx = new(asset)
                };

                yield return cell;
            }
        }
    }
}

