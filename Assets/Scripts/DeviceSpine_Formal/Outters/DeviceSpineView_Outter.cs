using Spine.Unity;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;

namespace DeviceSpine_Formal.Outters
{
    public abstract class DeviceSpineView_Outter : MonoBehaviour
    {
        public virtual void play_by_caravan(ref int index, SkeletonAnimation anm, BattleCaravan caravan)
        { 
        }

        public virtual void play_by_device(ref int index, SkeletonAnimation anm, object[] device_prms)
        { 
        }

        public virtual void play_by_graph(ref int index, SkeletonAnimation anm)
        {
        }

        public virtual void init(SkeletonAnimation anm)
        { 
        }
    }
}

