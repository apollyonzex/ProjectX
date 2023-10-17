using UnityEngine;

namespace Inits
{
    public class InitSceneRoot : Common.SceneRoot<InitSceneRoot>
    {
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void start()
        {
            Common.Expand.Utility.load_new_state<Worlds.WorldState>();
        }


        /// <summary>
        /// 结束游戏
        /// </summary>
        public void exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }


        private void LateUpdate()
        {
            synchronize_camera();
        }


        /// <summary>
        /// 中英文切换
        /// </summary>
        public void change_language()
        {
            var e = PlayerPrefs.GetString("Language");
            if (e == "CN")
                e = "EN";
            else
                e = "CN";
            PlayerPrefs.SetString("Language", e);

            LanguageMgr.instance.load();
        }


        private void Start()
        {
            
        }

    }

}