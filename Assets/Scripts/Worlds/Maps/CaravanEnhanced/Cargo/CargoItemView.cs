using Foundation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Worlds.Missions.Battles;

namespace CaravanEnhanced
{
    public class CargoItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public Item data;

        [HideInInspector]
        public CargoMgr owner;

        public CargoItemInfo info;

        public Transform CargoMgrPanel;

        private RectTransform canvsRect;

        public void init(Item item, CargoMgr owner)
        {
            this.data = item;
            this.owner = owner;
            AssetBundleManager.instance.load_asset<Sprite>("caravan", item.icon_path, out var asset);
            GetComponent<Image>().sprite = asset;
            canvsRect = GetComponentInParent<Canvas>().transform as RectTransform;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            info.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position - new Vector3(-1F, 1F, 0);
            info.set_info(data.name, data.description, data.current_hp, data.hp);
            info.gameObject.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            info.gameObject.SetActive(false);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            Worlds.WorldState.instance.mission.caravanMgr.HighLightSlot(data);
            info.gameObject.SetActive(false);
            transform.SetParent(CargoMgrPanel);
            GetComponent<Image>().raycastTarget = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector3 pos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.enterEventCamera, out pos);
            GetComponent<RectTransform>().position = pos;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            var caravanMgr = Worlds.WorldState.instance.mission.caravanMgr;
            caravanMgr.CancelHighLight();
            var obj = eventData.pointerCurrentRaycast.gameObject.GetComponent<CaravanMgrView>();
            if (obj != null)
            {
                var results = obj.raycast_to_rawimage(eventData.position);
                foreach (var r in results)
                {
                    var model_slot = r.transform.GetComponent<CaravanModelSlot>();
                    if (model_slot != null)
                    {
                        if (model_slot.owner.InstallItem(model_slot.data, data, out var origin_item))
                        {
                            owner.RemoveItem(data);
                            if (origin_item != null)
                            {
                                owner.AddItem(origin_item);
                            }
                            break;
                        }
                    }

                    //拓展：
                    if (r.transform.TryGetComponent<EX_Device_Slot_View>(out var ex_slot_view))
                    {
                        Transform t = ex_slot_view.transform;
                        Item item = CaravanFunc.TryMakeItem(ex_slot_view.id);
                        var p_slot = ex_slot_view.transform.GetComponentInParent<CaravanModelSlot>().data;
                        
                        Vector2 pos = t.localPosition;
                        pos += p_slot.position;
                        var rotation = t.localRotation;
                        rotation *= p_slot.rotation;

                        caravanMgr.add_slot(pos, rotation, p_slot.type, item);
                    }
                }
                transform.SetParent(CargoMgrPanel.GetComponent<CargoMgrView>().content);
                GetComponent<Image>().raycastTarget = true;
                return;
            }
            transform.SetParent(CargoMgrPanel.GetComponent<CargoMgrView>().content);
            GetComponent<Image>().raycastTarget = true;
        }
    }
}
