using UnityEngine;
using Devices;

namespace DeviceViews {
    public class DeviceScale : ProviderView {

        public GameObject gameObject_view;

        private IVector2Provider m_provider;
        public override void init(Device device, bool need_tick = true) {
            if (gameObject != null) {
                device.try_get_provider(component_name ?? string.Empty, out m_provider);
                gameObject_view.transform.localScale = new Vector3(m_provider.x, m_provider.y, 1);
            }
        }

        public void Update() {
            update_view();
        }

        void update_view() {
            if (m_provider != null) {
                gameObject_view.transform.localScale = new Vector3(m_provider.x, m_provider.y, 1);
            }
        }
    }
}
