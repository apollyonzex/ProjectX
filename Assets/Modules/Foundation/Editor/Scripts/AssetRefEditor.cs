


using UnityEngine;
using UnityEditor;

namespace Foundation.Editor {

    [CustomEditor(typeof(AssetRef))]
    public class AssetRefEditor : UnityEditor.Editor {

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            var asset = AssetDatabase.LoadAssetAtPath<AssetRef>(assetPath);
            if (asset != null && asset.asset != null) {
                var asset_icon = AssetPreview.GetAssetPreview(asset.asset);
                if (asset_icon != null) {
                    var icon = new Texture2D(width, height);
                    EditorUtility.CopySerialized(asset_icon, icon);
                    return icon;
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

    }

}