using Foundation;
using System.Collections.Generic;
using UnityEngine;

namespace Worlds.Maps
{
    public class SiteMgrView : MonoBehaviour, ISiteMgrView
    {
        public GameObject site_model;//样本，用于克隆

        internal SiteMgr owner;
        Dictionary<int, SiteCellView> sites = new();

        void IModelView<SiteMgr>.attach(SiteMgr owner)
        {
            this.owner = owner;

            /**************  绘制地图site  ***************/           
            foreach (var site in owner.Sites.sites)
            {
                //生成Site
                var s = Instantiate(site_model).transform;
                s.SetParent(transform);
                s.localScale = Vector3.one;
                s.position = site.Value.position;           

                SiteCellView cellView = s.GetComponent<SiteCellView>();
                cellView.index = site.Key;
                cellView.site = site.Value;
                var icon_path = site.Value.icon;
                var icon = Common.Utility.load_and_instantiate_component_from_prefab<SpriteRenderer>(icon_path.Item1, icon_path.Item2, cellView.transform, false);
                owner.add_view(cellView);
                sites.Add(cellView.index, cellView);

                //生成道路
                foreach (int next_i in owner.get_nextSite_index(cellView.index))
                {
                    LineRenderer road = Instantiate(cellView.road_Model);
                    road.transform.SetParent(s);
                    road.SetPosition(0, owner.get_site_pos(cellView.index));
                    road.SetPosition(1, owner.get_site_pos(next_i));
                    road.gameObject.SetActive(true);
                }
            }

            //高亮nextSite
            upd_nextSite();

            /**************  点击事件  ***************/
            Common.Expand.MonoLayer.instance.on_pointer_click += on_pointer_click;
        }


        void IModelView<SiteMgr>.detach(SiteMgr owner)
        {
            if (this.owner != owner)
            {
                this.owner = null;
            }

            Common.Expand.MonoLayer.instance.on_pointer_click -= on_pointer_click;
            Destroy(gameObject);
        }


        void IModelView<SiteMgr>.shift(SiteMgr old_owner, SiteMgr new_owner)
        {
        }


        void ISiteMgrView.notify_site_moved(int _old, int _new)
        {
        }


        /// <summary>
        /// 更新nextSite
        /// </summary>
        public void upd_nextSite()
        {
            foreach (int next_i in owner.next_indexs)
            {
                sites[next_i].active_site(true, Color.blue, E_Site_State.Attachable);
            }
        }


        private void on_pointer_click()
        {
            if (!Common.Expand.MonoLayer.instance.try_get_interact_component(out SiteCellView cellView)) return;
            cellView.on_pointer_click();
        }
    }
}


