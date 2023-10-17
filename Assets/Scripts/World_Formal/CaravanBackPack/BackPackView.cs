using Foundation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.Caravans.Devices;

namespace World_Formal.CaravanBackPack
{
    public class BackPackView : MonoBehaviour, IBackPackView
    {
        BackPackMgr owner;

        public BackPackCellView[,] cellArray = new BackPackCellView[9, 14];
        public Transform content;
        public Transform itemcontent;
        public BackPackCellView cellPrefab;
        public BackPackItemView itemPrefab;
        public List<BackPackItemView> itemList = new();
        //==================================================================================================

        void IModelView<BackPackMgr>.attach(BackPackMgr owner)
        {
            this.owner = owner;

            var data = owner.data;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    var cell = data.cells[i * 14 + j];
                    var view = Instantiate(cellPrefab, content, false);
                    view.init(owner, i, j, cell.occupied, cell.locked);
                    view.gameObject.SetActive(true);
                    cellArray[i, j] = view;
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());     //强制在物体添加的那一整就使用gridlayoutgroup来排列
            foreach (var area in data.areas)
            {
                int min_h = 999, max_h = -1, min_v = 999, max_v = -1;
                foreach (var cell in area.cellList)
                {
                    min_h = Mathf.Min(min_h, cell.position.x);
                    max_h = Mathf.Max(max_h, cell.position.x);
                    min_v = Mathf.Min(min_v, cell.position.y);
                    max_v = Mathf.Max(max_v, cell.position.y);
                }
                for (int i = min_h; i <= max_h; i++)
                {
                    cellArray[i, min_v].left.SetActive(true);
                    cellArray[i, max_v].right.SetActive(true);
                }
                for (int i = min_v; i <= max_v; i++)
                {
                    cellArray[min_h, i].top.SetActive(true);
                    cellArray[max_h, i].bottom.SetActive(true);
                }
            }

            foreach(var item in data.items)
            {
                put_device_view(item.position.x, item.position.y, item.rotatoed, item.device);
            }
        }


        void IModelView<BackPackMgr>.detach(BackPackMgr owner)
        {
            this.owner = null;
        }


        void IModelView<BackPackMgr>.shift(BackPackMgr old_owner, BackPackMgr new_owner)
        {
        }

        void IBackPackView.init_backpack()
        {
           
        }

        void IBackPackView.unlocked_area(int index)
        {
            foreach (var cell in owner.data.areas[index].cellList)
            {
                var x = cell.position.x;
                var y = cell.position.y;
                cellArray[x, y].SetUnlocked();
            }
        }

        void IBackPackView.put_device(int row, int col, bool rotatoed, Device device)
        {
            put_device_view(row, col, rotatoed, device);
        }

        private void put_device_view(int row, int col, bool rotatoed, Device device)
        {
            var it = Instantiate(itemPrefab, itemcontent, false);
            it.transform.position = cellArray[row, col].transform.position;
            it.init(device, row, col, rotatoed, owner);
            itemList.Add(it);
        }

        void IBackPackView.remove_device(int row, int col)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var it = itemList[i];
                if (it.row == row && it.col == col)
                {
                    Destroy(it.gameObject);
                    itemList.RemoveAt(i);
                    break;
                }
            }
        }

    }
}

