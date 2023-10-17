using Foundation;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;


namespace Worlds.Missions.Battles.Effects
{
    public interface IBattleEffectView : IModelView<BattleEffectMgr>
    {
        void on_reset_tick();

    }


    public class BattleEffectMgr : Model<BattleEffectMgr, IBattleEffectView>
    {
        public BattleEffectMgr()
        {
            BattleCaravanMgr.reset_x += on_reset;
        }

        private void on_reset()
        {
            foreach (var view in views)
            {
                view.on_reset_tick();
            }
        }

        public void add_blood_effect(Vector2 position)
        {
            BattleSceneRoot.instance.create_effect_view(this, "temp", "blood", 0.8f, position);
        }
    }

}

