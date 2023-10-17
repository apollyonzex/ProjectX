using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Foundation;
using World_Formal.Caravans.Devices;

namespace World_Formal.CaravanBackPack
{
    public class BackPackItemView : MonoBehaviour, IPointerClickHandler
    {

        public Transform main;

        public Device device;

        public int row, col;

        public BackPackMgr owner;

        public void init(Device device, int row, int col, bool rotatoed, BackPackMgr owner)
        {
            this.device = device;
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
            owner.remove_device(row, col);
        }


        
    }
}
