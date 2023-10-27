using Foundation;

namespace World
{
    public class WorldState : Game.State
    {
        public override void enter(Game.IState last)
        {
            Common.EX_Utility.load_scene("scenes", "World");
        }
    }
}

