using Foundation;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.Caravans.Devices;

namespace World_Formal.WareHouse
{
    public class DeviceWareHouseView : MonoBehaviour, IDeviceWareHouseMgrView
    {
        public List<WareHouseDeviceView> deviceViews = new();

        public WareHouseCellView[,] cellViews = new WareHouseCellView[12,6];        //只记录一页的格子

        public WareHouseCellView cellPrefab;

        public WareHouseDeviceView devicePrefab;

        public DeviceWareHouseManager owner;

        public Transform devicecontent;

        public TextMeshProUGUI page_text;

        [HideInInspector]
        public uint page = 1;
        void IModelView<DeviceWareHouseManager>.attach(DeviceWareHouseManager owner)
        {
            if(this.owner == null)
            {
                this.owner = owner;
            }
            this.page = owner.page_index;
            for (uint i = 0; i < 12; i++)
            {
                for (uint j = 0; j < 6; j++)
                {
                    var cell = Instantiate(cellPrefab, transform, false);
                    cellViews[i, j] = cell;
                    cell.init(owner, owner.page_index, i, j);
                    cell.gameObject.SetActive(true);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());     //强制在物体添加的那一整就使用gridlayoutgroup来排列

            SetPage(page);
        }

        void IModelView<DeviceWareHouseManager>.detach(DeviceWareHouseManager owner)
        {
            if (owner != null)
            {
                owner = null;
            }
        }

        void IModelView<DeviceWareHouseManager>.shift(DeviceWareHouseManager old_owner, DeviceWareHouseManager new_owner)
        {

        }

        void IDeviceWareHouseMgrView.init_warehouse(DeviceWareHouseManager owner)
        {
           
        }

        void IDeviceWareHouseMgrView.set_page(uint page)
        {
            SetPage(page);
        }
        
        private void SetPage(uint page)
        {
            this.page = page;
            owner.page_index = page;    
            page_text.text = $"{page + 1}/99";
                                            //重置页面的格子信息
            for (uint i = 0; i < 12; i++)
            {
                for (uint j = 0; j < 6; j++)
                {
                    cellViews[i, j].reset(page, owner.cells[page, i, j].occupied);
                }
            }                               

            owner.devices.TryGetValue(page, out var list);
            if (list != null)               //重置页面内的设备
            {
                foreach (var deviceView in deviceViews)
                {
                    Destroy(deviceView.gameObject);
                }
                deviceViews.Clear();
                foreach (var device in list)
                {
                    var d = Instantiate(devicePrefab, devicecontent.transform, false);
                    d.transform.position = cellViews[device.row, device.col].transform.position;
                    d.init(device.device, page, device.row, device.col, device.rotatoed, owner);
                    deviceViews.Add(d);
                }
            }
        }

        void IDeviceWareHouseMgrView.put_device(uint page, uint row, uint col, Device device, bool rotatoed)
        {
            if (this.page != page)
                return;         //别的页面设备增加,外观上无需表现

            var d = Instantiate(devicePrefab, devicecontent, false);
            d.transform.position = cellViews[row, col].transform.position;
            d.init(device,page,row,col,rotatoed,owner);
            deviceViews.Add(d);
        }

        void IDeviceWareHouseMgrView.remove_device(uint page, uint row, uint col)
        {
            if (this.page != page)
                return;         //别的页面设备减少,外观上无需表现
            for(int i = 0; i < deviceViews.Count; i++)          //删除就跳出的话,正序也没关系
            {
                var d = deviceViews[i];
                if(d.page  == page && d.row == row && d.col == col)
                {
                    Destroy(d.gameObject);
                    deviceViews.RemoveAt(i);
                    break;
                }
            }
        }

        public void GoNextPage(int go)
        {
            if((page+go)<0||(page+go)>=99)
            {
                return;
            }
            int p = (int)(page + go);
            owner.page_index = (uint)p;

            SetPage((uint)p);
        }
    }
}
