using Editor.DIY_Editor;
using UnityEditor;
using UnityEngine;

namespace Level_Entrance_Editor
{
    [CustomEditor(typeof(Level_Entrance_Root), true)]
    public class Level_Entrance_RootEditor : RootEditor
    {
        Level_Entrance_Root root;
        CellBrush m_brush;

        //==================================================================================================

        private void OnEnable()
        {
            root = (Level_Entrance_Root)target;

            m_brush = CreateInstance<CellBrush>();
            m_brush.init(root);

            root.m_brush = m_brush;
        }


        private void OnDisable()
        {
            DestroyImmediate(m_brush);
        }


        protected override void OnInspectorGUI_Up()
        {
            EditorGUILayout.Space();
            EditorGUILayout.EditorToolbar(m_brush);

            EditorGUILayout.Space();
            if (GUILayout.Button("Clear"))
            {
                root.clear();
            }

        }
    }
}

