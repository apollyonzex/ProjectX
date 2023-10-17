using Assets.Scripts.World_Formal;
using World_Formal.Enviroment;
using Common_Formal;
using Spine.Unity;
using UnityEngine;

namespace World_Formal.Normal_Views
{
    public class Caravan_Body_Normal_Spine_View : MonoBehaviour
    {
        public SkeletonAnimation anm;
        public string idle_name;
        public string run_name;

        bool is_init = false;

        EnviromentCaravanData m_e_caravan;

        private void Update()
        {
            if (WorldSceneRoot.instance == null) return;

            if (!is_init)
            {

                var clip = anm.state.Data.SkeletonData.FindAnimation(idle_name);
                if (clip == null) return;
                anm.state.SetAnimation(0, clip, true);

                Mission.instance.try_get_mgr("enviroment", out var imgr);
                m_e_caravan = (imgr as EnviromentMgr).enviroment_caravan;

                is_init = true;
                return;
            }

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

