using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace CaravanEnhanced {

    public class TimeLineEntity : MonoBehaviour
    {
        public PlayableDirector pd;
        private Dictionary<string, PlayableBinding> bindingDic = new Dictionary<string, PlayableBinding>();

        public void init() {
            foreach (var t in pd.playableAsset.outputs) {
                if (!bindingDic.ContainsKey(t.streamName)) {
                    bindingDic.Add(t.streamName, t);
                }
            }
        }

        public bool set_track(string str, Animator anim) {
            if (bindingDic.TryGetValue(str, out var value) == false) {
                return false;
            }
            pd.SetGenericBinding(bindingDic[str].sourceObject, anim);
            return true;
        }

        public void enviromentmove(bool t) {
       //     Worlds.WorldState.instance.mission.travelView.enviroment_move(t);
        }

        public void destroytimeline() {
      //      Worlds.WorldState.instance.mission.timelineMgr.destroytimeline();
        }
    }

}