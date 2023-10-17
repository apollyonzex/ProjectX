using Spine.Unity;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using static Worlds.Missions.Battles.Caravan.BattleCaravan_Enum;

namespace DeviceSpine_Formal.Outters
{
    public class FireJar_Landing : DeviceSpineView_Outter
    {
        public string on_anm_name;
        public string off_anm_name = "idle";

        [Header("罐子飞起时，高度倍率 - 越大越高")]
        public float jar_fly_height_factor = 2;

        [Header("罐子飞起时，播放速度倍率 - 越大播放越快")]
        public float jar_fly_timescale_factor = 5;

        string anm_name = "";
        bool is_lock = false;

        //==================================================================================================
        public override void play_by_caravan(ref int index, SkeletonAnimation anm, BattleCaravan caravan)
        {
            if (is_lock) return;

            index = 0;
            if (caravan.main_state == Main_State.Land)
            {
                anm_name = on_anm_name;
                var land_v_y = -caravan.land_velocity.y;
                anm.skeleton.FindTransformConstraint("stone_scale").MixScaleX = jar_fly_height_factor * land_v_y;
                anm.timeScale = jar_fly_timescale_factor / land_v_y;
            }
            else
            {
                anm_name = off_anm_name;
                anm.timeScale = 1;
            }        

            if (anm.AnimationName == anm_name) return;
            var clip = anm.state.Data.SkeletonData.FindAnimation(anm_name);
            if (clip == null) return;
            
            anm.state.SetAnimation(index++, clip, false);
            if (anm_name == on_anm_name)
            {
                anm.AnimationState.End += E_End;
                is_lock = true;
            }
        }


        void E_End(Spine.TrackEntry trackEntry)
        {
            is_lock = false;
        }
    }
}

