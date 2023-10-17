using UnityEngine;
using Spine.Unity;
using Common_Formal;
using World_Formal.Enviroment;
using World_Formal.Caravans;

namespace World_Formal.Normal_Views
{
    public class Device_Normal_Spine_View : MonoBehaviour
    {
        public SkeletonAnimation anm;

        public string idle_name;
        public string run_name;

        bool is_init = false;
        CaravanMgr_Formal caravan_mgr;
        EnviromentCaravanData m_e_caravan;

        Spine.Bone m_bone;
        Vector2 m_slot_pos;
        Vector2 m_bone_pos;
        string m_name;

        private void Update()
        {
            if (WorldSceneRoot.instance == null) return;

            if (!is_init)
            {
                if (anm != null) // 临时，因为不是所有装备都有动画
                {
                    var clip = anm.state.Data.SkeletonData.FindAnimation(idle_name);
                    if (clip == null) return;
                    anm.state.SetAnimation(0, clip, true);
                }

                Mission.instance.try_get_mgr("caravan", out var imgr1);
                caravan_mgr = (imgr1 as CaravanMgr_Formal);
                var slot = transform.GetComponentInParent<EnviromentSlotView>();
                m_name = slot.data.bone_name;
                m_slot_pos = slot.transform.position;
                //caravan_mgr.bone_pos_dic.TryGetValue(m_name, out m_bone_pos);

                Mission.instance.try_get_mgr("enviroment", out var imgr2);
                m_e_caravan = (imgr2 as EnviromentMgr).enviroment_caravan;

                is_init = true;
                return;
            }

            //caravan_mgr.bones_dic.TryGetValue(m_name, out m_bone);

            Vector3 pos = m_e_caravan.position - m_e_caravan.init_offset + new Vector2(m_bone.WorldX, m_bone.WorldY) + m_slot_pos - m_bone_pos;
            transform.position = pos;

            var e = transform.localPosition;
            e.z = 0;
            transform.localPosition = e;


            if (anm != null)
            {
                if (m_e_caravan.is_run)
                {
                    if (run_name == anm.AnimationName) return;
                    var clip = anm.state.Data.SkeletonData.FindAnimation(run_name);
                    if (clip == null) return;
                    anm.state.SetAnimation(0, clip, true);
                }
                else
                {
                    if (idle_name == anm.AnimationName) return;
                    var clip = anm.state.Data.SkeletonData.FindAnimation(idle_name);
                    if (clip == null) return;
                    anm.state.SetAnimation(0, clip, true);
                }
            }
                
        }
    }
}

