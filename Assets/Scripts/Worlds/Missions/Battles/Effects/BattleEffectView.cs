using Common;
using Foundation;
using UnityEngine;


namespace Worlds.Missions.Battles.Effects
{
    public class BattleEffectView : MonoBehaviour, IBattleEffectView
    {
        BattleEffectMgr owner;

        //==================================================================================================


        private void OnDestroy()
        {
            owner.remove_view(this);
        }


        void IModelView<BattleEffectMgr>.attach(BattleEffectMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<BattleEffectMgr>.detach(BattleEffectMgr owner)
        {
            this.owner = null;
        }


        void IBattleEffectView.on_reset_tick()
        {
            var pos = transform.localPosition;
            pos.x -= Config.current.reset_pos_intervel;
            transform.localPosition = pos;
        }


        void IModelView<BattleEffectMgr>.shift(BattleEffectMgr old_owner, BattleEffectMgr new_owner)
        {
        }


        internal void init(Vector2 position)
        {
            transform.localPosition = position;
        }
    }

}

