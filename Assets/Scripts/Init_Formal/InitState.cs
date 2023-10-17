using Foundation;

namespace Init_Formal
{
    public class InitState : Game.State
    {
        public override void enter(Game.IState last)
        {
            Common_Formal.EX_Utility.load_scene("scene_Formal", "init");
        }
    }
}

