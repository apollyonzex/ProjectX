using Foundation;

namespace Camp
{
    public class CampState : Game.State
    {
        public static CampState instance;

        public CampState()
        {
            instance = this;
        }

        public override void enter(Game.IState last)
        {
            Common_Formal.EX_Utility.load_scene("scene", "camp");
        }
    }
}
