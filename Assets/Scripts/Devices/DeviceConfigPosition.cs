
using UnityEngine;

namespace Devices {
    public class DeviceConfigPosition : DeviceConfig {

        public Transform target;
        public Transform from;


        public Vector2 get_position() {
            var position = (Vector2)(target ?? transform).position;
            if (from != null) {
                position = from.InverseTransformPoint(position);
            }
            return position;
        }
    }
}
