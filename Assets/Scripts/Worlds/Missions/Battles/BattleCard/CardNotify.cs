using UnityEngine.UI;
using UnityEngine;

namespace Worlds.Missions.Battles {
    public class CardNotify :MonoBehaviour {

        public Text notice;

        public void notify(string str) {
            notice.text = str;
        }

    }
}
