using System.Collections.Generic;
using System;
using Foundation;
using Common_Formal;
using World_Formal.Caravans;
using Foundation.Tables;

namespace World_Formal.Card
{

    public interface ICardManagerView : IModelView<CardManager>
    {
        void init();
        void tick();
        void update_card_area(int index);       //更新index号手牌区
        void use_card(int area_index, int card_slot_index);

        void drop_card(int area_index, int card_slot_index);
    }
    public class CardManager : Model<CardManager, ICardManagerView>,IMgr
    {

        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public CardManager(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }

        void IMgr.init(params object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);
        }

        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }

        //============================================================================================

        private const int max_qi_process = 100_0000;

        public int qi_num;

        public int qi_process;

        public List<Card> draw_cards = new List<Card>();

        public List<CardArea> card_areas = new List<CardArea>();    //手牌区

        public List<CardController> card_controllers = new();

        public List<Card> drop_cards = new List<Card>();


        #region 卡组整理

        public virtual void AddCardByID(uint id)
        {
            AutoCode.Tables.Cards cards = new AutoCode.Tables.Cards();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "cards", out var asset);
            cards.load_from(asset);
            cards.try_get(id, out var t);
            draw_cards.Add(new Card(t));
        }

        public virtual void AddCard(Card card)
        {
            draw_cards.Add(card);
        }

        public virtual void RemoveCardByID(uint id)
        {

        }

        public virtual void RemoveCard()
        {

        }
        /// <summary>
        /// 初始化卡组
        /// </summary>
        /// <param name="cvMgr"></param>
        public virtual void init_cards(CaravanMgr_Formal cvMgr)
        {
            draw_cards.Clear();
            drop_cards.Clear();


            var caravan = cvMgr.cell;
            foreach(var card in caravan._desc.f_cards)
            {
                AddCardByID(card);
            }
        }

        /// <summary>
        /// 洗抽卡堆
        /// </summary>
        public void ShuffleDrawCards()
        {
            System.Random random = new System.Random();
            List<Card> temp = new List<Card>();
            foreach (var card in draw_cards)
            {
                temp.Insert(random.Next(temp.Count + 1), card);
            }
            draw_cards = temp;
        }

        #endregion


        #region 战斗中卡牌的逻辑
        /// <summary>
        /// 自然增长気
        /// </summary>
        public void GainQi()
        {
            qi_process += Common.Config.current.qi_rising_rate;

            while(qi_process >= max_qi_process)
            {
                var qi_max = Common.Config.current.qi_max;
                if (qi_num < qi_max)
                {
                    qi_num += 1;
                    qi_process -= max_qi_process;
                }
                else
                {
                    qi_process = max_qi_process;
                    break;  
                }
            }
        }

        public virtual void tick() {
            GainQi();

            foreach (var view in views)
            {
                view.tick();
            }

        }
        /// <summary>
        /// 初始化战斗的卡牌系统
        /// </summary>
        public virtual void init_battle(CardManagerView cardMgrView)
        {
            foreach(var area in cardMgrView.area_list)
            {
                card_areas.Add(new CardArea(area.slots.Count));
            }

            foreach(var controller in cardMgrView.controllers)
            {
                card_controllers.Add(controller);
                controller.init();
            }

            WorldContext.instance.add_tick(Common.Config.CardMgr_Priority, Common.Config.CardMgr_Name, tick);
            ShuffleDrawCards();

            foreach (var view in views)
            {
                view.init();
            }
        }

        public virtual void end_battle()
        {
            WorldContext.instance.remove_tick(Common.Config.CardMgr_Name);

            foreach(var area in card_areas)         //清空所有手牌区
            {
                foreach(var slot in area.slots)
                {
                    if (slot.card != null)
                    {
                        draw_cards.Add(slot.card);
                        slot.card = null;
                    }
                }
            }

            foreach (var drop_card in drop_cards)   //清空弃牌区
            {
                draw_cards.Add(drop_card);
            }
            drop_cards.Clear();
            card_areas.Clear();
            card_controllers.Clear();

            qi_num = 0;
            qi_process = 0;
        }
        /// <summary>
        /// 用索引抽卡
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="area_index"></param>
        /// <returns></returns>
        public virtual Card DrawCard(CardController controller, int area_index)
        {
            if (draw_cards.Count == 0 && drop_cards.Count == 0)
            {
                return null;
            }
            else if (draw_cards.Count == 0 && drop_cards.Count != 0)
            {
                foreach (var drop_card in drop_cards)
                {
                    draw_cards.Add(drop_card);
                }
                drop_cards.Clear();
                ShuffleDrawCards();
            }
            var card = draw_cards[0];
            if (card_areas[area_index].put_card(card))
            {
                draw_cards.RemoveAt(0);
                foreach (var view in views)
                {
                    view.update_card_area(0);
                }
                return card;
            }
            return null;
        }
        /// <summary>
        /// 直接用区域抽卡
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public virtual Card DrawCard(CardController controller,CardArea area) {
            if (draw_cards.Count == 0 && drop_cards.Count == 0)
            {
                return null;
            }
            else if (draw_cards.Count == 0 && drop_cards.Count != 0)
            {
                foreach (var drop_card in drop_cards)
                {
                    draw_cards.Add(drop_card);
                }
                drop_cards.Clear();
                ShuffleDrawCards();
            }
            var card = draw_cards[0];
            if (area.put_card(card))
            {
                draw_cards.RemoveAt(0);
                foreach (var view in views)
                {
                    view.update_card_area(0);
                }
                return card;
            }
            return null;
        }

        /// <summary>
        /// area卡区的卡被使用了
        /// </summary>
        /// <param name="area"></param>
        /// <param name="card"></param>
        /// <returns></returns>


        public virtual bool UseCard(int area_index,int slot_index)
        {
            Card card = card_areas[area_index].slots[slot_index].card;
            
            if (qi_num >= card.desc.f_cost)
            {
                qi_num -= card.desc.f_cost;
                drop_cards.Add(card);
                card_areas[area_index].slots[slot_index].card = null;
                return true;
            }
            return false;
        }

        public virtual bool DropCard(int area_index, int slot_index) {
            Card card = card_areas[area_index].slots[slot_index].card;
            drop_cards.Add(card);
            card_areas[area_index].slots[slot_index].card = null; 
            return true; 
        }
        #endregion

    }
}
