using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using static Worlds.Missions.Battles.Caravan.BattleCaravan_Enum;

namespace DeviceSpine_Formal.Outters
{
    public class WheelStone_Move : DeviceSpineView_Outter
    {
        [Tooltip("播放速度 - 车速的x倍")]
        public float play_speed = 1f;

        Dictionary<Main_State, Spine_Info> m_anm_names_dic = new()
        {
            { Main_State.Idle, new Spine_Info("idle", true)},
            { Main_State.Run, new Spine_Info("run", true)},
            { Main_State.Brake, new Spine_Info("brake", false)},
            { Main_State.Jump, new Spine_Info("jump", false)},
            { Main_State.Land, new Spine_Info("land", false)},
            { Main_State.Spurt, new Spine_Info("run", true)},
            { Main_State.Jumping, new Spine_Info("jumping", true)},
        };

        //==================================================================================================

        public override void play_by_caravan(ref int index, SkeletonAnimation anm, BattleCaravan caravan)
        {
            var e = caravan.velocity.x * play_speed;
            anm.timeScale = e < 1 ? 1 : e;

            if (caravan.main_state == Main_State.Brake) // 刹车特殊处理
            {
                anm.state.Data.SetMix("run", "idle", 2 * anm.timeScale);
                anm.timeScale = 1;
            }                      

            m_anm_names_dic.TryGetValue(caravan.main_state, out var info);

            var name = info.name;
            if (anm.AnimationName == name) return;
            var clip = anm.state.Data.SkeletonData.FindAnimation(name);

            index = 0;
            anm.state.SetAnimation(index++, clip, info.loop);
        }
    }
}

