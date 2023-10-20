using Common;

namespace World.Rooms
{
    public class RoomCreator : Creator
    {
        public int num;
        public RoomView model;

        RoomMgr mgr;

        //==================================================================================================

        public override void @do()
        {
            mgr = new RoomMgr(Config.RoomMgr_Name);

            for (int i = 0; i < num; i++)
            {
                var view = Instantiate(model, transform);
                mgr.add_cell(i, view);
            }
        }
    }
}

