using Foundation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.UI.GridLayoutGroup;

namespace Worlds.Missions.Battles {
    public class BattleCardView : MonoBehaviour ,IPointerEnterHandler,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerExitHandler{

        public Text _name;

        public Text _cost;

        public Text desc;

        public Image _image;

        public Image _cardTitleImage;

        RaycastHit2D m_hit;


        public void init(BattleCardMgrView owner , BattleCard data,int index) {
            this.owner = owner;
            this.data = data;
            var card_data = data.raw_data;
            _name.text = card_data.name;
            _cost.text = $"{card_data.cost}";
            desc.text = card_data.description;
            AssetBundleManager.instance.load_asset<Sprite>(card_data.image.Item1, card_data.image.Item2,out var _sprite);
            _image.sprite = _sprite;
            if (_image.sprite == null) {
                _image.gameObject.SetActive(false);
            }
            SetRank(card_data.rank);
            this.index = index;
            canvsRect = GetComponentInParent<Canvas>().transform as RectTransform;
        }


        private void SetRank(int rank) {
            AssetBundleManager.instance.load_asset<Sprite>("card", "card_ui/title_" + rank, out var _card_title_sprite);
            if (_card_title_sprite != null) {
                _cardTitleImage.sprite = _card_title_sprite;
                return; 
            }
            AssetBundleManager.instance.load_asset<Sprite>("card", "card_ui/title_0", out var sprite);
            if(sprite == null) {
                Debug.LogWarning($"默认卡牌的title_0失效,请检查一下");
            }
            _cardTitleImage.sprite = sprite;
        }


        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            StopCoroutine("shrink");
            StartCoroutine("zoom");
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            StopCoroutine("zoom");
            StartCoroutine("shrink");

            start = transform.parent;
            transform.SetParent(owner.transform);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.enterEventCamera, out var pos);
            GetComponent<RectTransform>().position = pos;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            var focus_pos = BattleSceneRoot.instance.focus.pos;
            var data_pos = eventData.pointerCurrentRaycast.worldPosition;
            var caravan_pos = BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.position;
            var pos = data_pos + new Vector3(focus_pos.x, focus_pos.y,0);
            var device_pos = new Vector3();
            foreach (var slot in BattleSceneRoot.instance.battleMgr.device_mgr.caravan_mgr.slots) {
                if(slot.item == data.raw_data.owner) {
                    device_pos = slot.position + caravan_pos;
                }
            }

            owner.owner.use_card_position = pos - device_pos;
            //owner.owner.use_card_position = pos - new Vector3(caravan_pos.x,caravan_pos.y,0);
            Debug.DrawRay(pos, transform.right,Color.red,1f);

            if (try_combine_cards(eventData)) return;

            transform.SetParent(start);
            owner.owner.UseCard(index);
            
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            StopCoroutine("zoom");
            StartCoroutine("shrink");
        }

        private RectTransform canvsRect;

        private Transform start;
        [HideInInspector]
        public int index;

        public BattleCard data;

        public BattleCardMgrView owner;

        IEnumerator zoom() {
            while(transform.localScale.x <= 1.1f) {
                transform.localScale = transform.localScale + new Vector3(0.01f, 0.01f, 0.01f);
                yield return null;
            }
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        IEnumerator shrink() {
            while (transform.localScale.x > 1f) {
                transform.localScale = transform.localScale - new Vector3(0.01f, 0.01f, 0.01f);
                yield return null;
            }
            transform.localScale = new Vector3(1f, 1f, 1f);
        }


        /// <summary>
        /// 尝试组合卡牌
        /// </summary>
        bool try_combine_cards(PointerEventData eventData)
        {
            GraphicRaycaster gr = WorldSceneRoot.instance.uiRoot.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new();
            gr.Raycast(eventData, results);

            List<BattleCardView> views = new();
            foreach (var e in results)
            {
                var view = e.gameObject.GetComponentInParent<BattleCardView>();
                if (view == null) continue;
                views.Add(view);
            }
            if (views.Count != 2) return false;

            var id1 = views[0].data.raw_data.id;
            var id2 = views[1].data.raw_data.id;

            if (!DB.instance.card_combine.try_get(id1, out var r)) return false;
            if (!r.f_able_id.Contains(id2)) return false;

            var result_id = r.f_result_id;

            var func = views[0].data.raw_data.use_func;
            views[0].data.raw_data.use_func = null;
            views[0].transform.SetParent(start);
            views[0].owner.owner.UseCard(views[0].index);
            views[0].data.raw_data.use_func = func;

            func = views[1].data.raw_data.use_func;
            views[1].data.raw_data.use_func = null;
            views[1].transform.SetParent(start);
            views[1].owner.owner.UseCard(views[1].index);
            views[1].data.raw_data.use_func = func;

            owner.owner.SendCardToHand(new BattleCard { raw_data = owner.owner.GetCard(result_id) });

            return true;
        }
    }
}