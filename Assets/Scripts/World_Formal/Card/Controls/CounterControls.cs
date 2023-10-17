using Common_Formal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.Caravans;
using World_Formal.Card;

namespace World_Formal.Card.Controls
{
    public class CounterControls : CardController
    {
        public Slider slider;

        public string value_name;

        public override void tick()
        {
            Mission.instance.try_get_mgr("CaravanMgr", out CaravanMgr_Formal caravanMgr);
            caravanMgr.caravan_property.TryGetValue(value_name, out var value);
            slider.value = value.value - value.min;
            slider.maxValue = value.max -  value.min;
        }

    }
}
