using Foundation;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.Caravans.Devices;

namespace World_Formal.WareHouse
{
    public class WareHouseDeviceView : MonoBehaviour,IPointerClickHandler
    {
        public Device device;
        public DeviceWareHouseManager owner;
        [HideInInspector]
        public uint row, col,page;
        public void init(Device device,uint page,uint row,uint col,bool rotatoed,DeviceWareHouseManager owner)
        {
            this.device = device;
            this.page = page;
            this.row = row;
            this.col = col;
            this.owner = owner;
            AssetBundleManager.instance.load_asset<Sprite>(device._desc.f_icon.Item1, device._desc.f_icon.Item2, out var sprite);
            if (rotatoed)
            {
                transform.localScale = new Vector3(device._desc.f_size.Item1, device._desc.f_size.Item2, 1f);
            }
            else
            {
                transform.localScale = new Vector3(device._desc.f_size.Item2, device._desc.f_size.Item1, 1f);
            }
            GetComponent<Image>().sprite = sprite;
            gameObject.SetActive(true);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var helper = owner.helper;
            helper.has_holding(device);
            owner.try_remove_device(page,row, col);
        }
    }
}
