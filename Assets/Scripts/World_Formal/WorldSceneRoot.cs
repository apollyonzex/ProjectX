using World_Formal.Adventure;
using World_Formal.Map;
using Common.Expand;
using Common_Formal;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.Enviroment;
using World_Formal.Map.Journey;
using World_Formal.Enviroment.Road;
using World_Formal.Caravans;
using World_Formal.Caravans.Devices;
using Common_Formal.Helpers;
using World_Formal.Helpers;
using System;
using World_Formal.Enemys;
using World_Formal.UI;
using System.Collections;
using World_Formal.BattleSystem;
using World_Formal.CaravanBackPack;

namespace World_Formal
{

    public class WorldSceneRoot : SceneRoot<WorldSceneRoot> {

        [SerializeField]
        private float default_vfov;

        public SceneLayer SceneLayer;

        public MonoLayer monoLayer;

        public Transform caravan_node;
        public Transform monster_node;
        [Header("视图中摄像机专门特写篷车的位置")]
        public Transform caravan_camera_view;

        public GameObject large_map;

        public uint world_id;

        [Header("战斗UI")]
        public GameObject battle_ui;
        public Button btn_win;
        public Button btn_defeat;

        public Slider battle_progress;
        public Transform battle_progress_monster_group;
        public Transform battle_progress_start;
        public Transform battle_progress_end;

        [Header("旅行UI")]
        public GameObject travel_ui;
        public List<Button> nexts_opinion = new();

        [HideInInspector]
        public Transform caravan_body_node; //车体节点

        [HideInInspector]
        public Camera texture_camara;

        [HideInInspector]
        public Win_BackPack_World win_backpack;

        [HideInInspector]
        public Win_Reward win_reward;

        public Transform projectiles_root;

        Win_BackPack_World m_win_backpack;

        WorldContext ctx;

        public GameObject exit_button;

        //==================================================================================================

        public void init_world() {
            AutoCode.Tables.World world = new AutoCode.Tables.World();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "world", out var asset);
            world.load_from(asset);
            world.try_get(world_id,out var t);

            foreach(var level in t.f_level_list) {

            }
        }


        public void init() {
            Mission.instance.try_get_mgr("journey", out var jmgr);
            if(jmgr is JourneyMgr journeyMgr) {
                AutoCode.Tables.Level level = new AutoCode.Tables.Level();
                AssetBundleManager.instance.load_asset<BinaryAsset>("db", "level", out var asset);
                level.load_from(asset);
                level.try_get(journeyMgr.level_id, out var t);
                var enviromentMgr = new EnviromentMgr(Common.Config.EnviromentMgr_Name);
                AssetBundleManager.instance.load_asset<EnviromentView>("map", "EnviromentMgrView_Formal", out var eview);
                var enviromentView = Instantiate(eview, monoLayer.transform, false);
                enviromentMgr.add_view(enviromentView);
                enviromentMgr.init(journeyMgr.level_id);

                AssetBundleManager.instance.load_asset<RoadMgrView>("map", "road_view", out var rv);
                var roadView = Instantiate(rv, monoLayer.transform, false);
                enviromentMgr.roadMgr.add_view(roadView);
                enviromentMgr.roadMgr.init(t.f_scene_road,enviromentMgr);

                var mapMgr = new MapMgr(Common.Config.MapMgr_Name);
                AssetBundleManager.instance.load_asset<MapView>("map", "Map_Test", out var mview);
                var mapView = Instantiate(mview, monoLayer.transform, false);
                mapMgr.init(journeyMgr.world_id, t.f_map_data.Item1, t.f_map_data.Item2,t.f_init_journey_difficulty);
                mapMgr.add_view(mapView);

                ctx.world_id = journeyMgr.world_id;
                ctx.level_id = journeyMgr.level_id;
            }

            InitAdventure();
            //To be  Fixed        InitBackPack();
        } 


