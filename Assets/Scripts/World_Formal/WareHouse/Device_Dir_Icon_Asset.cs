using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.WareHouse
{
    [CreateAssetMenu(fileName = "Device_Pos_Icon_Asset", menuName = "ScriptObejct/Device_Pos_Icon_Asset", order = 3)]
    public class Device_Dir_Icon_Asset : ScriptableObject
    {
        public Device_Dir_Icon_Data[] datas;
    }


    [System.Serializable]
    public class Device_Dir_Icon_Data
    {
        public int dir;
        public Sprite icon;
    }
}

