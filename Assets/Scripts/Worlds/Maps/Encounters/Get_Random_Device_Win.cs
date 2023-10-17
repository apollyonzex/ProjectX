using System;
using UnityEngine;


namespace Worlds.Missions.Dialogs
{
    public class Get_Random_Device_Win : MonoBehaviour
    {
        public Transform sv_ct_layout;
        public DeviceView item_model;

        Get_Random_Device_Mgr mgr = new();

        public event Action callback;

        //==================================================================================================


        void Start()
        {
            var items = CaravanEnhanced.CaravanFunc.TryGetItemData();
            foreach (var item in items.records)
            {
                var e = Instantiate(item_model, sv_ct_layout);

                e.init(mgr ,"caravan", item.f_icon, item.f_name);
                mgr.all.Add(e, item.f_id);
            }
        }


        public void confirm()
        {
            var selects = mgr.selected_devices;
            var all = mgr.all;
            foreach (var e in selects)
            {
                var id = all[e];
                Worlds.WorldState.instance.mission.cargoMgr.AddItem((int)id);
            }

            callback?.Invoke();
            Destroy(gameObject);
        }
    }

}

