
using UnityEngine;
using Foundation;

namespace Common {

    public class SceneRoot<T> : MonoBehaviourSingleton<T> where T : SceneRoot<T> {

        public Camera mainCamera;
        public Camera uiCamera;
        public Canvas uiRoot;

        public virtual void make_active() {
            mainCamera.gameObject.SetActive(true);
            uiCamera.gameObject.SetActive(true);
        }

        public virtual void make_deactive() {
            mainCamera.gameObject.SetActive(false);
            uiCamera.gameObject.SetActive(false);
        }

        public Rect main_camera_rect { get; private set; }
        public float main_camera_focal_distance { get; private set; }

        public void synchronize_camera() {

            var config = Config.current;

            var width = mainCamera.pixelWidth;
            var height = mainCamera.pixelHeight;

            var aspect = (float)width / height;
            var desired_aspect = (float)config.desiredResolution.x / config.desiredResolution.y;
            float ppu_scale;
            if (aspect >= desired_aspect) {
                ppu_scale = (float)height / config.desiredResolution.y;
            } else {
                ppu_scale = (float)width / config.desiredResolution.x;
            }
            var ppu = config.pixelPerUnit * ppu_scale;
            config.scaled_pixel_per_unit = ppu;

            var size_hy = height * 0.5f / ppu;

            main_camera_focal_distance = size_hy / Mathf.Tan(config.desiredPerspectiveFOV * 0.5f * Mathf.Deg2Rad);

            mainCamera.orthographicSize = size_hy;
            uiCamera.orthographicSize = size_hy;

            var pos = mainCamera.transform.localPosition;

            var size_hx = mainCamera.aspect * size_hy;
            main_camera_rect = new Rect(pos.x - size_hx, pos.y - size_hy, size_hx * 2, size_hy * 2);
        }

        public void synchronize_camera(Vector3 world_position) {

            var config = Config.current;

            var width = mainCamera.pixelWidth;
            var height = mainCamera.pixelHeight;
            

            var aspect = (float)width / height;
            var desired_aspect = (float)config.desiredResolution.x / config.desiredResolution.y;
            float ppu_scale;
            if (aspect >= desired_aspect) {
                ppu_scale = (float)height / config.desiredResolution.y;
            } else {
                ppu_scale = (float)width / config.desiredResolution.x;
            }
            var ppu = config.pixelPerUnit * ppu_scale;
            config.scaled_pixel_per_unit = ppu;
            var inv_ppu = 1f / ppu;


            var size_hy = height * 0.5f * inv_ppu;

            main_camera_focal_distance = size_hy / Mathf.Tan(config.desiredPerspectiveFOV * 0.5f * Mathf.Deg2Rad);

            mainCamera.orthographicSize = size_hy;
            uiCamera.orthographicSize = size_hy;

            float x, y;
            if (width % 2 == 1) {
                x = (Mathf.Round(world_position.x * ppu) + 0.5f) * inv_ppu;
            } else {
                x = Mathf.Round(world_position.x * ppu) * inv_ppu;
            }
            if (height % 2 == 1) {
                y = (Mathf.Round(world_position.y * ppu) + 0.5f) * inv_ppu;
            } else {
                y = Mathf.Round(world_position.y * ppu) * inv_ppu;
            }

            mainCamera.transform.localPosition = new Vector3(x, y, world_position.z);

            var size_hx = mainCamera.aspect * size_hy;
            main_camera_rect = new Rect(x - size_hx, y - size_hy, size_hx * 2, size_hy * 2);
        }

