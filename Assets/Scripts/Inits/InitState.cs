using Foundation;


namespace Inits
{
    public class InitState : Game.State
    {
        public override void enter(Game.IState last)
        {
            var adm = AssetBundleManager.instance;
            if (adm == null) return;

            adm.load_scene("scene", "init", default);
        }
    }

}