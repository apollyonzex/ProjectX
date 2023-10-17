using System;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


namespace World_Formal.Card
{
    public class TrackCardAreaView : CardAreaView
    {
        public SkeletonGraphic track;

        public bool toright = true;

        public Transform leftp, rightp;

        public float speed;


        public override void tick()
        {
            move_slots();
        }

        private  void move_slots()
        {
            foreach(var slot in slots)
            {
                if(!slot.has_card)  //没有卡就一致放置在最侧边
                {
                    if (toright)
                        slot.transform.position = leftp.position;
                    else
                        slot.transform.position = rightp.position;

                    continue;
                }

                if (toright)        //右移
                {               
                    slot.transform.localPosition += Vector3.right * speed * Common.Config.PHYSICS_TICK_DELTA_TIME;
                    if(slot.transform.position.x >= rightp.position.x)
                    {
                        slot.cardView.drop();
                        slot.transform.position = leftp.position;
                    }
                }
                else            //左移
                {
                    slot.transform.localPosition += Vector3.left * speed * Common.Config.PHYSICS_TICK_DELTA_TIME;
                    if (slot.transform.position.x <= leftp.position.x)
                    {
                        slot.cardView.drop();
                        slot.transform.position = rightp.position;
                    }
                }
                if(Time.deltaTime <= Common.Config.PHYSICS_TICK_DELTA_TIME)
                {
                    track.timeScale = 1;
                }
                else
                {
                    track.timeScale = 1 * Common.Config.PHYSICS_TICK_DELTA_TIME / Time.deltaTime;
                }
            }
        }
    }
}
