using UnityEngine;

namespace World_Formal.Caravans.Devices
{
    public class Device_Offset_From_Slot_View : MonoBehaviour
    {
        public Transform dir;
        public Transform view;
        public float dir_offset = 0;

        private void Start()
        {
            if (dir == null) return;

            Vector2 dir_pos = dir.localPosition;

            view.localRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-dir_pos.y, dir_pos.x));
            view.localRotation *= Quaternion.Euler(0, 0, dir_offset);
        }
    }
}
