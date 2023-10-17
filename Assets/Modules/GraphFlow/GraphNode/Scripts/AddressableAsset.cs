
using UnityEngine;
using Foundation;

namespace GraphNode {

    [System.Serializable]
    public class AddressableAsset<T> where T : Object {
        public string bundle;
        public string path;

        public bool load_asset(out T asset) {
            if (string.IsNullOrEmpty(bundle) || string.IsNullOrEmpty(path)) {
                asset = null;
                return false;
            }
            var adm = AssetBundleManager.instance;
            if (adm != null) {
                var err = adm.load_asset(bundle, path, out asset);
                if (asset == null) {
                    Debug.LogError($"load asset '{path}' in '{bundle}' failed: {err}");
                    return false;
                }
                return true;
            }
            asset = null;
            return false;
        }
    }
}