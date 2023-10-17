using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeviceSpine_Formal
{
    public class Spine_Info
    {
        public string name;
        public bool loop;

        public Spine_Info(string name, bool loop)
        {
            this.name = name;
            this.loop = loop;
        }
    }
}

