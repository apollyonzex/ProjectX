using Foundation;
using UnityEngine;

namespace World_Formal.Map.Encounters
{
    public class EncounterView : MonoBehaviour, IEncounterView
    {
        EncounterMgr owner;

        //==================================================================================================

        void IModelView<EncounterMgr>.attach(EncounterMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<EncounterMgr>.detach(EncounterMgr owner)
        {
            this.owner = null;
        }


        void IModelView<EncounterMgr>.shift(EncounterMgr old_owner, EncounterMgr new_owner)
        {
        }
    }
}

