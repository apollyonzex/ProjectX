using System.Collections.Generic;
using UnityEngine;


namespace DeviceViews {
    public interface IProvider {
        object[] component_return_prms { get; }
    }


    public interface IProjectileProvider : IProvider {

    }


    public interface IDirectionProvider : IProvider {
        Vector2 direction { get; }
    }

    public interface IVesselProvider : IProvider {
        int value { get; }

        int max_value { get; }
    }

    public interface IActiveProvider: IProvider {
        bool value { get; }
    }
    public interface IProgressProvider : IProvider {
        float progress { get; }
    }

    public interface IVector2Provider : IProvider {
        float x { get; }
        float y { get; }
    }

    public abstract class ProviderView : MonoBehaviour {

        public abstract void init(Devices.Device device, bool need_tick = true);

        public string component_name;

        public object[] component_return_prms;
    }
    public class DeviceView : MonoBehaviour {
        public List<DeviceCollision> colliders  = new();

        public void init(Devices.Device device, bool need_tick = true) {
            foreach (var view in GetComponents<ProviderView>()) {
                view.init(device, need_tick);
            }
            foreach(var collider in colliders) {
                collider.init(device, false);
            }
        }

    }
}
