using Common;

namespace CaravanEnhanced
{
    public class DB : Singleton<DB>
    {
        AutoCode.Tables.Item m_items;
        public AutoCode.Tables.Item items
        {
            get
            {
                Common.Expand.Utility.try_load_table_without_running("item", "item", out m_items);
                return m_items;
            }
        }
    }
}

