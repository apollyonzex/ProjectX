using CaravanEnhanced;
using Common;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using UnityEngine;
using Worlds.CardSpace;


namespace Worlds.Missions.Battles {
    public interface IBattleCardMgrView : IModelView<BattleCardMgr> {
        void card_process_change(BattleCardMgr owner,int current_value);
        void soul_process_change(BattleCardMgr owner,int current_value);
        void soul_point_change(BattleCardMgr owner, int current_value);
        void draw_card(BattleCardMgr owner,BattleCard card);
        void use_card(BattleCardMgr owner, int index);
        void drop_card(BattleCardMgr owner, int index);
        void add_card_to_hand(BattleCardMgr owner, BattleCard card);
        void add_card_to_deck(BattleCardMgr owner, BattleCard card);
        void notified_player(BattleCardMgr owner, string info);
        void change_card(BattleCardMgr owner, int index, BattleCard card);
    }
    public class BattleCardMgr : Model<BattleCardMgr,IBattleCardMgrView>{


        public Vector2? use_card_position = null;

        public int max_soul_point = 9;

        public int current_soul_point = 0;

        private int soul_point_increasing = 0;

        public int card_draw_process = 180;

        public List<BattleCard> cards_inhand = new List<BattleCard>();

        public List<BattleCard> deck = new List<BattleCard>();

        public List<BattleCard> cards_in_graveyard = new List<BattleCard>();

        public void Duel() {
            InitCard();
            card_draw_process = Config.current.card_draw_interval;
            BattleSceneRoot.physics_tick += on_physics_tick;
        }

        public void EndDuel() {
            BattleSceneRoot.physics_tick -= on_physics_tick;
        }

        public void on_physics_tick() {
            soul_point_increasing += Config.current.soul_increase_rate;

            while(soul_point_increasing >= 100_000) {
                if (current_soul_point < max_soul_point) {
                    soul_point_increasing -= 100_000;
                    current_soul_point += 1;

                    foreach(var view in views) {
                        view.soul_point_change(this, current_soul_point);
                    }
                } else {
                    soul_point_increasing = 100_000;
                    break;
                }
            }
            foreach(var view in views) {
                view.soul_process_change(this,soul_point_increasing);
            }

            card_draw_process -= 1;

            if (card_draw_process <= 0) {   //try to draw card
                if (DrawCard()) {
                    card_draw_process = Config.current.card_draw_interval;
                } else {
                }
            }

            var current_value =(int)((float)(Config.current.card_draw_interval - (card_draw_process>=0?card_draw_process:0))/ Config.current.card_draw_interval * 1e5f);   //转换成百分比
            foreach (var view in views) {
                view.card_process_change(this,current_value);
            }
        }


        public void InitCard() {
            foreach(var card in Worlds.WorldState.instance.mission.cardMgr.cards) {
                deck.Add(new BattleCard{raw_data = card });
            }
            for(int i = 0; i < Config.current.card_num_init; i++) {
                DrawCard();
            }
            foreach(var view in views) {
                view.soul_point_change(this,current_soul_point);
            }
        }


        public bool DrawCard() {
            if (deck.Count == 0 && cards_in_graveyard.Count == 0) {

                if(card_draw_process == 0) {
                    foreach (var view in views) {
                        view.notified_player(this, "没卡了");
                    }
                }
                return false;
            }
            else if(cards_inhand.Count >= Config.current.card_num_max) {
                if (card_draw_process == 0) {               //只有第一次会提醒
                    foreach (var view in views) {
                        view.notified_player(this, "阿sir别抽了,拿不下了");
                    }
                }
                return false;
            }


            if (deck.Count != 0) {          //直接抽卡组

            } else {                        //先把墓地放回卡组
                foreach(var _card in cards_in_graveyard) {
                    deck.Add(_card);
                }
                cards_in_graveyard.Clear();
            }

            var num = Random.Range(0, deck.Count);
            var card = deck[num];
            deck.RemoveAt(num);


            if (card.raw_data.draw_func != null) {
                card.raw_data.draw_func.exec(card);
            }

            cards_inhand.Add(card);
            foreach (var view in views) {
                view.draw_card(this, card);
            }
            return true;
        }

        public void SendCardToHand(BattleCard card) {     //这个命名是预防以后可以塞卡进墓地或者卡组
            cards_inhand.Add(card);

            if (card.raw_data.add_func != null) {
                card.raw_data.add_func.exec(card);
            }



            foreach(var view in views) {
                view.add_card_to_hand(this,card);
            }
        }

        public void SendCardToHand(int card_id,Item owner) {
            var card = GetCard((uint)card_id);
            card.owner = owner;
            SendCardToHand(new BattleCard { raw_data = card });
        }

        public void SendCardToDeck(int card_id, Item owner) {
            var card = GetCard((uint)card_id);
            card.owner = owner;
            SendCardToDeck(new BattleCard { raw_data = card });
        }

        public void SendCardToDeck(BattleCard card) {
            cards_in_graveyard.Add(card);

            foreach(var view in views) {
                view.add_card_to_deck(this, card);
            }
        }

        public void RemoveCardinGY(int card_id) {
            for(int i = cards_in_graveyard.Count - 1; i>=0; i--) {
                if (cards_in_graveyard[i].raw_data.id == card_id) {
                    cards_in_graveyard.RemoveAt(i);
                    return;
                }
            }
        }

