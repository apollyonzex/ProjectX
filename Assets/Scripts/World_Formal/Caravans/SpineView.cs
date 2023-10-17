using Common_Formal;
using Common_Formal.DS;
using Spine.Unity;
using UnityEngine;

namespace World_Formal.Caravans
{
    public class SpineView : MonoBehaviour
    {
        public SkeletonAnimation anim;
        SpineDS m_anim_info;

        bool is_new_anim = false;
        bool is_lock = false;

        //==================================================================================================

        private void Update()
        {
            if (anim == null) return;
            if (!is_new_anim) return;
            if (is_lock) return;

            if (anim.AnimationName != m_anim_info.name)
            {
                var clip = anim.state.Data.SkeletonData.FindAnimation(m_anim_info.name);
                if (clip != null)
                {
                    anim.state.SetAnimation(0, clip, m_anim_info.loop);
                    trigger_anim_opr();
                }
            }

            is_new_anim = false;
        }


        public void set_anim(SpineDS ds)
        {
            m_anim_info = ds;
            is_new_anim = true;
        }


        /// <summary>
        /// 触发类动画，特殊处理
        /// </summary>
        void trigger_anim_opr()
        {
            if (!EX_Utility.valid_enum_contain_element<Enum.EN_caravan_anim_trigger_status, string>(m_anim_info.name))
                return;

            is_lock = true;
            anim.state.Complete += State_Complete;
        }


        void State_Complete(Spine.TrackEntry trackEntry)
        {
            is_lock = false;
        }
    }
}

