using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using static AutoCode.Tables.EnemyNormal;

namespace ProcessEditor.Enemys
{
    public class EnemyBrush : EditorTool
    {
        GUIContent m_icon;
        public override GUIContent toolbarIcon => m_icon;

        ProcessRoot m_root;
        Dictionary<uint, Record> m_data_dic;
        public uint m_cell_id = 1;

        bool is_active = false;
        delegate void mouse_action(Vector2 pos, bool is_local = false);

        //==================================================================================================

        public void init(ProcessRoot root)
        {
            m_icon = new()
            {
                image = EditorGUIUtility.IconContent("d_BuildSettings.Lumin").image,
                text = "Enemy Brush",
                tooltip = "Enemy Brush",
            };

            m_root = root;
            m_data_dic = EnemyMgr.instance.raw;
        }


        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public override void OnToolGUI(EditorWindow window)
        {
            create_win(window);
            if (!is_active) return;

            mouse_down(@do, m_root);
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

            GUILayout.BeginArea(new Rect(pos, panel_size), "Enemy Brush", GUI.skin.window);
            EditorGUIUtility.labelWidth = 40;

            m_cell_id = (uint)Mathf.Max(0, EditorGUILayout.IntField("Id", (int)m_cell_id));

            if (m_data_dic.ContainsKey(m_cell_id))
            {
                EditorGUILayout.LabelField(" ", m_data_dic[m_cell_id].f_desc ?? string.Empty);
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
        /// 鼠标左键，点击
        /// </summary>
        void mouse_down(mouse_action action, Component c)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var ev = Event.current;

            if (ev.type != EventType.MouseDown) return;
            if (ev.button != 0) return;
            if (!Common.Expand.Utility.try_get_point(HandleUtility.GUIPointToWorldRay(ev.mousePosition), c, out var point)) return;

            action?.Invoke(point);
            ev.Use();
        }


        public void @do(Vector2 pos, bool is_local = false)
        {
            var e = m_data_dic[m_cell_id];
            var view = e.f_view;
            string path = $"{view.Item1}/{view.Item2}.prefab";
            Common.Expand.Utility.try_load_asset_without_running(path, false, out GameObject go);

            var group_node = m_root.group;
            if (group_node == null)
            {
                Debug.LogWarning("必须设置group节点");
                return;
            }
            var cell = Instantiate(go, group_node);

            if (e.f_move_type.value == e_moveType.i_ground)
                pos.y = 0;

            if (is_local)
                cell.transform.localPosition = pos;
            else
            cell.transform.position = pos;

            cell.name = e.f_id.ToString();
        }
    }
}

