using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoCode.Tables;

namespace Worlds.Missions.Battles
{
    public class DB : Singleton<DB>
    {
        CardCombine m_card_combine;
        public CardCombine card_combine
        {
            get
            {
                if (m_card_combine == null)
                {
                    Common.Expand.Utility.try_load_table("card_combine", out m_card_combine);
                }

                return m_card_combine;
            }
        }


        Card m_card;
        public Card card
        {
            get
            {
                if (m_card == null)
                {
                    Common.Expand.Utility.try_load_table("card", out m_card);
                }

                return m_card;
            }
        }


        Item m_item;
        public Item item
        {
            get
            {
                if (m_item == null)
                {
                    Common.Expand.Utility.try_load_table("item", out m_item);
                }

                return m_item;
            }
        }
    }

}

