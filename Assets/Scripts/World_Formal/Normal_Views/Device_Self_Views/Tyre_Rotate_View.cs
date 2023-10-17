using Common_Formal;
using Spine.Unity;
using UnityEngine;
using World_Formal.Caravans;
using Worlds.CardSpace;

namespace World_Formal.Normal_Views.Device_Self_Views
{
    public class Tyre_Rotate_View : MonoBehaviour
    {
        public Transform tyre;
        public float rotationSpeed = 100;

        bool is_init = false;

        public SkeletonAnimation anim;
        public float play_speed = 0.2f;

        //================================================================================================

        void Update()
        {
            if (WorldSceneRoot.instance == null) return;

            if (!is_init)
            {
                is_init = true;
                return;
            }

            var ctx = WorldContext.instance;
            var v = ctx.caravan_velocity.x;
            if (ctx.caravan_move_status != Enum.EN_caravan_move_status.idle)
            {
                // 每帧旋转指定度数
                tyre.transform.Rotate(0, 0, -v * rotationSpeed * Time.deltaTime);
                var e = v * play_speed;
                anim.timeScale = e < 0.1f ? 0.1f : e;
            }
        }
    }
}

