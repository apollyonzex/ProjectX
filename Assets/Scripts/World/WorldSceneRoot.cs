using Common;

namespace World
{
    public class WorldSceneRoot : SceneRoot<WorldSceneRoot>
    {
        public Rooms.RoomCreator room_creator;

        WorldContext ctx;

        //==================================================================================================

        protected override void on_init()
        {
            ctx = WorldContext._init();
            ctx.init();

            init_mgr();
            init_module();

            ctx.can_start_tick = true;
        }


        void init_mgr()
        {
            var time_mgr = new Times.TimeMgr(Config.TimeMgr_Name);
            time_mgr.load_view(uiRoot.transform);
        }


        void init_module()
        {
            room_creator.@do();
        }


        private void Update()
        {
            ctx.update();
        }
    }
}

