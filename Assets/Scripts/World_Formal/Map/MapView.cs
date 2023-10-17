using World_Formal.Enviroment;
using Common_Formal;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Map
{
    public class MapView : MonoBehaviour, IMapView
    {
        

        MapMgr owner;

        public Camera mapCamera;

        public GameObject site_model;//样本，用于克隆

        public GameObject mapCaravanView;

        public SpriteRenderer bkg;

        Dictionary<int, MapCellView> sites = new();

        //==================================================================================================

        void IModelView<MapMgr>.attach(MapMgr owner)
        {
            this.owner = owner;

            bkg.sprite = owner.map_bkg;

            /**************  绘制地图site  ***************/
            foreach (var site in owner.Sites.sites) {
                //生成Site
                var s = Instantiate(site_model).transform;
                s.SetParent(transform);
                s.localScale = Vector3.one;
                s.localPosition = site.Value.position;

                MapCellView cellView = s.GetComponent<MapCellView>();
                cellView.index = site.Key;
                cellView.site = site.Value;
                var icon_path = site.Value.icon_small;
                var icon = Common.Utility.load_and_instantiate_component_from_prefab<SpriteRenderer>(icon_path.Item1, icon_path.Item2, cellView.transform, false);
                cellView.init(owner);
                sites.Add(cellView.index, cellView);

                //生成道路
                foreach (int next_i in owner.get_nextSite_index(cellView.index)) {
                    LineRenderer road = Instantiate(cellView.road_Model);
                    road.transform.SetParent(s);
                    road.SetPosition(0, owner.get_site_pos(cellView.index) + new Vector2(transform.localPosition.x,transform.localPosition.y));
                    road.SetPosition(1, owner.get_site_pos(next_i) + new Vector2(transform.localPosition.x, transform.localPosition.y));
                    road.gameObject.SetActive(true);
                }
            }
            mapCaravanView.GetComponent<SpriteRenderer>().sprite = Common.Config.current.map_caravan;
            mapCaravanView.transform.localPosition = sites[owner.current_index].transform.localPosition;
            mapCamera.transform.localPosition = new Vector3(sites[owner.current_index].transform.localPosition.x, sites[owner.current_index].transform.localPosition.y, -10f);

            foreach (var index in owner.next_indexs)
            {
                sites[index].active_site(true, Color.blue, E_Site_State.Attachable);
            }
        }


        void IModelView<MapMgr>.detach(MapMgr owner)
        {
            this.owner = null;
        }

        void IMapView.notify_site_moved(int _old, int _new) {
            StartCoroutine(IMove(_old, _new));
        }

        void IModelView<MapMgr>.shift(MapMgr old_owner, MapMgr new_owner){

        }
        public void upd_nextSite() {
            foreach (int next_i in owner.next_indexs) {
                sites[next_i].active_site(true, Color.blue, E_Site_State.Attachable);
            }
        }

        IEnumerator IMove(int _old,int _new) {

            Mission.instance.try_get_mgr("enviroment", out var mgr);

            var old_position = sites[_old].transform.localPosition;
            var new_position = sites[_new].transform.localPosition;

            var dir = (new_position - old_position).normalized;

            var currnet_position = old_position;

            while((new_position - currnet_position).sqrMagnitude > 0.01f) {
                currnet_position += dir * Common.Config.current.map_speed;
                mapCaravanView.transform.localPosition = currnet_position;
                mapCamera.transform.localPosition = new Vector3(currnet_position.x, currnet_position.y, -10f);
                if (mgr is EnviromentMgr emgr) {
                    emgr.move(Vector2.right * Common.Config.current.default_speed);
                }
                yield return new WaitForFixedUpdate() ;
            }

            sites[_old].active_site(false, Color.gray, E_Site_State.Self);
            foreach(var site in sites[_old].site.next_index) {
                sites[site].active_site(false, Color.gray, E_Site_State.Self);
            }
            sites[_new].active_site(true, Color.green, E_Site_State.Attachable);

            mapCaravanView.transform.localPosition = new_position;
            mapCamera.transform.localPosition = new Vector3(new_position.x, new_position.y, -10f);

            upd_nextSite();

            owner.arrived();
        }

        void IMapView.highlight_site(int index)
        {
            sites[index].active_site(true, Color.yellow, E_Site_State.Self);
        }

        void IMapView.unhighlight_site(int index)
        {
            sites[index].active_site(true, Color.blue, E_Site_State.Self);
        }

    }
}

