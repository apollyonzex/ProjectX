using AutoCode.Tables;
using Common;

namespace World
{
    public class DB : Singleton<DB>
    {
        Card m_card;
        public Card card
        {
            get
            {
                if (m_card == null)
                {
                    EX_Utility.try_load_table("card", out m_card);
                }

                return m_card;
            }
        }
    }
}

