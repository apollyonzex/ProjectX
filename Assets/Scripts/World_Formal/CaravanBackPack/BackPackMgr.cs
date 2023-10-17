using Common_Formal;
using Foundation;
using World_Formal.Caravans;
using World_Formal.Caravans.Devices;

namespace World_Formal.CaravanBackPack
{
    public interface IBackPackView : IModelView<BackPackMgr>
    {
        void init_backpack();
        void unlocked_area(int index);
        void put_device(int row, int col, bool rotatoed, Device device);
        void remove_device(int row, int col);
    }


    public class BackPackMgr : Model<BackPackMgr, IBackPackView>
    {

        public CaravanMgr_Formal owner;

        public Change_Device_Helper helper;

        //=================================================================================================
        public CaravanBackPackData data;

        private int default_row = 9, default_col = 14;

        public BackPackMgr(CaravanMgr_Formal owner,CaravanBackPackData data)
        {
            this.owner = owner;
            this.data = data;
        }

        public  void init_helper(Change_Device_Helper helper)
        {
            this.helper = helper;
        }

        public void init()
        {
            foreach (var view in views)
            {
                view.init_backpack();
            }
        }
        /// <summary>
        /// 把坐标转换为索引
        /// </summary>
        private int pos2index(int row, int col)
        {
            return row * default_col + col;
        }

        private bool try_put_device(int row, int col, Device device, out bool rotatoed)
        {

            var vert = device._desc.f_size.Item1;
            var hori = device._desc.f_size.Item2;

            rotatoed = false;

            for (int i = row; i < row + vert; i++)
            {
                for (int j = col; j < col + hori; j++)
                {
                    if (pos2index(i, j) >= 126)
                    {
                        var temp = hori;
                        hori = vert;
                        vert = temp;
                        rotatoed = true;
                        break;
                    }
                    var cell = data.cells[pos2index(i, j)];
                    if (cell.locked || cell.occupied || i >= default_row || j >= default_col)
                    {
                        var temp = hori;
                        hori = vert;
                        vert = temp;
                        rotatoed = true;            //先正着放,不行就把物体旋转
                    }
                }
            }
            if (rotatoed)
            {
                for (int i = row; i < row + vert; i++)
                {
                    for (int j = col; j < col + hori; j++)
                    {
                        if (pos2index(i, j) >= 126)
                            return false;
                        var cell = data.cells[pos2index(i, j)];
                        if (cell.locked || cell.occupied || i >= default_row || j >= default_col)
                        {
                            return false;           //旋转后还不行就是真的不行
                        }
                    }
                }
            }
            //此时行了

            for (int i = row; i < row + vert; i++)
            {
                for (int j = col; j < col + hori; j++)
                {
                    var cell = data.cells[pos2index(i, j)];
                    cell.occupied = true;
                }
            }
            data.items.Add(new CaravanBackPackItemData(device, (row, col), rotatoed, (vert, hori)));
             
            return true;
        }

        /// <summary>
        /// 尝试找位置放置item
        /// </summary>
        public bool put_device(Device device)
        {
            for (int i = 0; i < default_row; i++)
            {
                for (int j = 0; j < default_col; j++)
                {
                    if (try_put_device(i, j, device, out bool rotatoed))
                    {
                        foreach (var view in views)
                        {
                            view.put_device(i, j, rotatoed, device);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 向指定位置放item
        /// </summary>

        public bool put_device(int row, int col, Device device)
        {

            if (try_put_device(row, col, device, out bool rotatoed))
            {
                foreach (var view in views)
                {
                    view.put_device(row, col, rotatoed, device);
                }
                return true;
            }
            return false;
        }

        public bool remove_device(int row, int col)
        {

            for (int i = data.items.Count - 1; i >= 0; i--)
            {
                if (data.items[i].position == new UnityEngine.Vector2Int(row, col))
                {
                    var item = data.items[i];
                    data.items.RemoveAt(i);

                    var scale_x = item.scale.x;
                    var scale_y = item.scale.y;

                    for (int x = item.position.x; x < item.position.x + scale_x; x++)
                    {
                        for (int y = item.position.y; y < item.position.y + scale_y; y++)
                        {
                            data.cells[pos2index(x, y)].occupied = false;
                        }
                    }
                    break;
                }
            }
            foreach (var view in views)
            {
                view.remove_device(row, col);
            }
            return true;
        }

        public bool unlock_area(int index)
        {

            data.areas[index].Locked = false;

            foreach (var cell in data.areas[index].cellList)
            {
                var x = cell.position.x;
                var y = cell.position.y;
                data.cells[pos2index(x, y)].locked = false;
            }

            foreach (var view in views)
            {
                view.unlocked_area(index);
            }
            return true;
        }
    }
}

