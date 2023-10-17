using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using static AutoCode.Tables.World;
using Camp;
using Unity.VisualScripting;
using TMPro;

namespace Level_Entrance_Editor
{
    public class CellBrush : EditorTool
    {
        GUIContent m_icon;
        public override GUIContent toolbarIcon => m_icon;

        Level_Entrance_Root m_root;

        Dictionary<uint, Record> m_world_dic;
        public uint m_world_id = 0;
        public string m_name;

        Dictionary<uint, string> m_localization_dic;

        bool is_active = false;
        delegate void mouse_action(Vector2 pos);

        //==================================================================================================

        public void init(Level_Entrance_Root root)
        {
            m_icon = new()
            {
                image = EditorGUIUtility.IconContent("Collab.BuildSucceeded").image,
                text = "Cell Brush",
                tooltip = "Cell Brush",
            };

            m_root = root;

            m_world_dic = new();
            var worlds = DB.instance.world_without_running.records;
            foreach (var r in worlds)
            {
                m_world_dic.Add(r.f_id, r);
            }

            m_localization_dic = new();
            var lzs = Common_Formal.DB.instance.uint_lz_without_running.records;
            foreach (var r in lzs)
            {
                m_localization_dic.Add(r.f_id, r.f_en);
            }

        }


        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public override void OnToolGUI(EditorWindow window)
        {
            create_win(window);
            if (!is_active) return;

            mouse_left_down(do_when_left_down, m_root);
            mouse_middle_down(do_when_middle_down, m_root);
            mouse_right_down(do_when_right_down, m_root);
        }


        /// <summary>
        /// 创建窗体
        /// </summary>
        void create_win(EditorWindow window)
        {
            Handles.BeginGUI();
            var size = window.position.size;
            var panel_size = new Vector2(200, 70);
            var pos = size - panel_size - new Vector2(16, 36);

            GUILayout.BeginArea(new Rect(pos, panel_size), "Cell Brush", GUI.skin.window);
            EditorGUIUtility.labelWidth = 40;

            m_world_id = (uint)Mathf.Max(0, EditorGUILayout.IntField("Id", (int)m_world_id));
            if (m_world_dic.ContainsKey(m_world_id))
            {
                var name = m_world_dic[m_world_id].f_name;
                m_name = m_localization_dic[name];
                EditorGUILayout.LabelField(" ", m_name ?? string.Empty);
                is_active = true;
            }
            else
            {
                EditorGUILayout.LabelField(" ", "<Invalid>");
                is_active = false;
            }

            GUILayout.EndArea();
            Handles.EndGUI();
        }


        /// <summary>
        /// 鼠标左键，点击添加
        /// </summary>
        void mouse_left_down(mouse_action action, Component c)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var ev = Event.current;

            if (ev.type != EventType.MouseDown) return;
            if (ev.button != 0) return;
            if (!Common_Formal.EX_Utility.try_get_point(HandleUtility.GUIPointToWorldRay(ev.mousePosition), c, out var point)) return;     

            action?.Invoke(point);
            ev.Use();
        }


        void do_when_left_down(Vector2 pos)
        {
            create_cell(m_world_dic[m_world_id], m_world_id, pos, m_name);
        }


        CellData create_cell(Record r, uint world_id, Vector2 pos, string name)
        {
            var view_path = r.f_entrance_view;
            var path = $"{view_path.Item1}/{view_path.Item2}.prefab";
            Common_Formal.EX_Utility.try_load_asset_without_running(path, false, out Camp.Level_Entrances.Level_EntranceView prefab);
            var view = Instantiate(prefab, m_root.ui_root);
            view.gameObject.name = name;
            view.transform.localPosition = pos;

            var collider = view.AddComponent<CircleCollider2D>();
            collider.radius = 50f;

            var data = view.AddComponent<CellData>();
            data.init(world_id);
            return data;
        }


        /// <summary>
        /// 外部使用
        /// </summary>
        public void create_cell(Record r, uint world_id, Vector2 pos, string name, Vector2 size, uint seq)
        {
            var e = create_cell(r, world_id, pos, m_localization_dic[r.f_name]);
            e.seq = seq;

            var tmp = e.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = name;

            var rect = e.GetComponent<RectTransform>();
            rect.sizeDelta = size;
        }


        /// <summary>
        /// 鼠标中键，点击选中
        /// </summary>
        void mouse_middle_down(mouse_action action, Component c)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var ev = Event.current;
            if (ev.type != EventType.MouseDown) return;
            if (ev.button != 2) return;
            if (!Common_Formal.EX_Utility.try_get_point(HandleUtility.GUIPointToWorldRay(ev.mousePosition), c, out var point)) return;

            action?.Invoke(point);
            ev.Use();
        }


        void do_when_middle_down(Vector2 pos)
        {
            var hit = Physics2D.Raycast(pos, Vector2.zero).collider;
            if (hit == null) return;
            Selection.objects = new Object[] { hit.gameObject };
        }


        /// <summary>
        /// 鼠标右键，点击 & 拖拽
        /// </summary>
        void mouse_right_down(mouse_action action, Component c)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var ev = Event.current;
            if (ev.type != EventType.MouseDown && ev.type != EventType.MouseDrag) return;
            if (ev.button != 1) return;
            if (!Common_Formal.EX_Utility.try_get_point(HandleUtility.GUIPointToWorldRay(ev.mousePosition), c, out var point)) return;

            action?.Invoke(point);
            ev.Use();
        }


        void do_when_right_down(Vector2 pos)
        {
            var hit = Physics2D.Raycast(pos, Vector2.zero).collider;
            if (hit == null) return;

            var go = hit.gameObject;
            go.transform.localPosition = pos;
        }

    }
}

