

using Devices;
using System.Collections.Generic;
using UnityEngine;

namespace DeviceViews {
    public class DeviceVesselView : ProviderView {

        public Transform target;

        public GameObject bullet_prefab;

        private IVesselProvider m_provider;

        private List<GameObject> bullets = new List<GameObject>();
        public override void init(Device device, bool need_tick = true) {
            if (target != null&&bullet_prefab!=null) {
                device.try_get_provider(component_name ?? string.Empty, out m_provider);
                for(int i = 0; i < m_provider.max_value; i++) {
                    var g = Instantiate(bullet_prefab, target, false);
                    g.transform.localPosition = new Vector3(0.3f * i, 0, 0);
                    bullets.Add(g);
                }
            }
        }

        public void Update() {
            update_view();
        }

        void update_view() {
            if (m_provider != null) {
                for(int i = 0; i < m_provider.value; i++) {
                    var color = bullets[i].GetComponent<SpriteRenderer>().color;
                    bullets[i].GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
                }
                for(int i = m_provider.value; i < m_provider.max_value; i++) {
                    var color = bullets[i].GetComponent<SpriteRenderer>().color;
                    bullets[i].GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.3f);
                }
            }
        }

    }
}
