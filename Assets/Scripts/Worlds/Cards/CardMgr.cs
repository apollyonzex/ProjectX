using CaravanEnhanced;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;


namespace Worlds.CardSpace {
    public class CardMgr {

        public List<Card> cards = new List<Card>();

        public void AddCard(uint id,Item item ) {
            var data = TryGetCardData(id);
            Card card = new Card {
                id = data.f_id,
                name = data.f_name,
                description = data.f_desc,
                image = data.f_image,
                rank = data.f_rank,
                cost = data.f_cost,
                owner = item,
            };
            CardUtility.converter.convert(data.f_player_status.Item1, out var obj, out var err_msg);
            if (err_msg != null)
                UnityEngine.Debug.LogError($"{data.f_name} 的卡牌的 player_status 有问题,{err_msg}");
            card.player_status = (obj as IJudge, data.f_player_status.Item2);
            CardUtility.converter.convert(data.f_draw_func, out var draw_obj, out var err_msg2);
            if (err_msg2 != null)
                UnityEngine.Debug.LogError($"{data.f_name} 的卡牌的 draw_func 有问题,{err_msg2}");
            card.draw_func = draw_obj as IFunc;
            CardUtility.converter.convert(data.f_use_func, out var use_obj, out var err_msg3);
            if (err_msg3 != null)
                UnityEngine.Debug.LogError($"{data.f_name} 的卡牌的 use_func 有问题,{err_msg3}");
            card.use_func = use_obj as IFunc;
            CardUtility.converter.convert(data.f_add_func, out var add_obj, out var err_msg4);
            if (err_msg4 != null)
                UnityEngine.Debug.LogError($"{data.f_name} 的卡牌的 add_func 有问题,{err_msg4}");
            card.add_func = add_obj as IFunc;
            CardUtility.converter.convert(data.f_use_success_func, out var use_success_obj, out var err_msg5);
            if (err_msg5 != null)
                UnityEngine.Debug.LogError($"{data.f_name} 的卡牌的 use_success_func 有问题,{err_msg5}");
            card.use_success_func = use_success_obj as IFunc;
            CardUtility.converter.convert(data.f_use_fail_func, out var use_fail_obj, out var err_msg6);
            if (err_msg6 != null)
                UnityEngine.Debug.LogError($"{data.f_name} 的卡牌的 use_fail_func 有问题,{err_msg6}");
            card.use_fail_func = use_fail_obj as IFunc;
            cards.Add(card);
        }

        public void RemoveCard(uint id,Item item) {
            for(int i = 0; i < cards.Count; i++) {
                if (cards[i].id == id && cards[i].owner == item) {
                    cards.RemoveAt(i);
                    break;
                }
            }
        }

        public AutoCode.Tables.Card.Record TryGetCardData(uint id) {
            AutoCode.Tables.Card card = new AutoCode.Tables.Card();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "card", out var asset);
            card.load_from(asset);
            card.try_get(id, out var t);
            return t;
        }
    }
}
