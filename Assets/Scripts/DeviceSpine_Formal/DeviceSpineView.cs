using DeviceViews;
using Spine.Unity;
using System.Linq;
using UnityEngine;
using Worlds.Missions.Battles;
using Worlds.Missions.Battles.Caravan;

namespace DeviceSpine_Formal
{
    public class DeviceSpineView : MonoBehaviour
    {
        public SkeletonAnimation anm;

        Outters.DeviceSpineView_Outter[] m_outters;
        BattleCaravan m_caravan;

        //==================================================================================================

        private void Start()
        {
            m_outters = transform.GetComponents<Outters.DeviceSpineView_Outter>();
            foreach (var outter in m_outters)
            {
                outter.init(anm);
            }
        }


        private void Update()
        {
            if (m_caravan == null)
            {
                var root = BattleSceneRoot.instance;
                if (root == null) return;
                else
                    m_caravan = root.battleMgr.caravan_mgr.caravan;
            }

            int i = 0;
            foreach (var outter in m_outters)
            {
                foreach (var view in GetComponents<ProviderView>().Where(t => t.component_return_prms != null))
                {
                    outter.play_by_device(ref i, anm, view.component_return_prms);
                }

                outter.play_by_caravan(ref i, anm, m_caravan);
                outter.play_by_graph(ref i, anm);
            }
        }
    }
}

