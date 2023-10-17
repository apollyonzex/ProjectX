using Common_Formal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;
using World_Formal.WareHouse;

namespace World_Formal.Helpers
{
    public class BackPack_Helper : Singleton<BackPack_Helper>
    {
        public void init(BackPackView bview, Transform content, CaravanMgr_Formal cvMgr, Change_Device_Helper helper)
        {
            EX_Utility.try_load_asset("ui_Formal", "datas/Device_Dir_Icon_Asset", out Device_Dir_Icon_Asset e0);
            Dictionary<int, Sprite> icons = new();
            foreach (var e1 in e0.datas)
            {
                icons.Add(e1.dir, e1.icon);
            }

            EX_Utility.try_load_asset("ui_Formal", "windows/Slot_Info", out Slot_Info_View e2);
            foreach (var (slot, _) in cvMgr.slot_device_dic)
            {
                var view = Object.Instantiate(e2, content);
                cvMgr.add_view(view);
                view.init(helper, slot, icons[(int)slot.type]);

                helper.views.Add(slot, view);
            }

            cvMgr.backpack.add_view(bview);
            cvMgr.backpack.init_helper(helper);
        }
    }
}

