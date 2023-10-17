using UnityEngine;

namespace Common_Formal.Localization
{
    public class LanguageMgr : Singleton<LanguageMgr>
    {
        public System.Action load;

        //==================================================================================================

        public string get_text(string id)
        {
            string type = PlayerPrefs.GetString("Language", "EN");
            if (!DB.instance.string_lz.try_get(id, out var record)) return "";

            switch (type)
            {
                case "CN":
                    return record.f_zh;

                case "EN":
                    return record.f_en;
            }

            return "";
        }


        public string get_text(uint id)
        {
            string type = PlayerPrefs.GetString("Language", "EN");
            if (!DB.instance.uint_lz.try_get(id, out var record)) return "";

            switch (type)
            {
                case "CN":
                    return record.f_zh;

                case "EN":
                    return record.f_en;
            }

            return "";
        }
    }
}
