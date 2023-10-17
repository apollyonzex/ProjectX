using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace World_Formal.Card
{
    public class CardQiView :  MonoBehaviour
    {
        public TextMeshProUGUI qi_num;

        public Slider qi_slider;
        public void update_qi(int process,int num)
        {
            qi_slider.value = process;
            qi_num.text = $"{num}";
        }
    }
}
