using UnityEngine;

namespace World_Formal.Caravans.Slots
{
    public class Slot
    {
        public AutoCode.Tables.Device.e_slotType type;

        public Vector2 position;
        public Quaternion rotation;
        public uint device_id;
        public string bone_name;
        public Vector2 bone_pos;

        //==================================================================================================

        public Slot(CaravanEnhanced.CaravanMakerSlotData asset)
        {
            this.position = asset.position;
            this.rotation = asset.rotation;
            this.type = asset.type;
            this.device_id = asset.item_id;
            this.bone_name = asset.bone_name;
            this.bone_pos = asset.bone_pos;
        }
    }
}

