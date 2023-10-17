using Foundation;
using UnityEngine;


namespace Worlds.Maps
{
    public class SiteCellView : MonoBehaviour, ISiteMgrView
    {
        public SiteMgrView mgrView;
        public SpriteRenderer indicator_Pic;
        public LineRenderer road_Model;

        internal int index;
        internal Site site;
        internal SiteMgr owner;
        private bool can_Opr = false;//是否可操作


        void IModelView<SiteMgr>.attach(SiteMgr owner)
        {
            this.owner = owner;

            /**************  初始化site状态  ***************/
            active_site(index == owner.Sites.start_index, Color.green, E_Site_State.Self);
            gameObject.SetActive(true);
        }


        void IModelView<SiteMgr>.detach(SiteMgr owner)
        {
            if (this.owner != owner)
            {
                this.owner = null;
            }
            Destroy(gameObject);
        }


        void IModelView<SiteMgr>.shift(SiteMgr old_owner, SiteMgr new_owner)
        {
        }


        /// <summary>
        /// 激活/休眠Site
        /// </summary>
        /// <param name="b"></param>
        public void active_site(bool b, Color cl, E_Site_State state)
        {
            indicator_Pic.color = cl;
            indicator_Pic.gameObject.SetActive(b);
            can_Opr = b;

            if (state == E_Site_State.Attachable) can_Opr = true;

            if (state == E_Site_State.Self) can_Opr = false;           
        }


        public void on_pointer_click()
        {
            if (!can_Opr) return;
            owner.move(owner.current_index, index);
        }


        void ISiteMgrView.notify_site_moved(int _old, int _new)
        {
            active_site(index == owner.current_index, Color.green, E_Site_State.Self);
            mgrView.upd_nextSite();
        }
    }



}