        public bool RemoveCardinGY(BattleCard card) {
            for (int i = cards_in_graveyard.Count - 1; i >= 0; i--) {
                if (cards_in_graveyard[i] == card) {
                    cards_in_graveyard.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void UseCard(int index) {

            var card = cards_inhand[index];

            var data = card.raw_data;

            if (current_soul_point < data.cost) {
                foreach(var view in views) {
                    view.notified_player(this, "能量不够,使用失败");
                }
                return;
            }

            if (data.player_status.Item1 != null) {
                if (!data.player_status.Item1.result()) {

                    foreach(var view in views) {
                        view.notified_player(this, data.player_status.Item2);
                    }
                    return;
                }
            }
            
            if (data.use_func != null) {
                if (data.use_func.exec(card)) {         //执行成功
                    current_soul_point -= data.cost;
                    cards_inhand.RemoveAt(index);
                    cards_in_graveyard.Add(card);
                    
                    if(data.use_success_func !=null)
                        data.use_success_func.exec(card);


                    foreach (var view in views) {
                        view.use_card(this, index);
                    }

                    foreach (var view in views) {
                        view.soul_point_change(this, current_soul_point);
                    }

                } else {
                    current_soul_point -= data.cost;
                    cards_inhand.RemoveAt(index);
                    cards_in_graveyard.Add(card);

                    if (data.use_fail_func != null)
                        data.use_fail_func.exec(card);


                    foreach (var view in views) {
                        view.use_card(this, index);
                    }

                    foreach (var view in views) {
                        view.soul_point_change(this, current_soul_point);
                    }
                }
            } else {
                current_soul_point -= data.cost;
                cards_inhand.RemoveAt(index);
                cards_in_graveyard.Add(card);

                foreach (var view in views) {
                    view.use_card(this, index);
                }

                foreach (var view in views) {
                    view.soul_point_change(this, current_soul_point);
                }
            }
        }

        public void UseCardWithOutCost(int index) {
            var card = cards_inhand[index];

            var data = card.raw_data;

            if (data.player_status.Item1 != null) {
                if (!data.player_status.Item1.result()) {

                    foreach (var view in views) {
                        view.notified_player(this, data.player_status.Item2);
                    }
                    return;
                }
            }

            if (data.use_func != null) {
                if (data.use_func.exec(card)) {         //执行成功

                    cards_inhand.RemoveAt(index);
                    cards_in_graveyard.Add(card);

                    if (data.use_success_func != null)
                        data.use_success_func.exec(card);

                    foreach (var view in views) {
                        view.use_card(this, index);
                    }

                } else {
                    if (data.use_fail_func != null) {
                        cards_inhand.RemoveAt(index);
                        cards_in_graveyard.Add(card);

                        if (data.use_fail_func != null)
                            data.use_fail_func.exec(card);


                        foreach (var view in views) {
                            view.use_card(this, index);
                        }
                    }
                }
            }
        }


        public void DropCard(int index) {
            var card = cards_inhand[index];
            cards_inhand.RemoveAt(index);
            cards_in_graveyard.Add(card);

            foreach (var view in views) {
                view.drop_card(this, index);
            }
        }

        public void ChangeCard(int index,BattleCard new_card) {

            cards_inhand[index] = new_card;

            foreach(var view in views) {
                view.change_card(this, index, new_card);
            }
        }

        public void RemoveCard(int index) {
            cards_inhand.RemoveAt(index);
            foreach (var view in views) {
                view.drop_card(this, index);
            }
        }
        public void RemoveCard(BattleCard card) {
            var index = cards_inhand.IndexOf(card); 
            cards_inhand.Remove(card);
            foreach (var view in views) {
                view.drop_card(this, index);
            }
        }


        public Card GetCard(uint id) {
            AutoCode.Tables.Card rec = new AutoCode.Tables.Card();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "card", out var asset);
            rec.load_from(asset);
            rec.try_get(id, out var data);
            if (data == null) {
                return null;
            }
            Card card = new Card {
                id = data.f_id,
                name = data.f_name,
                description = data.f_desc,
                image = data.f_image,
                rank = data.f_rank,
                cost = data.f_cost,
                owner = null,
            };

            CardUtility.converter.convert(data.f_player_status.Item1, out var obj, out var err_msg);
            if (err_msg != null)
                Debug.LogError($"{data.f_name} 的卡牌的 player_status 有问题,{err_msg}");
            card.player_status = (obj as IJudge, data.f_player_status.Item2);
            CardUtility.converter.convert(data.f_draw_func, out var draw_obj, out var err_msg2);
            if (err_msg2 != null)
                Debug.LogError($"{data.f_name} 的卡牌的 draw_func 有问题,{err_msg2}");
            card.draw_func = draw_obj as IFunc;
            CardUtility.converter.convert(data.f_use_func, out var use_obj, out var err_msg3);
            if (err_msg3 != null)
                Debug.LogError($"{data.f_name} 的卡牌的 use_func 有问题,{err_msg3}");
            card.use_func = use_obj as IFunc;
            CardUtility.converter.convert(data.f_add_func, out var add_obj, out var err_msg4);
            if (err_msg4 != null)
                Debug.LogError($"{data.f_name} 的卡牌的 add_func 有问题,{err_msg4}");
            card.add_func = add_obj as IFunc;
            CardUtility.converter.convert(data.f_use_success_func, out var use_success_obj, out var err_msg5);
            if (err_msg5 != null)
                Debug.LogError($"{data.f_name} 的卡牌的 use_success_func 有问题,{err_msg5}");
            card.use_success_func = use_success_obj as IFunc;
            CardUtility.converter.convert(data.f_use_fail_func, out var use_fail_obj, out var err_msg6);
            if (err_msg6 != null)
                Debug.LogError($"{data.f_name} 的卡牌的 use_fail_func 有问题,{err_msg6}");
            card.use_fail_func = use_fail_obj as IFunc;

            return card;
        }
    }






}
