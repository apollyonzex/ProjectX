﻿using UnityEngine;
using TMPro;

namespace Common_Formal.Localization
{
    public class LanguageLoad_String : MonoBehaviour
    {
        public string id;
        TextMeshProUGUI cpn;

        //==================================================================================================

        private void OnEnable()
        {
            cpn = transform.GetComponent<TextMeshProUGUI>();
            LanguageMgr.instance.load += load;

            load();
        }


        private void OnDisable()
        {
            LanguageMgr.instance.load -= load;
        }


        void load()
        {
            cpn.text = LanguageMgr.instance.get_text(id);
        }

    }
}

