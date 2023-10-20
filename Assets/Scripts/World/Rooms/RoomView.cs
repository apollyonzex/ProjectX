using Foundation;
using UnityEngine;

namespace World.Rooms
{
    public class RoomView : MonoBehaviour, IRoomView
    {
        public RoomMgr mgr;
        public Room cell;

        //==================================================================================================

        void IModelView<RoomMgr>.attach(RoomMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<RoomMgr>.detach(RoomMgr mgr)
        {
            this.mgr = null;
        }

        
        void IModelView<RoomMgr>.shift(RoomMgr old_mgr, RoomMgr new_mgr)
        {
        }


        void IRoomView.notify_on_init(Room cell)
        {
            this.cell = cell;
            transform.localPosition = cell.pos;
        }
    }
}

