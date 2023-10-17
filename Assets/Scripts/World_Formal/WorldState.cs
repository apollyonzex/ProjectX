using Common_Formal;
using Foundation;

namespace World_Formal
{
    public class WorldState : Game.State
    {

        //==================================================================================================

        public override void enter(Game.IState last)
        {
            EX_Utility.load_scene("scene_Formal", "world_formal");
        }
    }
}

