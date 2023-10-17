using demo_editor;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using UnityEngine;


namespace Worlds.Maps
{
    public struct Sites
    {
        public Dictionary<int, Site> sites;//KV: index,Site
        public int start_index;
        public int end_index;

        public Sites(Dictionary<int, Site> keyValues, int start_index, int end_index)
        {
            this.sites = keyValues;
            this.start_index = start_index;
            this.end_index = end_index;
        }
    }


    public struct Site
    {
        public uint id;
        public int index;
        public Vector2 position;
        public List<int> next_index;
        public string name;
        public (string,string) icon;
        public uint[] encounters;

        //不稳定内容(指不属于策划需求的内容
        public (string, string) timeline;
    }


    public enum E_Site_State
    {
        Self,
        Attachable,
    }


    public interface ISiteMgrView : IModelView<SiteMgr>
    {
        void notify_site_moved(int _old, int _new);
    }


    public class SiteMgr : Model<SiteMgr, ISiteMgrView>
    {
        public Sites Sites => m_sites;
        private Sites m_sites;

        public int current_index;
        public List<int> next_indexs = new();

        private EncounterMgr m_encounterMgr;
        private List<int> exit_indexs = new();

        public SiteMgr()
        {
            AssetBundleManager.instance.load_asset<MapEditorData>("map/map_data", "DemoMap_001", out var asset);
            tryGetData_from_db(out var db_dic);

            m_sites = new Sites(new Dictionary<int, Site>(), asset.start_index, 0);
            foreach (var item in asset.sites)
            {
                uint rnd_id = item.id[Random.Range(0, item.id.Count)];
                var data = db_dic[rnd_id];
                Site site = new Site()
                {
                    id = rnd_id,
                    index = item.index,
                    position = item.position,
                    next_index = item.next_index,
                    name = data.f_name,
                    icon = data.f_icon,
                    encounters = data.f_encounter,
                    //timeline = data.f_timeline
                };

                m_sites.sites.Add(item.index, site);
            }

            foreach(var exit in asset.exit_indexs)
            {
                exit_indexs.Add(exit);
            }

            current_index = asset.start_index;
            next_indexs = get_nextSite_index(current_index);

            //加载关联类
            m_encounterMgr = new();
        }


        /// <summary>
        /// 根据index，获取下个site的index
        /// </summary>
        public List<int> get_nextSite_index(int i)
        {
            return m_sites.sites[i].next_index;
        }


        /// <summary>
        /// 根据index，获取指定site位置
        /// </summary>
        public Vector2 get_site_pos(int i)
        {
            return m_sites.sites[i].position;
        }


        /// <summary>
        /// 移动到新site
        /// </summary>
        /// <param name="_old"></param>
        /// <param name="_new"></param>
        public void move(int _old, int _new)
        {
            current_index = _new;
            next_indexs = get_nextSite_index(current_index);

            foreach (var view in views)
            {
                view.notify_site_moved(_old, _new);
            }

            uint[] ec_ids = m_sites.sites[current_index].encounters;
            m_encounterMgr.active(ec_ids);
        }


        /// <summary>
        /// 读表获取
        /// </summary>
        private void tryGetData_from_db(out Dictionary<uint, AutoCode.Tables.Site.Record> dic)
        {
            AutoCode.Tables.Site site = new();
            dic = new();

            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "site", out var asset);

            site.load_from(asset);

            foreach (var rec in site.records)
            {
                dic.Add(rec.f_id, rec);
            }
        }
        

    }
}


