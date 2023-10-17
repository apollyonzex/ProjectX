

using UnityEngine;

namespace CaravanEnhanced {
    public class Slot {
        public AutoCode.Tables.Item.e_slotType type;
        public Item item;
        public Vector2 position;
        public Quaternion rotation;

        public string bone_name;

        public Slot(Slot data) {
            type = data.type;
            item = data.item;
            position = data.position;
            rotation = data.rotation;
            bone_name = data.bone_name;
        }
        public Slot() {

        }

        public Vector2 direction => (rotation * Vector3.forward).normalized;
    }
}