        public void synchronize_camera(Vector3 world_position, float scale) {

            scale = Mathf.Max(1e-3f, scale);

            var config = Config.current;

            var width = mainCamera.pixelWidth;
            var height = mainCamera.pixelHeight;


            var aspect = (float)width / height;
            var desired_aspect = (float)config.desiredResolution.x / config.desiredResolution.y;
            float ppu_scale;
            if (aspect >= desired_aspect) {
                ppu_scale = (float)height / config.desiredResolution.y;
            } else {
                ppu_scale = (float)width / config.desiredResolution.x;
            }
            var ppu = config.pixelPerUnit * ppu_scale * scale;
            config.scaled_pixel_per_unit = ppu;
            var inv_ppu = 1f / ppu;


            var size_hy = height * 0.5f * inv_ppu;

            main_camera_focal_distance = size_hy / Mathf.Tan(config.desiredPerspectiveFOV * 0.5f * Mathf.Deg2Rad);

            mainCamera.orthographicSize = size_hy;
            uiCamera.orthographicSize = size_hy;

            float x, y;
            if (width % 2 == 1) {
                x = (Mathf.Round(world_position.x * ppu) + 0.5f) * inv_ppu;
            } else {
                x = Mathf.Round(world_position.x * ppu) * inv_ppu;
            }
            if (height % 2 == 1) {
                y = (Mathf.Round(world_position.y * ppu) + 0.5f) * inv_ppu;
            } else {
                y = Mathf.Round(world_position.y * ppu) * inv_ppu;
            }

            mainCamera.transform.localPosition = new Vector3(x, y, world_position.z);

            var size_hx = mainCamera.aspect * size_hy;
            main_camera_rect = new Rect(x - size_hx, y - size_hy, size_hx * 2, size_hy * 2);
        }

        public void synchronize_camera(Vector3 world_position, Rect limit) {

            var config = Config.current;

            var width = mainCamera.pixelWidth;
            var height = mainCamera.pixelHeight;


            var aspect = (float)width / height;
            var desired_aspect = (float)config.desiredResolution.x / config.desiredResolution.y;
            float ppu_scale;
            if (aspect >= desired_aspect) {
                ppu_scale = (float)height / config.desiredResolution.y;
            } else {
                ppu_scale = (float)width / config.desiredResolution.x;
            }
            var ppu = config.pixelPerUnit * ppu_scale;
            config.scaled_pixel_per_unit = ppu;
            var inv_ppu = 1f / ppu;



            var size_hy = height * 0.5f * inv_ppu;
            var size_hx = mainCamera.aspect * size_hy;

            main_camera_focal_distance = size_hy / Mathf.Tan(config.desiredPerspectiveFOV * 0.5f * Mathf.Deg2Rad);

            mainCamera.orthographicSize = size_hy;
            uiCamera.orthographicSize = size_hy;

            var min = (Vector2)world_position - new Vector2(size_hx, size_hy);
            var max = (Vector2)world_position + new Vector2(size_hx, size_hy);
            min -= limit.min;
            max -= limit.max;
            if (min.x < 0) {
                if (max.x <= 0) {
                    world_position.x -= min.x;
                } else {
                    world_position.x -= (min.x + max.x) * 0.5f;
                }
            } else if (max.x > 0) {
                world_position.x -= max.x;
            }
            if (min.y < 0) {
                if (max.y <= 0) {
                    world_position.y -= min.y;
                } else {
                    world_position.y -= (min.y + max.y) * 0.5f;
                }
            } else if (max.y > 0) {
                world_position.y -= max.y;
            }


            float x, y;
            if (width % 2 == 1) {
                x = (Mathf.Round(world_position.x * ppu) + 0.5f) * inv_ppu;
            } else {
                x = Mathf.Round(world_position.x * ppu) * inv_ppu;
            }
            if (height % 2 == 1) {
                y = (Mathf.Round(world_position.y * ppu) + 0.5f) * inv_ppu;
            } else {
                y = Mathf.Round(world_position.y * ppu) * inv_ppu;
            }

            mainCamera.transform.localPosition = new Vector3(x, y, world_position.z);
            main_camera_rect = new Rect(x - size_hx, y - size_hy, size_hx * 2, size_hy * 2);
        }


        public Vector2 world_to_ui_point(Vector2 position) {
            return position - (Vector2)mainCamera.transform.localPosition;
        }

        public Vector2 screen_to_world_point(Vector2 position) {
            return mainCamera.ScreenToWorldPoint(position);
        }
    }

}