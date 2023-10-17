
using UnityEngine;

namespace Devices {
    public abstract class DeviceConfig : MonoBehaviour {

        public string config_name;

    }

    public static class DeviceConfigExtensions {
        public static bool get_item<T>(this DeviceConfig[] config, string name, out T item) where T : DeviceConfig {
            foreach (var e in config) {
                if (e is T t && t.config_name == name) {
                    item = t;
                    return true;
                }
            }
            item = null;
            return false;
        }
    }
}
