using Common;
using World.Helpers;

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

            init_helpers();
            init_module();

            ctx.can_start_tick = true;
        }


        void init_helpers()
        {
            var mouse_h = Common.Helpers.Mouse_Move_Helper._init();
            mouse_h.init(uiCamera);
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


        public void btn_enter_battle()
        {
            ui_active(false);

            Battle_In_Out_Helper.instance.enter_battle(ctx);
        }


        public void btn_end_battle()
        {
            Battle_In_Out_Helper.instance.end_battle(ctx);

            ui_active(true);
        }


        void ui_active(bool bl)
        {
            uiRoot.gameObject.SetActive(bl);
            monoRoot.gameObject.SetActive(bl);
        }
    }
}

