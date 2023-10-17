using Common_Formal;
using Common_Formal.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;

namespace World_Formal.WareHouse
{
    public class WorldBackPack : MonoBehaviour
    {
        public BackPackView bview;

        public Transform slotsinfo_content;

        public void init(Change_Device_Helper helper)
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cvMgr);
            var spine_caravan = DB.instance.spineWorldCaravan_info[cvMgr.cell_id];
            var spine_device = DB.instance.spineWorldDevice_info;
            var spine_helper = Spine_Load_Helper.instance;
            spine_caravan = spine_helper.@do(spine_caravan,spine_caravan.init);
            spine_device = spine_helper.do_on_dic_init(spine_device);

            cvMgr.init_default_view(WorldSceneRoot.instance.caravan_camera_view,spine_caravan,spine_device,WorldSceneRoot.instance.transform.localPosition);
            
            
            //这里TBD        不能init_default_view          要另一种
            EX_Utility.try_load_asset("ui_Formal", "datas/Device_Dir_Icon_Asset", out Device_Dir_Icon_Asset e0);
            Dictionary<int, Sprite> icons = new();
            foreach (var e1 in e0.datas)
            {
                icons.Add(e1.dir, e1.icon);
            }
            EX_Utility.try_load_asset("ui_Formal", "windows/Slot_Info", out Slot_Info_View e2);
            foreach (var (slot, _ ) in cvMgr.slot_device_dic)
            {
                var view = Instantiate(e2, slotsinfo_content,false);
                cvMgr.add_view(view);
                view.init(helper, slot, icons[(int)slot.type]);
                helper.views.Add(slot, view);
            }
            
            cvMgr.backpack.add_view(bview);
            cvMgr.backpack.init_helper(helper);
        }


        public void destroy()
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cvMgr);
            cvMgr.backpack.remove_view(bview);
            Destroy(gameObject);
        }
    }
}
