using Spine.Unity;
using UnityEngine;

namespace CaravanEnhanced {
    [ExecuteInEditMode]
    public class CaravanMakerSlot :MonoBehaviour {
        [HideInInspector]
        public int index;   //插槽号码,不是初始道具号码,用作插槽之间的区分

        public uint id;

        public AutoCode.Tables.Device.e_slotType slotType;

        //[HideInInspector]
        public CaravanBody owner;

        public BoneFollower boneFollower;



        public void OnDestroy() {
            owner.RemoveSlot(this);
        }
    }
}
