using UnityEditor;
using UnityEngine;
using Common;

namespace Inits
{
    public class LanguageMgr : Singleton<LanguageMgr>
    {
        public System.Action load;
        AutoCode.Tables.Localize m_localize;

        //==================================================================================================

        public LanguageMgr()
        {
            Common.Expand.Utility.try_load_table("localize", out m_localize);
        }


        public string get_text(string id)
        {
            string type = PlayerPrefs.GetString("Language", "EN");
            if (!m_localize.try_get(id, out var record)) return "";

            switch (type)
            {
                case "CN":
                    return record.f_CN;

                case "EN":
                    return record.f_EN;
            }

            return "";
        }
    }
}
