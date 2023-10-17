
using UnityEditor;
using UnityEngine;

namespace Foundation.Editor {

    [CustomEditor(typeof(BinaryAsset))]
    [CanEditMultipleObjects]
    public class BinaryAssetEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            if (serializedObject.isEditingMultipleObjects) {
                long total = 0;
                foreach (BinaryAsset target in targets) {
                    var size = target.bytes != null ? target.bytes.Length : 0;
                    EditorGUILayout.LabelField($"{target.name}: {EditorUtility.FormatBytes(size)}");
                    total += size;
                }
                EditorGUILayout.LabelField($"Total: {EditorUtility.FormatBytes(total)}");
            } else {
                var target = this.target as BinaryAsset;
                EditorGUILayout.LabelField(EditorUtility.FormatBytes(target.bytes != null ? target.bytes.Length : 0));
            }
        }

        protected override bool ShouldHideOpenButton() {
            return true;
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            var icon = new Texture2D(width, height);
            EditorUtility.CopySerialized(EditorResources.instance.binaryAssetIcon, icon);
            return icon;
        }
    }

}

