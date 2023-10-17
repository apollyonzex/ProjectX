
using UnityEngine;
using Devices;

namespace DeviceViews {
    public class DeviceBoolView : ProviderView {

        public GameObject gameobject_view;

        private IActiveProvider m_provider;
        public override void init(Device device, bool need_tick = true) {
            if (gameobject_view != null) {
                device.try_get_provider(component_name ?? string.Empty, out m_provider);
                gameobject_view.SetActive(m_provider.value);
            }
        }
        public void Update() {
            update_view();
        }

        void update_view() {
            if (m_provider != null) {
                gameobject_view.SetActive(m_provider.value);
            }
        }
    }
}
