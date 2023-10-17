using Foundation;
using Foundation.Tables;
using UnityEditor;
using UnityEngine;
using Worlds.Missions.Battles;

namespace Worlds.CardSpace {
    public class BattleCardWindow :EditorWindow {
        [MenuItem("EditorWindow/CardWindow/BattleCardWindow")]

        

        public static void ShowWindow() {
            var window = EditorWindow.GetWindow(typeof(BattleCardWindow));
        }

        int add_card_to_deck_id;

        int add_card_to_hand_id;

        public Card GetCard(uint id) {
            AutoCode.Tables.Card card = new AutoCode.Tables.Card();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "card", out var asset);
            card.load_from(asset);
            card.try_get(id, out var data);
            if (data == null) {
                return null;
            }
            Card _card = new Card {
                id = data.f_id,
                name = data.f_name,
                description = data.f_desc,
                image = data.f_image,
                rank = data.f_rank,
                cost = data.f_cost,
            };

            CardUtility.converter.convert(data.f_player_status.Item1, out var obj, out var err_msg);
            _card.player_status = (obj as IJudge, data.f_player_status.Item2);
            CardUtility.converter.convert(data.f_draw_func, out var draw_obj, out var err_msg2);
            _card.draw_func = draw_obj as IFunc;
            CardUtility.converter.convert(data.f_use_func, out var use_obj, out var err_msg3);
            _card.use_func = use_obj as IFunc;
            CardUtility.converter.convert(data.f_add_func, out var add_obj, out var err_msg4);
            _card.add_func = add_obj as IFunc;

            return _card;
        }
        public void OnGUI() {
            if (!Application.isPlaying) {
                GUILayout.Label("请先运行游戏");
                return;
            }
            if (BattleSceneRoot.instance == null) {
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("手牌:");
            foreach(var card in BattleSceneRoot.instance.battlecardMgr.cards_inhand) {
                if(card.raw_data.owner == null) {
                    GUILayout.Label($"{card.raw_data.name}   属于item:{"null"}");
                    continue;
                }
                if (card.raw_data.owner.owner == null) {
                    GUILayout.Label($"{card.raw_data.name}   属于item:{card.raw_data.owner.name}  属于槽位:{"null"}");
                    continue;
                }
                GUILayout.Label($"{card.raw_data.name}   属于item:{card.raw_data.owner.name}  属于槽位:{card.raw_data.owner.owner.type}");
            }

            GUILayout.Space(5f);
            GUILayout.Label("卡组:");
            foreach (var card in BattleSceneRoot.instance.battlecardMgr.deck) {
                GUILayout.Label($"{card.raw_data.name}");
            }
            GUILayout.Space(5f);
            GUILayout.Label("墓地:");

            foreach (var card in BattleSceneRoot.instance.battlecardMgr.cards_in_graveyard) {
                GUILayout.Label($"{card.raw_data.name}");
            }
            GUILayout.Space(5f);
            GUILayout.Label("开了就是开了?");
            GUILayout.Label("加入卡组");
            GUILayout.BeginHorizontal();
            add_card_to_deck_id = EditorGUILayout.IntField(add_card_to_deck_id);
            if (GUILayout.Button("添加")) {
                if (GetCard((uint)add_card_to_deck_id)!=null) {
                    var data = GetCard((uint)add_card_to_deck_id);
                    BattleSceneRoot.instance.battlecardMgr.deck.Add(new BattleCard { raw_data = data});
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.Label("塞入手牌(无视抽卡限制)");
            GUILayout.BeginHorizontal();
            add_card_to_hand_id = EditorGUILayout.IntField(add_card_to_hand_id);
            if (GUILayout.Button("添加")) {
                if (GetCard((uint)add_card_to_hand_id) != null) {
                    var data = GetCard((uint)add_card_to_deck_id);
                    BattleSceneRoot.instance.battlecardMgr.SendCardToHand(new BattleCard { raw_data = data });
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
