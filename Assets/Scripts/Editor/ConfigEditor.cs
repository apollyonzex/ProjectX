
using UnityEngine;
using UnityEditor;

namespace Editor {

    [CustomEditor(typeof(Common.Config))]
    public class ConfigEdtor : UnityEditor.Editor {


        public override void OnInspectorGUI() {
            serializedObject.Update();

            m_filter = GUILayout.TextField(m_filter);

            if (string.IsNullOrEmpty(m_filter)) {
                var sp = serializedObject.GetIterator();
                if (sp.NextVisible(true) && sp.NextVisible(false)) {
                    do {
                        EditorGUILayout.PropertyField(sp);
                    } while (sp.NextVisible(false));
                }
            } else {
                var sp = serializedObject.GetIterator();
                if (sp.NextVisible(true) && sp.NextVisible(false)) {
                    do {
                        if (filter(sp.name)) {
                            EditorGUILayout.PropertyField(sp);
                        }

                    } while (sp.NextVisible(false));
                }
            }


            serializedObject.ApplyModifiedProperties();
        }

        private string m_filter;

        private bool filter(string content) {
            int last = 0;
            foreach (var c in m_filter) {
                var idx = content.IndexOf(c, last);
                if (idx < last) {
                    return false;
                }
                last = idx + 1;
            }
            return true;
        }
    }
}