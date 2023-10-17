using Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Camp {
    public class CameraController : MonoBehaviour
    {
        private Camera m_Camera;
        public float mouseOffset;

        public float max_x = 7.3f, max_y = 4.2f, min_x = -7.3f, min_y = -4.2f;

        public Vector2 target_pos;

        internal bool is_lock = false;

        void Start()
        {
            m_Camera = GetComponent<Camera>();
            target_pos = transform.position;
        }

        private void Update()
        {
            if (is_lock) return;

            float moveSpeed = Config.current.basement_camera_move_speed;
            Vector3 v1 = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());

            if (v1.x <= mouseOffset)
            {
                target_pos += moveSpeed * Time.deltaTime * Vector2.left;
            }
            if (v1.x >= 1 - mouseOffset)
            {
                target_pos += moveSpeed * Time.deltaTime * Vector2.right;
            }
            if (v1.y <= mouseOffset)
            {
                target_pos += moveSpeed * Time.deltaTime * Vector2.down;
            }
            if (v1.y >= 1 - mouseOffset)
            {
                target_pos += moveSpeed * Time.deltaTime * Vector2.up;
            }

            target_pos.x = Mathf.Clamp(target_pos.x, min_x, max_x);
            target_pos.y = Mathf.Clamp(target_pos.y, min_y, max_y);

            transform.position = new Vector3(target_pos.x,target_pos.y,transform.position.z);
        }
    }
}

