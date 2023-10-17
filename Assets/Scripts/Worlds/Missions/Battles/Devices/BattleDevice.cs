using CaravanEnhanced;
using Devices;
using Spine;
using UnityEngine;


namespace Worlds.Missions.Battles.Devices
{
    public class BattleDevice
    {
        public Vector2 position;
        public Vector2 direction;
        public Vector2 slot_position;
        public Vector2 slot_direction;
        public Device device;
        public Item item;

        public Bone bone;

        BattleDeviceMgr owner;

        //================================================================================================


        public void init(BattleDeviceMgr owner)
        {
            this.owner = owner;
            var caravan_mgr = owner.caravan_mgr;

            device.position = position;//临时
            device.direction = direction;
            device.velocity = caravan_mgr.caravan.velocity;
            device.faction = Device.Faction.player;
            device.item = item;

            //caravan_mgr.caravan.anm.skeleton.FindBone("");

            owner.cell_tick += tick;
        }


        public void tick()
        {
            device.position = position;
            device.velocity = owner.caravan_mgr.caravan.velocity;
            device.tick();
        }       
    }



}

