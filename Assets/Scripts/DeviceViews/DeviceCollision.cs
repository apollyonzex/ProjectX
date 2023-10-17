using Devices;
using UnityEngine;

namespace DeviceViews {
    public class DeviceCollision : MonoBehaviour {

        private Device m_device;

        public Devices.DeviceCollision m_start_component;

        public Devices.DeviceCollisionStay m_stay_component;

        public Devices.DeviceCollisionExit m_exit_component;

        public string collision_name;


        public void init(Devices.Device device, bool need_tick) {
           m_device = device;
           device.try_get_component(collision_name ?? string.Empty,out m_start_component);
           device.try_get_component(collision_name ?? string.Empty, out m_stay_component);
           device.try_get_component(collision_name ?? string.Empty, out m_exit_component);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (m_start_component == null)
                    return;
            if(collision.TryGetComponent<Worlds.Missions.Battles.Enemies.IEnemyView>(out var e)) {
                m_start_component.start_collision(m_device, e);
            }
            else if(collision.TryGetComponent<Worlds.Missions.Battles.Projectiles.IProjectileView>(out var p)) {
                m_start_component.start_collision(m_device, p);
            }
        }


        private void OnTriggerStay2D(Collider2D collision) {
            if (m_stay_component == null)
                return;
            if (collision.TryGetComponent<Worlds.Missions.Battles.Enemies.IEnemyView>(out var e)) {
                m_stay_component.stay_collision(m_device, e);
            } else if (collision.TryGetComponent<Worlds.Missions.Battles.Projectiles.IProjectileView>(out var p)) {
                m_stay_component.stay_collision(m_device, p);
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (m_exit_component == null)
                return;
            if (collision.TryGetComponent<Worlds.Missions.Battles.Enemies.IEnemyView>(out var e)) {
                m_exit_component.exit_collision(m_device, e);
            } else if (collision.TryGetComponent<Worlds.Missions.Battles.Projectiles.IProjectileView>(out var p)) {
                m_exit_component.exit_collision(m_device, p);
            }
        }
    }
}
