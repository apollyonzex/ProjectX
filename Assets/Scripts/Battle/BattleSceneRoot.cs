using Battle.Enemys.BT_GraphFlow;
using Common;
using World;

namespace Battle
{
    public class BattleSceneRoot : SceneRoot<BattleSceneRoot>
    {
        public Creator[] creators;
        public Trigger[] triggers;

        WorldContext ctx;
        WorldSceneRoot root;

        //==================================================================================================

        protected override void on_init()
        {
            root = WorldSceneRoot.instance;
            uiCamera = WorldSceneRoot.instance.uiCamera;
            uiRoot.worldCamera = uiCamera;

            ctx = WorldContext.instance;
            
            init_modules();
            run_triggers(true);
        }


        void init_modules()
        {
            foreach (var e in creators)
            {
                e.@do();
            }
        }


        void run_triggers(bool is_init = false)
        {
            foreach (var e in triggers)
            {
                e.@do(is_init);
            }
        }


        public void btn_vectory()
        {
            root.btn_end_battle();
        }


        public void btn_fail()
        {
            root.btn_end_battle();
        }


        public void btn_end_turn()
        {
            run_triggers();
        }
    }
}

