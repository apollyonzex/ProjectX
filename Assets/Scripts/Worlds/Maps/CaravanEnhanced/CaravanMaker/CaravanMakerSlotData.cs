using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CaravanEnhanced {

    [System.Serializable]
    public class CaravanMakerSlotData {
        public AutoCode.Tables.Device.e_slotType type;
        public uint item_id;
        public Vector2 position;
        public Quaternion rotation;
        public bool horizontal_flip;
        public bool vertical_flip;
        public string bone_name;
        public Vector2 bone_pos;
    }
}
