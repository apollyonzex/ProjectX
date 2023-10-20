using Common;
using System.Collections.Generic;

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

            foreach (var cell in init_and_get_cells())
            {
                
            }

            mgr.load_view();
        }


        /// <summary>
        /// 初始化cell
        /// </summary>
        Room init_cell(int id)
        {
            Room cell = new()
            { 
                id = id,
            };

            return cell;
        }


        /// <summary>
        /// 初始化并获取所有cell
        /// </summary>
        IEnumerable<Room> init_and_get_cells()
        {
            for (int i = 0; i < num; i++)
            {
                var view = Instantiate(model, transform);
                var cell = init_cell(i);

                mgr.add_cell(cell, view);

                yield return cell;
            }
        }
    }
}

