using Battle.Enemys.BT_GraphFlow;
using Common;

namespace Battle.Enemys
{
    public class EnemyTrigger : Trigger
    {
        BT_Context bctx;

        //==================================================================================================

        public override void @do(bool is_init)
        {
            if (is_init)
            {
                EX_Utility.try_load_asset(("enemys", "bt_graphs/mbt_001"), out BT_GraphAsset asset);
                bctx = new(asset);
            }

            bctx.tick();
        }
    }
}

