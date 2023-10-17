using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World_Formal.Adventure
{
    public class BonusView : MonoBehaviour {

        public TextMeshProUGUI talent_point_text;

        public void init(UpgradeBonus bonus)
        {
            talent_point_text.text = $"获得{bonus.talent_point}点冒险点数";
        }

        public void destroy()
        {
            Destroy(gameObject);
        }
    }
}
