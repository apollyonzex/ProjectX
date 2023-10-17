
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace demo_editor {
    [ExecuteInEditMode]

    public class MapEditorRoot :MonoBehaviour{

        public MapEditorData map_data;

        public Site prefab;

        public List<Site> sites_list = new List<Site>();

        public Site start_site;

        public GameObject bkg;

        public List<Site> exit_sites = new List<Site>();

        public void save() {
            if (map_data != null) {
            } else {
                var t = ScriptableObject.CreateInstance<MapEditorData>();
                AssetDatabase.CreateAsset(t, @"Assets\Resources\RawResources\map\config\" + "new mapdata" +".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                map_data = t;
            }
            map_data.sites.Clear();
            if(start_site == null) {
                Debug.LogError("保存失败,没有设置初始地点");
                return;
            }
            foreach(var exit in exit_sites)
            {
                map_data.exit_indexs.Add(exit.index);
            }
            map_data.start_index = start_site.index;
            foreach (var site in sites_list){
                map_data.sites.Add(new SiteData {
                    index = site.index,
                    id = site.excel_id_list,
                    position = site.transform.position,
                    scene_resource_ids = site.scene_resource_ids,
                });
                foreach(var next_site in site.next_sites) { 
                    map_data.sites[map_data.sites.Count - 1].next_index.Add(next_site.index);
                }
            }
            map_data.map_bkg = bkg.GetComponent<SpriteRenderer>().sprite;
            EditorUtility.SetDirty(map_data);
            AssetDatabase.SaveAssetIfDirty(map_data);
            Debug.Log("保存成功");
        }

        public void create_point(Vector2 pos) {
            var g = Instantiate(prefab,transform, false);
            g.transform.position = pos;
            g.index = sites_list.Count;
            g.text.text = g.index.ToString();
            g.gameObject.SetActive(true);
            g.transform.SetParent(transform);
            g.gameObject.name = $"point {g.index}";
            sites_list.Add(g);
        }

        public void remove_point(int index) {
            foreach(var site in sites_list) {           //先移除所有所有连向这个点的点
                for(int i = 0; i < site.next_sites.Count; i++) {
                    if (site.next_sites[i].index == index) {
                        site.next_sites.RemoveAt(i);
                    }
                }
            }
            sites_list.RemoveAt(index);                 //再移除这个点
            for(int i = 0; i < sites_list.Count; i++) { //最后重新生成表
                sites_list[i].index = i;
                sites_list[i].gameObject.name = $"point {i}";
                sites_list[i].text.text = i.ToString();
            }
        }

    }
}
