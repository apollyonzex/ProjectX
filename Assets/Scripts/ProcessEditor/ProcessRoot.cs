using Common;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProcessEditor
{
    public class ProcessRoot : Root<ProcessAsset>
    {
        public GameObject section;
        public Transform group;
        public float length;

        GameObject m_environment_node;

        //==================================================================================================

        public override void load_asset()
        {
            @clear();

            length = asset.length;
            modify_length();

            var groups = asset.groups;
            for (int i = 0; i < groups.Length; i++)
            {
                var e = groups[i];

                var node = new GameObject();
                node.name = i.ToString();
                var cpn = node.AddComponent<Groups.Group>();
                cpn.icon_pos = e.icon_pos;

                node.transform.position = new Vector2(e.trigger_pos, 0);
                group = node.transform;

                var brush = ScriptableObject.CreateInstance<Enemys.EnemyBrush>();
                brush.init(this);
                foreach (var cell in e.enemies)
                {
                    brush.m_cell_id = cell.id;
                    brush.@do(new Vector2(cell.x, cell.y), true);
                }
            }
        }


        protected override void save_asset(ProcessAsset asset)
        {
            asset.length = length;

            var datas = FindObjectsOfType<Groups.Group>();
            List<ProcessAsset.Group> groups = new();

            foreach (var data in datas)
            {
                ProcessAsset.Group e = new()
                {
                    icon_pos = data.icon_pos,
                    trigger_pos = data.transform.localPosition.x,
                    enemies = data.enemies,
                };
                groups.Add(e);
            }

            asset.groups = groups.ToArray();
        }


        public void @clear(bool only_environment = false)
        {
            if (m_environment_node == null) return;
            DestroyImmediate(m_environment_node);
            if (only_environment) return;

            var groups = FindObjectsOfType<Groups.Group>();
            foreach (var e in groups)
            {
                DestroyImmediate(e.gameObject);
            }
        }


        /// <summary>
        /// 调整战场长度
        /// </summary>
        public void modify_length()
        {
            var floor_length = Config.current.reset_pos_intervel;
            var floor_num = Mathf.FloorToInt(length / floor_length);

            m_environment_node = new GameObject();
            m_environment_node.hideFlags = HideFlags.HideInHierarchy;

            for (int i = 0; i < floor_num; i++)
            {
                var pos = floor_length * i * Vector2.right;

                var view = Instantiate(section, m_environment_node.transform);
                view.transform.localPosition = pos;

                for (int j = 0; j < view.transform.childCount; j++)
                {
                    var e = view.transform.GetChild(j);
                    e.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                }
            }
        }


        public void create_group_node()
        {
            string name = "0";
            var e = FindObjectOfType<Groups.Group>();

            if (e != null)
            {
                name = e.gameObject.name;
                var max = Convert.ToInt32(name);
                max++;
                name = max.ToString();
            }

            var go = new GameObject();
            go.name = name;
            go.AddComponent<Groups.Group>();


        }
    }
}

