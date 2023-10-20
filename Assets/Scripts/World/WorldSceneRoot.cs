using Common;

namespace World
{
    public class WorldSceneRoot : SceneRoot<WorldSceneRoot>
    {
        public Creator[] creators;

        WorldContext ctx;

        //==================================================================================================

        protected override void on_init()
        {
            ctx = WorldContext._init();
            ctx.init();

            init_module();

            ctx.can_start_tick = true;
        }


        void init_module()
        {
            foreach (var e in creators)
            {
                e.@do();
            }
        }


        private void Update()
        {
            ctx.update();
        }
    }
}

