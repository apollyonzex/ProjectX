using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World_Formal.WareHouse
{
    public class WareHouseCellView : MonoBehaviour,IPointerClickHandler
    {
        public DeviceWareHouseManager owner;

        public uint page,row, col;
        public bool occupied;

        public void init(DeviceWareHouseManager owner,uint page,uint row,uint col)
        {
            this.owner = owner;
            this.page = page;
            this.row = row;
            this.col = col;
        }

        public void reset(uint page,bool occupied)
        {
            this.page = page;
            this.occupied = occupied;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var helper = owner.helper;
            var holding_device = helper.holding.Item2;
            if (holding_device != null)
            {
                if(owner.try_put_device(page,row,col,holding_device,out var rotatoed))
                {
                    helper.mgr.remove_device(helper.holding.Item1, helper.holding.Item2);
                    helper.no_holding();
                }
            }
        }
    }
}
