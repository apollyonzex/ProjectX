using World_Formal.Adventure;
using World_Formal.Map.Journey;
using Camp.Select_BackPack;
using Common_Formal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using World_Formal.Caravans.Devices;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;
using World_Formal.WareHouse;
using System.Linq;

namespace Camp
{
    public class CampSceneRoot : SceneRoot<CampSceneRoot> {


        public float before_z, after_z;

        private Controls1 input;
        public GameObject systemPanel;
        public PlayableDirector pd;
        private bool first_time = true;
        private bool play_timeline = false;

        public CameraController cameraController;

        public GameObject shadow;

        GameObject m_win_select_LV;
        GameObject m_win_select_carbody;
        GameObject m_win_select_backpack;

        System.Action m_btn_back_action;

        public Button Btn_go;
        public Transform caravan_node;

        //==================================================================================================

        public void SetSystemPanel() {
            systemPanel.SetActive(!systemPanel.activeSelf);
        }

        public void toWorld() {
            EX_Utility.load_game_state(typeof(Worlds.WorldState));
        }


        protected override void on_init()
        {
            input = new Controls1();
            input.Enable();
            InitAdventure();

            if (!Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager gmgr))
            {
                var cbMgr = new GarageManager(Common.Config.GarageMgr_Name);
                cbMgr.AddCaravan(300000000);
                var cardMgr = new World_Formal.Card.CardManager(Common.Config.CardMgr_Name);
                var warehouseMgr = new DeviceWareHouseManager(Common.Config.WareHouseMgr_Name);
                warehouseMgr.init_warehouse();
            }
            else
            {
                gmgr.reset();
            }
        }


        private void InitAdventure()
        {
            if (Mission.instance.try_get_mgr("adventure", out var mgr))
            {
                if(mgr is AdventureMgr amgr)
                {
                    EX_Utility.try_load_asset<AdventureView>("adventure", "adventureView", out var asset);
                    var adventureView = Instantiate(asset,uiRoot.transform,false); 
                    amgr.add_view(adventureView);
                }
            }
            else
            {
                var amgr = new AdventureMgr("adventure");
                EX_Utility.try_load_asset<AdventureView>("adventure", "adventureView", out var asset);
                var adventureView = Instantiate(asset, uiRoot.transform, false);
                amgr.add_view(adventureView);
            }
        }

        public  void PlayTimeLine(GameObject caravan) {
            Object sourceObj = null;
            if (caravan.GetComponent<Animator>()==null) {
                caravan.AddComponent<Animator>();
            }
            foreach(var track in pd.playableAsset.outputs) {
                if(track.streamName == "CaravanAnimTrack") {
                    sourceObj = track.sourceObject;
                    break;
                }
            }
            if (sourceObj == null) {
                Debug.LogWarning("没有找到TimeLine中的CaravanAnimTrack");
                return;
            }
            pd.SetGenericBinding(sourceObj, caravan.GetComponent<Animator>());
        }


        public void Update() {
            if (input.System.Config.WasPressedThisFrame()) {
                SetSystemPanel();
            }
            ClickAscensionMachine();

            if (Mission.instance.try_get_mgr(Common.Config.DeviceMgr_Name, out DeviceMgr deviceMgr))
                deviceMgr.un_use_tick();
        }

