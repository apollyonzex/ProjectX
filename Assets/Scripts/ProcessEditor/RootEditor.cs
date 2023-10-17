using UnityEditor;
using UnityEngine;

namespace ProcessEditor
{
    [CustomEditor(typeof(Root), true)]
    public class RootEditor : Editor
    {
        public sealed override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OnInspectorGUI_Up();

            if (GUILayout.Button("Save Asset"))
            {
                foreach (Root target in this.targets)
                {
                    if (target.enabled)
                        target.save_asset();
                }
            }

            OnInspectorGUI_Down();
        }


        public virtual void OnInspectorGUI_Up()
        {
        }


        public virtual void OnInspectorGUI_Down()
        {
        }
    }
}

