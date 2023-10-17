using UnityEngine;


namespace DeviceViews
{
    public class Device_Normal_View : MonoBehaviour
    {
        public Transform dir;
        public Transform view;
        public float dir_offset = -90;

        private void Start()
        {
            Vector2 dir_pos = dir.localPosition;

            view.localRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-dir_pos.y, dir_pos.x));
            view.localRotation *= Quaternion.Euler(0, 0, dir_offset);
        }
    }
}
