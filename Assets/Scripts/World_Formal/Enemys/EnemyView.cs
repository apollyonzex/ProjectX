using Common_Formal;
using Foundation;
using Spine.Unity;
using UnityEngine;

namespace World_Formal.Enemys
{
    public class EnemyView : MonoBehaviour, IEnemyView
    {
        public Enemy_SpineView spine_view;
        public Collider2D collider_box;

        EnemyMgr mgr;
        public Enemy cell;

        SkeletonAnimation anim => spine_view.anim;

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


        void IEnemyView.notify_on_tick1()
        {
            transform.localPosition = cell.view_pos;
            transform.localRotation = cell.view_dir;

            if (anim != null)
            {
                anim.skeleton.ScaleX = cell.scaleX;
                anim.skeleton.ScaleY = cell.scaleY;
            } 
        }


        void IEnemyView.notify_on_init(Enemy cell)
        {
            this.cell = cell;
            cell.collider = collider_box;
        }


        void IEnemyView.notify_on_destory()
        {
            DestroyImmediate(gameObject);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Caravan_Body")
            {
                cell.is_collider_with_caravan = true;
            }
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Caravan_Body")
            {
                cell.is_collider_with_caravan = false;
            }
        }


        public void change_Layer(int n)
        {
            gameObject.layer = n;
        }
    }
}

