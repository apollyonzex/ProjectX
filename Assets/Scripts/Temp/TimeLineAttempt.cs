using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;


namespace Assets.Scripts.Temp {
    public class TimeLineAttempt : MonoBehaviour {
        public PlayableDirector timeline;
        public GameObject  test;

        private Dictionary<string, PlayableBinding> bindings = new Dictionary<string, PlayableBinding>();

        public void Start() {
            play();
            add_track();
        }
        public void play() {
            foreach(var t in timeline.playableAsset.outputs){
                if (!bindings.ContainsKey(t.streamName)) {
                    bindings.Add(t.streamName, t);
                }
            }

        }

        public void add_track() {
            timeline.SetGenericBinding(bindings["CameraAnim"].sourceObject,test.GetComponent<Animator>());
        }
    }
}