        public void ClickAscensionMachine() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }
            var mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame) {
                var onScreenPosition = mouse.position.ReadValue();
                var p = mainCamera.ScreenToWorldPoint(new Vector3(onScreenPosition.x, onScreenPosition.y, first_time ? before_z : after_z));       //z轴

                var hits = Physics2D.RaycastAll(new Vector2(p.x,p.y), Vector2.zero, Mathf.Infinity);
                foreach (var hit in hits)
                {
			        Debug.Log($"{hit.collider.gameObject.name}");
                    if (hit.collider != null && hit.collider.gameObject.name == "Machine")
                    {
                        if (first_time)
                        {
                            pd.Play();
                            first_time = false;
                            Invoke("ActiveCameraController", 2f);
                            play_timeline = true;
                        }
                        else if(play_timeline ==  false)
                        {
                            hit.collider.transform.GetChild(1).GetComponent<Spine.Unity.SkeletonAnimation>().loop = false;
                            hit.collider.transform.GetChild(1).GetComponent<Spine.Unity.SkeletonAnimation>().AnimationName = "click";
                            hit.collider.transform.GetChild(2).GetComponent<Spine.Unity.SkeletonAnimation>().loop = false;
                            hit.collider.transform.GetChild(2).GetComponent<Spine.Unity.SkeletonAnimation>().AnimationName = "click";
                            hit.collider.transform.GetChild(4).gameObject.SetActive(true);
                            hit.collider.transform.GetChild(4).GetComponent<Spine.Unity.SkeletonAnimation>().AnimationName = "animation";                     //临时代码,敏捷开发给美术看个效果
                            Invoke("open_win_level_entrance", 2f);
                        }
                        break;
                    }
                }
            }
        }

        private void ActiveCameraController()
        {
            mainCamera.GetComponent<CameraController>().enabled = true;
            play_timeline = false;
        }


        /// <summary>
        /// 按钮：结束游戏
        /// </summary>
        public void btn_exit_game()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }


        /// <summary>
        /// 按钮：返回主菜单
        /// </summary>
        public void btn_to_main_menu()
        {
            EX_Utility.load_game_state("Init_Formal.InitState", "Init_Formal");
        }


        /// <summary>
        /// 打开窗口时调用
        /// 需注册返回键action
        /// </summary>
        void register_back_action(System.Action btn_back_action)
        {
            shadow.transform.SetSiblingIndex(uiRoot.transform.childCount - 2); //打开时永远位于倒数第二
            shadow.SetActive(true);

            m_btn_back_action = btn_back_action;
            cameraController.is_lock = true;
        }


        /// <summary>
        /// 返回按钮
        /// </summary>
        public void btn_back()
        {
            m_btn_back_action?.Invoke();
        }


        /// <summary>
        /// 前往world
        /// </summary>
        public void btn_go()
        {
            Mission.instance.try_get_mgr(Common.Config.WareHouseMgr_Name, out DeviceWareHouseManager wmgr);
            wmgr.remove_view(m_win_select_backpack.GetComponent<Win_Select_BackPack>().wview);

            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cmgr);
            cmgr.backpack.remove_view(m_win_select_backpack.GetComponent<Win_Select_BackPack>().bview);

            var ctx = CampRootContext.instance;
            World_Formal.Helpers.World_In_Out_Helper.instance.enter_world(ctx.world_id, ctx.level_id);
        }


        #region 选择关卡
        void open_win_level_entrance()
        {
            if (m_win_select_LV == null)
            {
                EX_Utility.try_load_asset("level_entrance", "Win_Level_Entrance", out Win_Level_Entrance asset);
                var win = Instantiate(asset, uiRoot.transform);
                win.init();

                m_win_select_LV = win.gameObject;
            }
            else
            {
                m_win_select_LV.gameObject.SetActive(true);
            }

            m_win_select_LV.transform.SetSiblingIndex(uiRoot.transform.childCount - 1);
            register_back_action(close_level_entrance_map);
        }


        void close_level_entrance_map()
        {
            m_win_select_LV.SetActive(false);
            cameraController.is_lock = false;
            shadow.SetActive(false);
        }
        #endregion


        #region 选车
        public void open_win_select_carbody()
        {
            if (m_win_select_carbody == null)
            {
                Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager gMgr);
                
                EX_Utility.try_load_asset("ui_Formal", "windows/Win_Select_CarBody", out Win_Select_CarBody asset);
                var win = Instantiate(asset, uiRoot.transform);
                win.init();
                gMgr.add_view(win);                                         //因为车体仓库是唯一的,所以可以只调整true/false

                m_win_select_carbody = win.gameObject;
            }
            else
            {
                m_win_select_carbody.gameObject.SetActive(true);            
            }

            m_win_select_carbody.transform.SetSiblingIndex(uiRoot.transform.childCount - 1);
            register_back_action(close_win_select_carbody);
        }


        void close_win_select_carbody()
        {
            m_win_select_carbody.SetActive(false);
            open_win_level_entrance();
        }
        #endregion


        #region 背包仓库
        public void open_win_select_backpack(CarBody car)
        {
            if (m_win_select_backpack == null)
            {
                EX_Utility.try_load_asset("ui_Formal", "windows/Win_Select_BackPack", out Win_Select_BackPack asset);
                var win = Instantiate(asset, uiRoot.transform);
                win.init(car);

                m_win_select_backpack = win.gameObject;
            }
            else
            {
                m_win_select_backpack.GetComponent<Win_Select_BackPack>().init(car);    //用新版车体初始化一遍    
                m_win_select_backpack.gameObject.SetActive(true);
            }

            m_win_select_backpack.transform.SetSiblingIndex(uiRoot.transform.childCount - 1);
            register_back_action(close_win_select_backpack);

            Btn_go.gameObject.SetActive(true);
        }


        void close_win_select_backpack()
        {
            Mission.instance.try_get_mgr(Common.Config.WareHouseMgr_Name, out DeviceWareHouseManager wmgr);
            wmgr.remove_view(m_win_select_backpack.GetComponent<Win_Select_BackPack>().wview);

            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cmgr);
            cmgr.backpack.remove_view(m_win_select_backpack.GetComponent<Win_Select_BackPack>().bview);
            
            Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager gmgr);
            gmgr.AddCaravan(cmgr);

            foreach (var cview in cmgr.views)
            {
                if (cview is not CaravanView t) continue;
                t.destroy();
            }
            Mission.instance.detach_mgr(Common.Config.CaravanMgr_Name);     //移除篷车系统

            Destroy(m_win_select_backpack);
            m_win_select_backpack = null;

            //关闭仓库外观

            //关闭车体BackPack的view

            //把车体mgr转换成carbody放入车库

            //销毁win_select_backpack || 关闭

            Btn_go.gameObject.SetActive(false);
            open_win_select_carbody();
        }
        #endregion
    }
}
