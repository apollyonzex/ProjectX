using Common_Formal;
using Common_Formal.Helpers;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;
using World_Formal.WareHouse;

namespace Camp.Select_BackPack
{
    public class Win_Select_BackPack : MonoBehaviour
    {
        public BackPackView bview;
        public DeviceWareHouseView wview;

        public Button backButton;
        public Button nextButton;

        public Transform content;

        CampSceneRoot root;

        //==================================================================================================

        public void init(CarBody car)
        {
            root = CampSceneRoot.instance;

            var spine_caravan = DB.instance.spineCampCaravan_info[car.caravan._desc.f_id];
            var spine_device = DB.instance.spineCampDevice_info;
            var spine_helper = Spine_Load_Helper.instance;
            spine_caravan = spine_helper.@do(spine_caravan, spine_caravan.init);
            spine_device = spine_helper.do_on_dic_init(spine_device);

            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cvMgr);
            cvMgr.init_default_view(root.caravan_node, spine_caravan, spine_device, root.caravan_node.transform.localPosition);
            var helper = new Change_Device_Helper(cvMgr);

            //按钮
            backButton.onClick.AddListener(root.btn_back);
            nextButton.onClick.AddListener(root.btn_go);

            //背包
            World_Formal.Helpers.BackPack_Helper.instance.init(bview, content, cvMgr, helper);

            //卡牌
            Mission.instance.try_get_mgr(Common.Config.CardMgr_Name, out World_Formal.Card.CardManager cardMgr);
            cardMgr.init_cards(cvMgr);                      //初始化卡牌系统的数据 not新建

            //仓库
            Mission.instance.try_get_mgr(Common.Config.WareHouseMgr_Name, out DeviceWareHouseManager warehouseMgr);
            warehouseMgr.add_view(wview);
            warehouseMgr.init_helper(helper);            //生成仓库系统的外观 和 辅助类

        }
    }

}

