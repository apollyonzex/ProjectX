using World_Formal.Enviroment;
using World_Formal.Map.Encounters;
using Common_Formal;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using UnityEngine;
using World_Formal;
using World_Formal.Helpers;
using Common;
using static AutoCode.Tables.Sites;

namespace World_Formal.Map
{
    public interface IMapView : IModelView<MapMgr>
    {
        void notify_site_moved(int _old, int _new);
        void highlight_site(int index);
        void unhighlight_site(int index);
    }


    public class MapMgr : Model<MapMgr, IMapView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        WorldContext ctx;

        //==================================================================================================

        public MapMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }


        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }


        void IMgr.init(object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            ctx = WorldContext.instance;
            ctx.add_tick(Config.MapMgr_Priority, Config.MapMgr_Name, tick);
        }


        void tick()
        {
            if (is_arrive_process) //规则：等车停下后，才会触发随机奇遇
            {
                if (ctx.caravan_move_status == Enum.EN_caravan_move_status.idle)
                {
                    m_sites.sites.TryGetValue(current_index, out var site);
                    m_encounterMgr.active(site.site_encounter);
                    WorldSceneRoot.instance.ShowNext();
                    is_arrive_process = false;
                }
            }
        }

        //=====================================================================

        // private Vector2 map_offset = new Vector2(0, 100f);

        public Sites Sites => m_sites;
        private Sites m_sites;

        public int current_index;
        public List<int> next_indexs = new();
        public List<int> arrived_indexs = new();
        public List<int> exit_indexs = new();

        private EncounterMgr m_encounterMgr;

        public Sprite map_bkg;                  //考虑到这一素材可能会替换,所以记录下来

        public bool is_arrive_process = true;

        public void init(uint world_id,string data_bundle,string data_path,uint init_difficult) {
            AssetBundleManager.instance.load_asset<demo_editor.MapEditorData>(data_bundle, data_path, out var asset);
            tryGetData_from_db(out var db_dic);
            map_bkg = asset.map_bkg;
            m_sites = new Sites(new Dictionary<int, Site>(), asset.start_index, 0);
            foreach (var site_data in asset.sites) {
                uint rnd_id = site_data.id[Random.Range(0, site_data.id.Count)];
                var data = db_dic[rnd_id];
                Site site = new Site() {
                    id = rnd_id,
                    index = site_data.index,
                    position = site_data.position,
                    next_index = site_data.next_index,
                    name = data.f_name.ToString(),
                    icon_small = data.f_icon_small,
                    icon_large = data.f_icon_large,
                    scene_resources_id = site_data.scene_resource_ids.ToArray(),
                    site_encounter =data.f_site_encounter,
                };

                m_sites.sites.Add(site_data.index, site);
            }

            foreach(var exit in asset.exit_indexs)
            {
                exit_indexs.Add(exit);
            }

            current_index = asset.start_index;
            next_indexs = get_nextSite_index(current_index);

            //加载关联类
            m_encounterMgr = new(world_id,init_difficult);

            WorldSceneRoot.instance.ShowNext();
        }
        /// <summary>
        /// 根据index，获取下个site的index
        /// </summary>
        public List<int> get_nextSite_index(int i) {
            return m_sites.sites[i].next_index;
        }


        /// <summary>
        /// 根据index，获取指定site位置
        /// </summary>
        public Vector2 get_site_pos(int i) {
            return m_sites.sites[i].position;
        }


        /// <summary>
        /// 移动到新site
        /// </summary>
        /// <param name="_old"></param>
        /// <param name="_new"></param>
        public void move(int _old, int _new) {
            m_sites.sites.TryGetValue(_new, out var sites);

            Mission.instance.try_get_mgr("enviroment", out var mgr);
            var e = (mgr as EnviromentMgr);

            if (sites.scene_resources_id.Length != 0) {
                e.update_scene_resources(sites.scene_resources_id);
            }
            current_index = _new;
            next_indexs = get_nextSite_index(current_index);
            if (!arrived_indexs.Contains(sites.index)) {
                arrived_indexs.Add(sites.index);
                m_encounterMgr.journey_difficult += 1;
                WorldContext.instance.journey_difficult = m_encounterMgr.journey_difficult;
                Debug.Log("难度增加了1");
            }
            foreach (var view in views) {
                view.notify_site_moved(_old, _new);
            }
        }

        public void arrived() {
            m_sites.sites.TryGetValue(current_index, out var site);
            var se_type = site.site_encounter;

            if (se_type == e_se_Type.i_Battle)
            {
                m_encounterMgr.active(e_se_Type.i_Battle);
                WorldSceneRoot.instance.ShowNext();
                return;
            }

            Caravan_Move_Helper.instance.stop(ctx);
            is_arrive_process = true;
        }


        /// <summary>
        /// 读表获取
        /// </summary>
        private void tryGetData_from_db(out Dictionary<uint, AutoCode.Tables.Sites.Record> dic) {
            AutoCode.Tables.Sites site = new();
            dic = new();

            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "sites", out var asset);

            site.load_from(asset);

            foreach (var rec in site.records) {
                dic.Add(rec.f_id, rec);
            }
        }

        public void highlight_site(int index)
        {
            foreach(var view in views)
            {
                view.highlight_site(index);
            }
        }

        public void unhighlight_site(int index)
        {
            foreach (var view in views)
            {
                view.unhighlight_site(index);
            }
        }

    }
    public struct Sites {
        public Dictionary<int, Site> sites;//KV: index,Site
        public int start_index;
        public int end_index;

        public Sites(Dictionary<int, Site> keyValues, int start_index, int end_index) {
            this.sites = keyValues;
            this.start_index = start_index;
            this.end_index = end_index;
        }
    }


    public struct Site {
        public uint id;
        public int index;
        public Vector2 position;
        public List<int> next_index;
        public string name;
        public (string, string) icon_small;
        public (string, string) icon_large;
        public uint[] encounters;
        public uint[] scene_resources_id;
        public AutoCode.Tables.Sites.e_se_Type site_encounter;
    }

    public enum E_Site_State {
        Self,
        Attachable,
    }
}

