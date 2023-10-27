using Common;

namespace World.Helpers
{
    public class Battle_In_Out_Helper : Singleton<Battle_In_Out_Helper>
    {
        public void enter_battle(WorldContext ctx)
        {
            ctx.bctx = new();
            EX_Utility.load_scene_async("scenes", "Battle");

            ctx.is_battle = true;
        }


        public void end_battle(WorldContext ctx)
        {
            EX_Utility.unload_scene_async("Battle");

            ctx.is_battle = false;
        }
    }
}

