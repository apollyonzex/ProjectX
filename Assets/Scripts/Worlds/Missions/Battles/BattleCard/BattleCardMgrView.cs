using UnityEngine;
using System.Collections.Generic;
using Foundation;
using Worlds.CardSpace;
using UnityEngine.UI;


namespace Worlds.Missions.Battles {
    public class BattleCardMgrView : MonoBehaviour, IBattleCardMgrView {

        public BattleCardMgr owner;

        public SliderAndCount soul_slider;

        public SliderAndCount card_slider;

        public Transform content;

        public BattleCardView prefab;

        public CardNotify notify;

        public List<BattleCardView> cards_view_list = new List<BattleCardView>();
        void IModelView<BattleCardMgr>.attach(BattleCardMgr owner) {
            this.owner = owner;
        }

        void IModelView<BattleCardMgr>.detach(BattleCardMgr owner) {
            if (this.owner != null) {
                owner = null;
            }
            Destroy(gameObject);
        }


        void IBattleCardMgrView.draw_card(BattleCardMgr owner, BattleCard card) {
            card_slider.count.text = $"{owner.cards_inhand.Count}";
            var g = Instantiate(prefab, content, false);
            g.init(this, card, cards_view_list.Count);
            cards_view_list.Add(g);
            g.gameObject.SetActive(true);
        }

        void IBattleCardMgrView.drop_card(BattleCardMgr owner, int index) {
            card_slider.count.text = $"{owner.cards_inhand.Count}";
            Destroy(cards_view_list[index].gameObject);
            cards_view_list.RemoveAt(index);
            for(int i = 0; i < cards_view_list.Count; i++) {
                cards_view_list[i].index = i;   //重新分配i
            }
        }

        void IModelView<BattleCardMgr>.shift(BattleCardMgr old_owner, BattleCardMgr new_owner) {
            
        }

        void IBattleCardMgrView.use_card(BattleCardMgr owner, int index) {
            card_slider.count.text = $"{owner.cards_inhand.Count}";
            Destroy(cards_view_list[index].gameObject);
            cards_view_list.RemoveAt(index);
            for (int i = 0; i < cards_view_list.Count; i++) {
                cards_view_list[i].index = i;   //重新分配i
            }
        }

        void IBattleCardMgrView.add_card_to_hand(BattleCardMgr owner, BattleCard card) {
            card_slider.count.text = $"{owner.cards_inhand.Count}";

            var g = Instantiate(prefab, content, false);
            g.init(this, card, cards_view_list.Count);
            cards_view_list.Add(g);
            g.gameObject.SetActive(true);
        }

        void IBattleCardMgrView.soul_point_change(BattleCardMgr owner, int value) {
            soul_slider.count.text = $"{value}";
        }

        void IBattleCardMgrView.notified_player(BattleCardMgr owner, string info) {
            CancelInvoke("close_notify");
            notify.gameObject.SetActive(true);
            notify.notify(info);
            Invoke("close_notify",3f);
        }

        private void close_notify() {
            notify.gameObject.SetActive(false);
        }

        void IBattleCardMgrView.card_process_change(BattleCardMgr owner,int value) {
            card_slider.slider.value = value / 1000f;
        }

        void IBattleCardMgrView.soul_process_change(BattleCardMgr owner,int value) {
            soul_slider.slider.value = value / 1000f;
        }

        void IBattleCardMgrView.change_card(BattleCardMgr owner, int index, BattleCard card) {
            cards_view_list[index].init(this,card,index);
        }

        void IBattleCardMgrView.add_card_to_deck(BattleCardMgr owner, BattleCard card) {
            //卡组加了新卡
            Debug.Log("卡组里面加新卡啦");
        }
    }
}
