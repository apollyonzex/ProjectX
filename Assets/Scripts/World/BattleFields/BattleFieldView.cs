using Foundation;
using UnityEngine;

namespace World.BattleFields
{
    public class BattleFieldView : MonoBehaviour, IBattleFieldView
    {
        BattleFieldMgr mgr;

        //==================================================================================================

        void IModelView<BattleFieldMgr>.attach(BattleFieldMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<BattleFieldMgr>.detach(BattleFieldMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<BattleFieldMgr>.shift(BattleFieldMgr old_mgr, BattleFieldMgr new_mgr)
        {
        }
    }
}

