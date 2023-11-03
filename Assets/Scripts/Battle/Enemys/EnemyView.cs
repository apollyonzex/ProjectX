using Foundation;
using UnityEngine;

namespace Battle.Enemys
{
    public class EnemyView : MonoBehaviour, IEnemyView
    {
        EnemyMgr mgr;
        Enemy cell;

        //==================================================================================================

        void IModelView<EnemyMgr>.attach(EnemyMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<EnemyMgr>.detach(EnemyMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<EnemyMgr>.shift(EnemyMgr old_mgr, EnemyMgr new_mgr)
        {
        }


        void IEnemyView.notify_on_init(Enemy cell)
        {
            this.cell = cell;
        }
    }
}

