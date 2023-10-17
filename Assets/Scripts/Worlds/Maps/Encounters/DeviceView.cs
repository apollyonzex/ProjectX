using Foundation;
using UnityEngine;
using UnityEngine.UI;


namespace Worlds.Missions.Dialogs
{
    public class DeviceView : MonoBehaviour
    {
        public Toggle toggle;
        public Image selected;
        public Image icon;
        public new Text name;

        Get_Random_Device_Mgr mgr;

        //================================================================================================


        public void init(Get_Random_Device_Mgr mgr, string bundle, string path, string name)
        {
            this.mgr = mgr;

            AssetBundleManager.instance.load_asset<Sprite>(bundle , path, out var asset);
            icon.sprite = asset;
            this.name.text = name;

            gameObject.SetActive(true);
        }


        public void select()
        {
            var bl = toggle.isOn;
            selected.gameObject.SetActive(bl);
            mgr?.change_selected_device(this, bl);
        }
    }
}

