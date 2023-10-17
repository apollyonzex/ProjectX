using Common;
using Foundation;
using UnityEngine;

namespace Common_Formal
{
    public class Main : MonoBehaviour
    {
        public Config config = null;

        //==================================================================================================

        private void Awake()
        {
            var go = new GameObject("[Game]");
            go.AddComponent<Game>();
            go.AddComponent<AssetBundleManager>();
            DontDestroyOnLoad(go);

            EX_Utility.load_game_state("Init_Formal.InitState", "Init_Formal");
        }
    }
}

