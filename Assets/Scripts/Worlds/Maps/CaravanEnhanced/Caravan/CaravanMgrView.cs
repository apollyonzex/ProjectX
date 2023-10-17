using Foundation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CaravanEnhanced
{
    public class CaravanMgrView : MonoBehaviour,ICaravanMgrView,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler,IPointerMoveHandler{

        private bool pressing;

        public CaravanMgr owner;

        public RawImage rendertexture;

        public CaravanModel model;

        public Image moving_item;
        void IModelView<CaravanMgr>.attach(CaravanMgr owner) {
            this.owner = owner;
        }

        void IModelView<CaravanMgr>.detach(CaravanMgr owner) {
            if(owner != null) {
                owner = null;
            }
            Destroy(gameObject);
        }

        void ICaravanMgrView.initialization(CaravanMgr owner) {
            model = Worlds.WorldSceneRoot.instance.Open_UI_Prefab<CaravanModel>("caravan", "prefabs/CaravanModel", Worlds.WorldSceneRoot.instance.MonoLayer.transform);
            model.transform.localPosition = new Vector3(5000, 0, 0);
            model.init(owner);                                          //初始化模型
            owner.model = model;
        }

        void ICaravanMgrView.notified_item_install(CaravanMgr owner, Slot slot, Item item) {
            Debug.Log("装!");
            model.InstallItem(slot, item);
        }

        void ICaravanMgrView.notified_item_remove(CaravanMgr owner, Slot slot) {
            model.RemoveItem(slot);
        }

        void IModelView<CaravanMgr>.shift(CaravanMgr old_owner, CaravanMgr new_owner) {
            
        }

        public RaycastHit2D[] raycast_to_rawimage(Vector2 pos) {            //点击位置由rendertexture传到model位置
            var canvas = GetComponentInParent<Canvas>();
            var model_camera = model.modelCamera;
            var ui_camera = canvas.worldCamera;

            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform,pos,ui_camera,out var click_pos_in_rawimage)) {
                float imageWidth = rendertexture.rectTransform.rect.width;
                float imageHeight = rendertexture.rectTransform.rect.height;

                float localPositionX = rendertexture.rectTransform.localPosition.x;
                float localPositionY = rendertexture.rectTransform.localPosition.y;
                
                float p_x = (click_pos_in_rawimage.x - localPositionX) / imageWidth;
                float p_y = (click_pos_in_rawimage.y - localPositionY) / imageHeight;

                var ray = model_camera.ViewportPointToRay(new Vector2(p_x, p_y));
                var results = Physics2D.RaycastAll(ray.origin, ray.direction);

                return results;
            }
            return null;
        }

        void ICaravanMgrView.notified_highlight_slot(CaravanMgr owner, Item item) {
            model.highlight(item);
        }

        void ICaravanMgrView.notified_cancel_highlight(CaravanMgr owner) {
            model.cancelhighlight();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                var results = raycast_to_rawimage(eventData.position);
                foreach (var r in results) {
                    var model_slot = r.transform.GetComponent<CaravanModelSlot>();
                    if (model_slot != null && model_slot.data.item != null) {
                        owner.RemoveItem(model_slot.data, out var remove_item);
                        if (remove_item != null) {
                            Worlds.WorldState.instance.mission.cargoMgr.AddItem(remove_item);
                        }
                    }
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            pressing = true;
            var results = raycast_to_rawimage(eventData.position);
            foreach (var r in results) {
                var model_slot = r.transform.GetComponent<CaravanModelSlot>();
                if (model_slot != null && model_slot.data.item != null) {
                    var data = new Slot(model_slot.data);
                    owner.RemoveItem(model_slot.data, out var remove_item);
                    if (remove_item == null) {
                        continue;
                    }
                    owner.hovered_slot = data;
                    AssetBundleManager.instance.load_asset<Sprite>("caravan", data.item.icon_path, out var sprite);
                    moving_item.sprite = sprite;
                    moving_item.gameObject.SetActive(true);
                    return;
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {

            bool success = false;

            var results = raycast_to_rawimage(eventData.position);              //重新装回槽位
            foreach (var r in results) {
                var model_slot = r.transform.GetComponent<CaravanModelSlot>();
                if (model_slot != null && owner.hovered_slot != null) {
                    if(owner.InstallItem(model_slot.data, owner.hovered_slot.item, out var origin_item)) {
                        if (origin_item != null) {
                            Worlds.WorldState.instance.mission.cargoMgr.AddItem(origin_item);
                        }
                        success = true;
                        break;
                    }
                }
            }
            
            if (!success){          //松手但是安装失败,直接送回仓库
                if (owner.hovered_slot != null) {
                    owner.hovered_slot.item.owner = null;
                    Worlds.WorldState.instance.mission.cargoMgr.AddItem(owner.hovered_slot.item);
                }
            }

            pressing = false;
            owner.hovered_slot = null;
            moving_item.gameObject.SetActive(false);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
            if (pressing && owner.hovered_slot!=null) {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.enterEventCamera, out var pos);
                moving_item.GetComponent<RectTransform>().position = pos;
            }
        }

    }
}
