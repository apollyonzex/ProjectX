using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Assets.Scripts.Temp {
    public class RimgToUi : MonoBehaviour  {

        /*public bool trigger_rawimage_button(Vector2 pos) {
            var canvas = GetComponentInParent<Canvas>();
            var ui_camera = canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos, ui_camera, out var click_pos_in_rawimage)) {

                var rendertexture = GetComponent<RawImage>();

                float imageWidth = rendertexture.rectTransform.rect.width;
                float imageHeight = rendertexture.rectTransform.rect.height;

                float localPositionX = 0;
                float localPositionY = 0;

                float p_x = (click_pos_in_rawimage.x - localPositionX) / imageWidth;
                float p_y = (click_pos_in_rawimage.y - localPositionY) / imageHeight;

         //       var model_camera = Worlds.WorldState.instance.mission.travelView.textureCamera;

                PointerEventData pointerData = new PointerEventData(EventSystem.current);
      //          pointerData.position = model_camera.ViewportToScreenPoint(new Vector2(p_x, p_y));
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach(var r in results) {
                    if (r.gameObject.TryGetComponent<Button>(out var t)) {
                        t.onClick.Invoke();
                    }
                }

            }

            return false;
        }



        public RaycastHit2D[] raycast_to_rawimage(Vector2 pos) {            //点击位置由rendertexture传到model位置
            var canvas = GetComponentInParent<Canvas>();
            var ui_camera = canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos, ui_camera, out var click_pos_in_rawimage)) {

                var rendertexture = GetComponent<RawImage>();

                float imageWidth = rendertexture.rectTransform.rect.width;
                float imageHeight = rendertexture.rectTransform.rect.height;

                *//*                float localPositionX = rendertexture.rectTransform.localPosition.x;
                                float localPositionY = rendertexture.rectTransform.localPosition.y;*//*               //还待学习
                float localPositionX = 0;
                float localPositionY = 0;

                float p_x = (click_pos_in_rawimage.x - localPositionX) / imageWidth;
                float p_y = (click_pos_in_rawimage.y - localPositionY) / imageHeight;

         //       var model_camera = Worlds.WorldState.instance.mission.travelView.textureCamera;

                PointerEventData pointerData = new PointerEventData(EventSystem.current);
    //            pointerData.position = model_camera.ViewportToScreenPoint(new Vector2(p_x, p_y));
                List<RaycastResult> results = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pointerData, results);

      //          var ray = model_camera.ViewportPointToRay(new Vector2(p_x, p_y));           //如果目标是ui的话,就不应该使用射线检测   吗?

                Debug.DrawRay(ray.origin, ray.direction, Color.magenta,10f);

                var rresults = Physics2D.RaycastAll(ray.origin, ray.direction);

                return rresults;
            }
            return null;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            trigger_rawimage_button(eventData.position);
        }*/
    }
}

