using UnityEditor;
using UnityEngine;

namespace Editor.DIY_Editor
{
    [CustomEditor(typeof(Root), true)]
    public class RootEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (Root)target;
            if (!t.enabled) return;

            OnInspectorGUI_Up();

            EditorGUILayout.Space();
            if (GUILayout.Button("Load Asset"))
            {
                t.load_asset();
            }
            if (GUILayout.Button("Save Asset"))
            {
                t.save_asset();
            }

            OnInspectorGUI_Down();
        }


        protected virtual void OnInspectorGUI_Down()
        {
        }


        protected virtual void OnInspectorGUI_Up()
        {
        }
    }

}

