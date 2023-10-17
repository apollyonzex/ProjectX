using Foundation;
using TMPro;
using UnityEngine;

namespace Camp.Level_Entrances
{
    public class Level_EntranceView : MonoBehaviour, ILevel_EntranceView
    {
        public new TextMeshProUGUI name;

        Level_EntranceMgr owner;

        //==================================================================================================

        void IModelView<Level_EntranceMgr>.attach(Level_EntranceMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<Level_EntranceMgr>.detach(Level_EntranceMgr owner)
        {
            this.owner = null;
        }


        void IModelView<Level_EntranceMgr>.shift(Level_EntranceMgr old_owner, Level_EntranceMgr new_owner)
        {
        }
    }
}

