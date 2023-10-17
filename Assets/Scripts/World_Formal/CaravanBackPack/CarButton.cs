using Common_Formal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.Caravans;

namespace World_Formal.CaravanBackPack
{
    [RequireComponent(typeof(Button))]
    public class CarButton : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public CarBody car;
        public void init(CarBody car)
        {
            this.car = car;
            text.text = car.caravan._desc.f_name.ToString();
        }

        public void onclick()
        {
            if (Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager gmgr))
                gmgr.RemoveCaravan(car);       //把车开出车库
                        
            var cvMgr = new CaravanMgr_Formal(Common.Config.CaravanMgr_Name);
            cvMgr.init(car);     
        }

    }
}
