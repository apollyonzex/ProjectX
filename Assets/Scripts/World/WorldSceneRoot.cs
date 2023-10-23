using Common;
using UnityEngine;
using World.Helpers;

namespace World
{
    public class WorldSceneRoot : SceneRoot<WorldSceneRoot>
    {
        public Creator[] creators;
        public GameObject ui_none;
        public GameObject ui_battle;

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


        public void btn_enter_battle()
        {
            Battle_In_Out_Helper.instance.enter_battle();

            ui_none.SetActive(false);
            ui_battle.SetActive(true);
        }


        public void btn_end_battle()
        {
            Battle_In_Out_Helper.instance.end_battle();

            ui_battle.SetActive(false);
            ui_none.SetActive(true);
        }
    }
}

