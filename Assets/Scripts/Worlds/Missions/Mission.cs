using CaravanEnhanced;
using Foundation;
using System.Collections;
using UnityEngine.UI;
using Worlds.Missions.Battles;


namespace Worlds.Missions
{
    public class Mission
    {
        public CargoMgrView cargoMgrView;
        public CaravanMgrView caravanMgrView;
     //   public CaravanTextureView travelView;


       // public TimeLineMgr timelineMgr;
        public Maps.SiteMgr siteMgr;
        public CargoMgr cargoMgr = new();
        public CaravanMgr caravanMgr;
        public BattleMgr battleMgr;
        public CardSpace.CardMgr cardMgr;

        WorldSceneRoot world_root;
        
        //==================================================================================================


        /// <summary>
        /// 初始化Mission
        /// </summary>
        public void init()
        {
            world_root = WorldSceneRoot.instance;
            var monoLayer = world_root.MonoLayer.transform;
            var sceneLayer = world_root.SceneLayer.transform;

            //加载地图及site
            siteMgr = new();         
            var siteMgrView = world_root.Open_UI_Prefab<Maps.SiteMgrView>("map", "newbee_0_map", monoLayer);
            siteMgr.add_view(siteMgrView);

            //加载UI
            cargoMgrView = world_root.Open_UI_Prefab<CargoMgrView>("caravan", "prefabs/CargoManagerPanel", sceneLayer);
            caravanMgrView = world_root.Open_UI_Prefab<CaravanMgrView>("caravan", "prefabs/CaravanManagerPanel", sceneLayer);

            var btn_main_menu = world_root.Open_UI_Prefab<Button>("caravan", "prefabs/MainMenuButton", sceneLayer);
            btn_main_menu.onClick.AddListener(WorldState.instance.to_main_menu);

            var btn_caravan = world_root.Open_UI_Prefab<Button>("caravan", "prefabs/CaravanButton", sceneLayer);
            btn_caravan.onClick.AddListener(() => 
            {
                cargoMgrView.gameObject.SetActive(!cargoMgrView.gameObject.activeInHierarchy);
                caravanMgrView.gameObject.SetActive(!caravanMgrView.gameObject.activeInHierarchy);
            });

/*            var btn_gonext = world_root.Open_UI_Prefab<Button>("caravan", "prefabs/GoNextButton", sceneLayer);
            btn_gonext.onClick.AddListener(() =>{
    //            travelView.site_move();
            });*/

            //加载类
            cardMgr = new();

            cargoMgr = new();
            cargoMgr.add_view(cargoMgrView);

            caravanMgr = new();
            caravanMgr.add_view(caravanMgrView);
            caravanMgr.init(1);

            battleMgr = new();

      //      timelineMgr = new();

            //加载行驶画面
        //    travelView = world_root.Open_UI_Prefab<CaravanTextureView>("temp", "travel", monoLayer);
         //   caravanMgr.add_view(travelView.caravanView);
        }


        #region 开始战斗入口
        public void start_battle(string scene_name, System.Action<bool> callback)
        {
            world_root.StartCoroutine(load_battle_scene(scene_name, callback));
        }


        IEnumerator load_battle_scene(string battle_scene, System.Action<bool> callback)
        {
            world_root.load_mission_layer("[BattleLayer]", out var layer);
            var view = world_root.Open_UI_Prefab<BattleMgrView>("ui", "normal_battle", layer.transform);

            yield return AssetBundleManager.instance.load_scene_async("scene", battle_scene, new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Additive));

            battleMgr = new();
            battleMgr.init_expand(callback);
            battleMgr.add_view(view);

            battleMgr.start_battle_expand(caravanMgr);

            WorldState.instance.subState = WorldState.SubState.normal_battle;
        }
        #endregion


        #region 结束战斗入口
        public void end_battle(UnityEngine.SceneManagement.Scene scene)
        {
            world_root.StartCoroutine(unload_battle_scene(scene));
        }


        IEnumerator unload_battle_scene(UnityEngine.SceneManagement.Scene scene)
        {
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);

            world_root.unload_mission_layer();

            battleMgr.clear_views();
            battleMgr.fini_expand();

            WorldState.instance.subState = WorldState.SubState.world;
        }
        #endregion

    }


}