        private void InitAdventure()
        {
            if (Mission.instance.try_get_mgr("adventure", out var mgr))
            {
                if (mgr is AdventureMgr amgr)
                {
                    EX_Utility.try_load_asset<AdventureView>("adventure", "adventureView", out var asset);
                    var adventureView = Instantiate(asset, uiRoot.transform, false);
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


        public void ShowNext() {
            Mission.instance.try_get_mgr(Common.Config.MapMgr_Name, out var mgr);
            if(mgr is MapMgr mapMgr) {
                var next_sites = mapMgr.next_indexs;
                for(int i = 0; i < next_sites.Count; i++) {
                    nexts_opinion[i].gameObject.SetActive(true);
                    var next_index = next_sites[i];
                    mapMgr.Sites.sites.TryGetValue(next_index, out var site);
                    AssetBundleManager.instance.load_asset<Sprite>(site.icon_large.Item1, site.icon_large.Item2, out var s);
                    if (s != null) {
                        nexts_opinion[i].transform.GetComponent<Image>().sprite = s;
                    } else {
                        Debug.Log("large_icon为空");
                    }
                    nexts_opinion[i].transform.GetChild(0).GetComponent<Text>().text = site.name;

                    nexts_opinion[i].TryGetComponent<MapButton>(out var map_button);

                    if(map_button == null)
                    {
                        Debug.LogError("按钮缺失mapButton组件");
                        return;
                    }

                    map_button.on_click = () =>
                    {
                        exit_button.SetActive(false);
                        mapMgr.unhighlight_site(site.index);
                        map_button.mouse_exit = null;

                        mapMgr.move(mapMgr.current_index, site.index);
                        foreach (var op in nexts_opinion)
                        {
                            op.gameObject.SetActive(false);   
                        }
                    };

                    map_button.mouse_enter = () =>
                    {
                        mapMgr.highlight_site(site.index);
                    };

                    map_button.mouse_exit = () =>
                    {
                        mapMgr.unhighlight_site(site.index);
                    };

                }

                if (mapMgr.exit_indexs.Contains(mapMgr.current_index))
                {
                    exit_button.SetActive(true);
                }
            }
        }


        public void SetLargeMap() {
            large_map.gameObject.SetActive(!large_map.gameObject.activeSelf);
        }


        void init_caravan(out CaravanMgr_Formal mgr)
        {
            ref var pos = ref ctx.caravan_pos;
            Road_Info_Helper.try_get_altitude(pos.x, out pos.y);

            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out mgr);
            var spine_caravan = DB.instance.spineWorldCaravan_info[mgr.cell_id];
            var spine_device = DB.instance.spineWorldDevice_info;
            var spine_helper = Spine_Load_Helper.instance;
            spine_caravan = spine_helper.@do(spine_caravan, spine_caravan.init);
            spine_device = spine_helper.do_on_dic_init(spine_device);

            mgr.clear_views();           
            var view = mgr.init_default_view(caravan_node, spine_caravan, spine_device, new Vector2(0, mgr.cell.height_offset), true);
            caravan_body_node = view.body;
        }


        protected override void on_init()
        {
            ctx = WorldContext.init();
            ctx.init_data();

            init();
            load_mgr();
            load_ui();
            init_win();

            ctx.can_start_tick = true;
        }


        void load_mgr()
        {
            var mission = Mission.instance;

            init_caravan(out var cvMgr);
            cvMgr.load_in_world_scene();

            var pmgr = new ProjectileMgr(Common.Config.ProjectileMgr_Name);
            ctx.add_tick(Common.Config.ProjectileMgr_Priority,Common.Config.ProjectileMgr_Name,pmgr.tick);

            mission.try_get_mgr(Common.Config.DeviceMgr_Name, out DeviceMgr deviceMgr);
            deviceMgr.load_in_world_scene();

            new EnemyMgr(Common.Config.EnemyMgr_Name);
            new Enemys.Projectiles.ProjectileMgr(Common.Config.ProjectileMgr_Enemy_Name);
        }


        /// <summary>
        /// 根据战斗/非战斗加载UI
        /// </summary>
        public void load_ui()
        {
            var bl = ctx.is_battle;

            travel_ui.SetActive(!bl);
            battle_ui.SetActive(bl);

            if (bl)
            {
                Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal caravanMgr);
                var cardPanel = caravanMgr.cell._desc.f_card_panel;
                AssetBundleManager.instance.load_asset<Card.CardManagerView>(cardPanel.Item1, cardPanel.Item2, out var asset);
                var cardMgrView = Instantiate(asset, battle_ui.transform, false);
                cardMgrView.transform.SetAsFirstSibling();
                Mission.instance.try_get_mgr(Common.Config.CardMgr_Name, out Card.CardManager cardMgr);
                cardMgr.add_view(cardMgrView);
                cardMgr.init_battle(cardMgrView);
            }
            else
            {
                Mission.instance.try_get_mgr(Common.Config.CardMgr_Name, out Card.CardManager cardMgr);
                cardMgr.clear_views();
                cardMgr.end_battle();
            }
        }


        /// <summary>
        /// 仅加载一次：初始化弹窗
        /// </summary>
        void init_win()
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cvMgr);
            var helper = new Change_Device_Helper(cvMgr);

            init_texture_camera();
            init_win_backpack(cvMgr, helper);
            init_win_reward(helper);
        }


        void init_texture_camera()
        {
            EX_Utility.try_load_asset("ui_Formal", "parts/textureCamera", out Camera asset);
            texture_camara = Instantiate(asset, caravan_body_node);
        }


        void init_win_backpack(CaravanMgr_Formal cvMgr, Change_Device_Helper helper)
        {
            EX_Utility.try_load_asset("ui_Formal", "windows/Win_BackPack_World", out Win_BackPack_World asset);
            win_backpack = Instantiate(asset, uiRoot.transform);
            win_backpack.init(this, cvMgr, helper);

            win_backpack._active(false);
        }


        void init_win_reward(Change_Device_Helper helper)
        {
            EX_Utility.try_load_asset("ui_Formal", "windows/Win_Reward", out Win_Reward asset);
            win_reward = Instantiate(asset, uiRoot.transform);
            win_reward.init(this, helper);

            win_reward._active(false);
        }


        void Update()
        {
            ctx.update();
        }


        //==================================================================================================
        #region 按钮
        public void btn_BackPack()
        {
            win_backpack._active(true);
        }


        public void btn_leave()
        {
            World_In_Out_Helper.instance.leave_world();
        }
        #endregion


        //==================================================================================================
        #region 外部改变UI
        public void update_battle_progress(float ratio)
        {
            battle_progress.value = ratio;
        }


        public void add_process_monster_tip(Vector2 pos, progress_monster_tip model, bool is_active, out progress_monster_tip tip)
        {
            tip = Instantiate(model, battle_progress_monster_group);
            tip.transform.localPosition = pos;
            tip.gameObject.SetActive(is_active);
        }

        #endregion


        //==================================================================================================
        /// <summary>
        /// 随机延迟，添加怪物
        /// </summary>
        public void add_enemy_per_random_time(Action action)
        {
            StartCoroutine(ie());

            IEnumerator ie()
            {
                yield return new WaitForSeconds(EX_Utility.rnd_float(0, 1.5f));
                action.Invoke();
            }
        } 
    }
}
