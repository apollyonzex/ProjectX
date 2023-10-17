using UnityEngine;
using Foundation;


namespace Common
{
    class Main : MonoBehaviour
    {
        public Config config = null;

        //==================================================================================================


        private void Awake()
        {
            var go = new GameObject("[Game]");
            go.AddComponent<Game>();
            go.AddComponent<AssetBundleManager>();
            DontDestroyOnLoad(go);

            Expand.Utility.load_new_state<Inits.InitState>();
        }
    }

}