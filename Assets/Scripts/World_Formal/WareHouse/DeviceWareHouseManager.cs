using Common_Formal;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans.Devices;

namespace World_Formal.WareHouse
{
    public interface IDeviceWareHouseMgrView : IModelView<DeviceWareHouseManager>
    {
        void init_warehouse(DeviceWareHouseManager owner);
        void set_page(uint page);
        void put_device(uint page,uint row,uint col,Device device,bool rotatoed);
        void remove_device(uint page,uint row,uint col);
    }



    public class DeviceWareHouseManager : Model<DeviceWareHouseManager,IDeviceWareHouseMgrView> ,IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public DeviceWareHouseManager(string name,params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }

        void IMgr.init(params object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);
        }
        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }

        //===================================================================================================
        /// <summary>
        /// 目前仓库大小是固定的99 * 12 * 6
        /// </summary>
        public WareHouseCell[, , ] cells = new WareHouseCell[99, 12, 6];
        public uint page_index = 0;
        /// <summary>
        /// 某页中的设备
        /// </summary>
        public Dictionary<uint, List<WareHouseDevice>> devices = new();

        public Change_Device_Helper helper;


        public void init_helper(Change_Device_Helper helper)
        {
            this.helper = helper;
        }

        public void init_warehouse()
        {
            for(uint i = 0; i < cells.GetLength(0); i++)
            {
               for(uint j = 0; j < cells.GetLength(1); j++)
                {
                    for(uint k = 0; k < cells.GetLength(2); k++)
                    {
                        cells[i, j, k] = new WareHouseCell()
                        {
                            occupied = false,
                            page = i,
                            row = j,
                            col = k,
                        };
                    }
                }
            }

            for(uint i = 0; i < 99; i++)
            {
                devices.Add(i, new List<WareHouseDevice>());
            }

            page_index = 0;

            foreach(var view in views)
            {
                view.init_warehouse(this);
            }
        }
        /// <summary>
        /// 尝试往设备仓库放置设备
        /// </summary>
        public bool try_put_device(uint page,uint row,uint col,Device device,out bool rotatoed)
        {
            if(device._desc == null)
            {
                rotatoed = false;
                return false;
            }
            var vert = device._desc.f_size.Item1;
            var hori = device._desc.f_size.Item2;

            rotatoed = false;

            for(uint i = row; i < row + vert; i++)
            {
                for(uint j = col; j < col + hori; j++)
                {
                    if( (!position_is_logic(i, j))|| cells[page, i, j].occupied)     
                    {
                        var temp = hori;
                        hori = vert;
                        vert = hori;
                        rotatoed = true;
                        break;
                    }
                }
            }

            if (rotatoed)
            {
                for (uint i = row; i < row + vert; i++)
                {
                    for (uint j = col; j < col + hori; j++)
                    {
                        if ((!position_is_logic(i, j)) || cells[page, i, j].occupied)
                        {
                            return false;               //转了之后还不行就是真的不行
                        }
                    }
                }
            }

            for(uint i = row; i < row + vert; i++)
            {
                for(uint j = col; j < col + hori; j++)
                {
                    cells[page, i, j].occupied = true;
                }
            }

            devices.TryGetValue(page,out var list);
            list.Add(new WareHouseDevice(device,page,row,col,rotatoed,(vert,hori)));

            foreach(var view in views)
            {
                view.put_device(page,row,col,device,rotatoed);
            }

            return true;
        }

        public bool try_remove_device(uint page, uint row, uint col)
        {
            devices.TryGetValue((uint)page,out var list);   //获取这一页的设备队列
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].row == row && list[i].col == col)
                {
                    var item = list[i];
                    list.RemoveAt(i);

                    var scale_x = item.scale.x;
                    var scale_y = item.scale.y;

                    for(uint x = item.row; x < item.row + item.scale.x; x++)
                    {
                        for(uint y = item.col; y < item.col + item.scale.y; y++)
                        {
                            cells[page, x, y].occupied = false;
                        }
                    }

                    foreach(var view in views)
                    {
                        view.remove_device(page, row, col);
                    }

                    return true;
                }
            }
            return false;
        }


        public void remove_all_devices()
        {
            List<(uint page, uint row, uint col)> dels = new();
            foreach (var (page, list) in devices.Where(t => t.Value.Any()))
            {
                foreach (var e in list)
                {
                    dels.Add((page, e.row, e.col));
                }
            }

            foreach (var (page, row, col) in dels)
            {
                try_remove_device(page, row, col);
            }
        }


        /// <summary>
        /// 判断这个位置是否合理
        /// </summary>s>
        private bool position_is_logic(uint row,uint col)
        {
            if (row >= 0 && row < cells.GetLength(1) && col >= 0 && col < cells.GetLength(2))
                return true;
            return false;
        }

        public bool put_device(Device device)
        {
            for(uint i =  0; i < 99; i++)
            {
                for(uint j = 0; j < 12; j++)
                {
                    for(uint k = 0; k < 6; k++)
                    {
                        if(try_put_device(i,j,k,device,out var rotatoed))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class WareHouseCell
    {
        public bool occupied;

        public uint page;

        public uint row, col;
    }

    public class WareHouseDevice
    {
        public Device device;
        public Vector2Int scale;
        public uint page, row, col;
        public bool rotatoed;

        public WareHouseDevice(Device device, uint page, uint row, uint col, bool rotatoed, (int vert, int hori) value)
        {
            this.device = device;
            this.page = page;
            this.row = row;
            this.col = col;
            this.rotatoed = rotatoed;
            scale = new Vector2Int(value.vert, value.hori);
        }
    }
}
