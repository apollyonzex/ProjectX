using UnityEditor;
using UnityEngine;

namespace ProcessEditor
{
    [CustomEditor(typeof(ProcessRoot), true)]
    public class ProcessRootEditor : RootEditor
    {
        ProcessRoot root;

        Enemys.EnemyBrush m_brush;

        //==================================================================================================

        private void OnEnable()
        {
            root = (ProcessRoot)target;

            m_brush = CreateInstance<Enemys.EnemyBrush>();
            m_brush.init(root);
        }


        private void OnDisable()
        {
            DestroyImmediate(m_brush);
        }


        public override void OnInspectorGUI_Up()
        {
            EditorGUILayout.Space();
            EditorGUILayout.EditorToolbar(m_brush);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Group Node"))
            {
                root.create_group_node();
            }
            if (GUILayout.Button("Modify Length"))
            {
                root.@clear(true);
                root.modify_length();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button("Load Asset"))
            {
                root.load_asset();
            }
        }
    }

}

