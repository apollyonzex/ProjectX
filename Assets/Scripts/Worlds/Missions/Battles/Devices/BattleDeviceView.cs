using Foundation;
using UnityEngine;


namespace Worlds.Missions.Battles.Devices
{
    public class BattleDeviceView : MonoBehaviour, IBattleDeviceView
    {
        BattleDeviceMgr owner;

        public BattleDevice cell => m_cell;
        BattleDevice m_cell;

        //==================================================================================================


        public void init(BattleDevice cell)
        {
            this.m_cell = cell;
            transform.localPosition = cell.position;
            transform.localRotation = Utility.quick_rotate(cell.direction);
        }


        void IModelView<BattleDeviceMgr>.attach(BattleDeviceMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<BattleDeviceMgr>.detach(BattleDeviceMgr owner)
        {
            this.owner = null;
            m_cell = null;
        }


        void IBattleDeviceView.on_modify_physics_tick()
        {
            transform.localPosition = cell.position;
            transform.localRotation = Utility.quick_rotate(cell.direction);
        }


        void IModelView<BattleDeviceMgr>.shift(BattleDeviceMgr old_owner, BattleDeviceMgr new_owner)
        {
        }
    }

}

