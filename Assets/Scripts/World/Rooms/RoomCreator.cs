using Common;
using UnityEngine;

namespace World.Rooms
{
    public class RoomCreator : MonoBehaviour
    {
        public int num;
        
        RoomMgr mgr;

        //==================================================================================================

        public void @do()
        {
            Base_Utility.try_load_asset(("rooms", "temp"), out RoomView asset);
            mgr = new RoomMgr(Config.RoomMgr_Name);

            for (int i = 0; i < num; i++)
            {
                var view = Instantiate(asset, transform);
                mgr.add_cell(i, view);
            }
        }
    }
}

