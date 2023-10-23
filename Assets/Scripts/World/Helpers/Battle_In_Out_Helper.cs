using Common;

namespace World.Helpers
{
    public class Battle_In_Out_Helper : Singleton<Battle_In_Out_Helper>
    {
        public void enter_battle()
        {
            Base_Utility.load_scene_async("scenes", "Battle");
        }


        public void end_battle()
        {
            Base_Utility.unload_scene_async("Battle");
        }
    }
}

