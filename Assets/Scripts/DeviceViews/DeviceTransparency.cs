using UnityEngine;
using Devices;

namespace DeviceViews {
    public class DeviceTransparency : ProviderView {

        public GameObject gameObject_view;

        private IProgressProvider m_provider;
        public override void init(Device device, bool need_tick = true) {
            if (gameObject != null) {
                device.try_get_provider(component_name ?? string.Empty, out m_provider);
                gameObject_view.TryGetComponent<SpriteRenderer>(out var sprite_renderer);
                if (sprite_renderer != null) {
                    var color = sprite_renderer.color;
                    sprite_renderer.color = new Color(color.r, color.g, color.b, m_provider.progress);
                }
            }
        }

        public void Update() {
            update_view();
        }

        void update_view() {
            if (m_provider != null) {
                gameObject_view.TryGetComponent<SpriteRenderer>(out var sprite_renderer);
                if (sprite_renderer != null) {
                    var color = sprite_renderer.color;
                    sprite_renderer.color = new Color(color.r, color.g, color.b, m_provider.progress);
                }
            }
        }
    }
}
