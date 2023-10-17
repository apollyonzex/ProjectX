using Camp.Level_Entrances;
using UnityEngine;

namespace Level_Entrance_Editor
{
    public class CellData : MonoBehaviour
    {
        public uint seq; //顺序
        public uint world_id;

        public new string name => transform.GetComponent<Level_EntranceView>().name.text;

        //==================================================================================================

        public void init(uint world_id)
        {
            this.world_id = world_id;
        }
    }
}

