using Spine.Unity;
using UnityEngine;

namespace Devices
{
    public class DeviceConfigSpineActive : DeviceConfig
    {
        public SkeletonAnimation anm;

        [HideInInspector]
        public bool is_active = false;

        //==================================================================================================

        void Start()
        {
            anm.AnimationState.Complete += AnimationState_Complete;
        }


        private void AnimationState_Complete(Spine.TrackEntry trackEntry)
        {
            is_active = true;
        }
    }
}
