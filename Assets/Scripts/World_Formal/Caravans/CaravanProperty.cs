using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Caravans
{
    public class CaravanProperty
    {
        public float max;
        public float min;
        public float value;

        public void change_value(float delta)
        {
            value = Mathf.Clamp(value + delta,min,max);
        }
    }

}
