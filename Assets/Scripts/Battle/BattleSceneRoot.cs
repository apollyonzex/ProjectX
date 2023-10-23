using Common;
using World;
using World.Helpers;

namespace Battle
{
    public class BattleSceneRoot : SceneRoot<BattleSceneRoot>
    {
        public Creator[] creators;

        WorldContext ctx;
        WorldSceneRoot root;

        //==================================================================================================

        protected override void on_init()
        {
            root = WorldSceneRoot.instance;
            uiCamera = WorldSceneRoot.instance.uiCamera;
            uiRoot.worldCamera = uiCamera;

            ctx = WorldContext.instance;
            init_module();
        }


        void init_module()
        {
            foreach (var e in creators)
            {
                e.@do();
            }
        }


        public void btn_end_battle()
        {
            root.btn_end_battle();
        }
    }
}

