using Devices;
using UnityEngine;
using Worlds.Missions.Battles;

namespace DeviceViews
{
    public class DeviceRotation : ProviderView {

        public Transform target;

        public override void init(Device device, bool need_tick = true) {
            if (target != null) {
                device.try_get_provider(component_name ?? string.Empty, out m_provider);
            }
        }


        public void Update() {
            set_target();
        }


        void set_target()
        {
            if (m_provider != null)
            {
                var dir = m_provider.direction;
                target.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-dir.y, dir.x));
            }
        }


        private IDirectionProvider m_provider;
    }
}
