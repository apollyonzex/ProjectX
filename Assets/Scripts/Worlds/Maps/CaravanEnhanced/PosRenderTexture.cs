using UnityEngine;
using UnityEngine.UI;

namespace CaravanEnhanced {
    public class PosRenderTexture {

        private Vector2 ClickPosInRawImg;
        // 画布
        public Canvas canvas;
        // 预览映射相机
        public Camera previewCamera;
        public RawImage previewImage;
        public Camera UiCamera;
        public void DrawRayLine(Vector2 pos ) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pos, UiCamera, out ClickPosInRawImg)) {

                //获取预览图的长宽
                float imageWidth = previewImage.rectTransform.rect.width;
                float imageHeight = previewImage.rectTransform.rect.height;
                //获取预览图的坐标，此处RawImage的Pivot需为(0,0)，不然自己再换算下
                float localPositionX = previewImage.rectTransform.localPosition.x;
                float localPositionY = previewImage.rectTransform.localPosition.y;

                //获取在预览映射相机viewport内的坐标（坐标比例）
                float p_x = (ClickPosInRawImg.x - localPositionX) / imageWidth;
                float p_y = (ClickPosInRawImg.y - localPositionY) / imageHeight;

                //从视口坐标发射线
                Ray p_ray = previewCamera.ViewportPointToRay(new Vector2(p_x, p_y));
                RaycastHit p_hitInfo;
                if (Physics.Raycast(p_ray, out p_hitInfo)) {
                    //显示射线，只有在scene视图中才能看到
                    Debug.DrawLine(p_ray.origin, p_hitInfo.point);
                    // Debug.Log(p_hitInfo.transform.name);
                }
            }
        }
    }
}
