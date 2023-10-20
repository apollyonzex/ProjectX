using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Rooms
{
    public class RoomCreator : Creator
    {
        public int num;
        public RoomView model;

        public float radius;

        public Vector2 wh_min;
        public Vector2 wh_max;

        RoomMgr mgr;

        //==================================================================================================

        public override void @do()
        {
            mgr = new RoomMgr(Config.RoomMgr_Name);

            foreach (var cell in init_and_get_cells())
            {
                load_wh(cell);
                load_pos(cell);
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


        void load_pos(Room cell)
        {
            var length = EX_Utility.rnd_float(0, radius);
            var rad = EX_Utility.rnd_float(0, 360) * Mathf.Deg2Rad;

            var dir = EX_Utility.convert_rad_to_dir(rad);
            var pos = dir * length;

            var offset_dir = -dir * cell.wh.magnitude / 2; 
            pos += offset_dir;

            cell.pos = pos;
        }


        void load_wh(Room cell)
        {
            var w = EX_Utility.rnd_float(wh_min.x, wh_max.x);
            var h = EX_Utility.rnd_float(wh_min.y, wh_max.y);

            cell.wh = new Vector2(w, h);
        }
        
    }
}

