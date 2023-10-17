using Foundation;
using Spine.Unity;
using UnityEngine;


namespace Worlds.Missions.Battles.Caravan
{
    public class BattleCaravanView : MonoBehaviour, IBattleCaravanView
    {
        public SkeletonMecanim sm;
        public Animator anm;
        public BoxCollider2D box_collider;

        BattleCaravanMgr owner;
        BattleCaravan cell;

        BattleCaravan IBattleCaravanView.cell => this.cell;

        //==================================================================================================


        void IModelView<BattleCaravanMgr>.attach(BattleCaravanMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<BattleCaravanMgr>.detach(BattleCaravanMgr owner)
        {
            this.owner = null;
        }


        void IBattleCaravanView.on_modify_physics_tick()
        {
            transform.localPosition = cell.position;
            //transform.localRotation = Common.Expand.Utility.quick_rotate(cell.direction);
        }


        void IModelView<BattleCaravanMgr>.shift(BattleCaravanMgr old_owner, BattleCaravanMgr new_owner)
        {
        }


        public void init(BattleCaravan cell)
        {
            this.cell = cell;
            transform.localPosition = cell.position;
            //transform.localRotation = Common.Expand.Utility.quick_rotate(cell.direction);
        }


        void IBattleCaravanView.on_modify_init()
        {
            var dic = owner.bone_pos_dic;
            var bone_nodes = sm.transform.GetComponentsInChildren<BoneFollower>();

            dic.Clear();
            foreach (var e in bone_nodes)
            {
                dic.Add(e.boneName, e.transform.localPosition);
            }
        }
    }

}
