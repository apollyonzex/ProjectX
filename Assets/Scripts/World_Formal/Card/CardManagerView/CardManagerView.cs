using Foundation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Card
{
    public class CardManagerView : MonoBehaviour, ICardManagerView
    {
        public CardManager owner;

        public List<CardAreaView> area_list = new();

        public CardQiView qiview;

        public List<CardController> controllers = new();

        void IModelView<CardManager>.attach(CardManager owner)
        {
            this.owner = owner as CardManager;
        }

        void IModelView<CardManager>.detach(CardManager owner)
        {
            if (owner != null)
            {
                owner = null;
            }
            Destroy(gameObject);
        }

        void ICardManagerView.drop_card(int area_index, int card_slot_index)
        {
            area_list[area_index].slots[card_slot_index].use_card();
        }

        void ICardManagerView.init()
        {
            foreach(var area in area_list)
            {
                area.init_area(owner);
            }
        }

        void IModelView<CardManager>.shift(CardManager old_owner, CardManager new_owner)
        {
            
        }

        void ICardManagerView.tick()
        {
            qiview.update_qi(owner.qi_process,owner.qi_num);


            foreach(var area in area_list)
            {
                area.tick();
            }

            foreach(var  controller in controllers)
            {
                controller.tick();
            }
        }

        void ICardManagerView.update_card_area(int area_index)
        {
            var logic_area = owner.card_areas[area_index];
            area_list[area_index].set_area(logic_area);
        }

        void ICardManagerView.use_card(int area_index, int card_slot_index)
        {
            qiview.update_qi(owner.qi_process, owner.qi_num);
            area_list[area_index].slots[card_slot_index].use_card();
        }
    }
}
