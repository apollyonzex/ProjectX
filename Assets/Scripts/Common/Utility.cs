
using UnityEngine;
using Foundation;

namespace Common {

    public static class Utility {

        public static float round_to_pixel(float value) {
            var ppu = Config.current.scaled_pixel_per_unit;
            return Mathf.Round(value * ppu) / ppu;
        }

        public static Vector2 round_to_pixel(Vector2 value) {
            var ppu = Config.current.scaled_pixel_per_unit;
            var inv = 1f / ppu;
            value.x = Mathf.Round(value.x * ppu) * inv;
            value.y = Mathf.Round(value.y * ppu) * inv;
            return value;
        }

        public static RectInt min_max_to_rect_int(Vector2 min, Vector2 max) {
            var a = Vector2Int.FloorToInt(min);
            var b = Vector2Int.CeilToInt(max);
            return new RectInt(a, Vector2Int.Max(b - a, Vector2Int.one));
        }

        public static T load_and_instantiate_component_from_prefab<T>(string bundle, string path, Transform parent = null, bool in_world_space = true) where T : Component {
            var adm = AssetBundleManager.instance;
            if (adm == null) {
                return null;
            }
            var err = adm.load_asset(bundle, path, out GameObject prefab);
            if (prefab == null) {
                Debug.LogError($"load {path} in {bundle} failed: {err}");
                return null;
            }

            var component = prefab.GetComponent<T>();
            if (component == null) {
                Debug.LogError($"invalid prefab {path} in {bundle}: component {typeof(T)} dose not exist");
                return null;
            }
            if (parent != null) {
                return Object.Instantiate(component, parent, in_world_space);
            }
            return Object.Instantiate(component);
        }

        public static T load_asset<T>(string bundle, string path) where T : Object {
            var adm = AssetBundleManager.instance;
            if (adm == null) {
                return null;
            }
            var err = adm.load_asset(bundle, path, out T asset);
            if (asset == null) {
                Debug.LogError($"load asset {path} in {bundle} failed: {err}");
            }
            return asset;
        }

        public static T load_asset<T>((string bundle, string path) addr) where T : Object {
            return load_asset<T>(addr.bundle, addr.path);
        }

        public static Sprite load_icon((string bundle, string path) addr) {
            if (addr != (string.Empty, string.Empty)) {
                return load_asset<Sprite>(addr);
            }
            return null;
        }

        public static Color32 parse_hex_color(string s) {
            byte r = 0xFF;
            byte g = 0xFF;
            byte b = 0xFF;
            byte a = 0xFF;
            try {
                r = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(s.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                a = byte.Parse(s.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            } catch (System.Exception) {

            }
            return new Color32(r, g, b, a);
        }
    }

}