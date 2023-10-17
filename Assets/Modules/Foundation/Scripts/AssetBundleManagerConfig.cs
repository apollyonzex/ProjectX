using UnityEngine;

namespace Foundation {

#if UNITY_EDITOR
    public class AssetBundleManagerConfig : ScriptableObject {

        static AssetBundleManagerConfig s_instance;
        const string PATH = "Assets/AssetBundleManagerConfig.asset";
        public static AssetBundleManagerConfig instance {
            get {
                if (s_instance == null) {
                    s_instance = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetBundleManagerConfig>(PATH);
                    if (s_instance == null) {
                        s_instance = CreateInstance<AssetBundleManagerConfig>();
                        UnityEditor.AssetDatabase.CreateAsset(s_instance, PATH);
                    }
                }
                return s_instance;
            }
        }


        public string asset_bundle_path = "Assets/StreamingAssets";
        public UnityEditor.BuildAssetBundleOptions build_options = UnityEditor.BuildAssetBundleOptions.None;


    }
#endif
}