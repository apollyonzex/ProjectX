using Foundation;


namespace Worlds
{
    public class WorldState : Game.State
    {
        public static WorldState instance;
        public Missions.Mission mission;

        public SubState subState = SubState.world;
        public enum SubState
        {
            world,
            normal_battle,
        }

        //==================================================================================================


        public WorldState()
        {
            instance = this;
        }


        public override void enter(Game.IState last)
        {
            var adm = AssetBundleManager.instance;
            if (adm == null) return;

            adm.load_scene("scene", "World", default);
        }


        public void enter_new_mission()
        {
            mission ??= new();
            mission.init();
        }


        void exit_current_mission()
        {
            mission = null;
        }


        /// <summary>
        /// 跳转到主菜单
        /// </summary>
        public void to_main_menu()
        {
            exit_current_mission();
            Common.Expand.Utility.load_new_state<Inits.InitState>();
        }
    }
}



