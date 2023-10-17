using Devices;
using Spine.Unity;
using UnityEngine;

namespace DeviceSpine_Formal.Outters
{
    public class FireJar_Vessel : DeviceSpineView_Outter
    {
        public DeviceConfig_GetData_Int config;

        int m_interval = -1;
        Spine.Bone m_light;

        //==================================================================================================

        public override void init(SkeletonAnimation anm)
        {
            m_light = anm.skeleton.FindBone("light");
        }


        public override void play_by_graph(ref int index, SkeletonAnimation anm)
        {
            if (m_interval < 0)
            {
                m_interval = config.init_int;
                return;
            }

            float scale = (float)(m_interval - config._int) / 100;
            m_light.ScaleX = scale;
            m_light.ScaleX = scale;
        }
    }
}

