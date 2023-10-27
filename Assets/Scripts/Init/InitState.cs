using Foundation;

namespace Init
{
    public class InitState : Game.State
    {
        public override void enter(Game.IState last)
        {
            Common.EX_Utility.load_scene("scenes", "Init");
        }
    }
}

