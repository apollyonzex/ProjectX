using Common_Formal;
using UnityEngine;

namespace Init_Formal
{
    public class InitSceneRoot : SceneRoot<InitSceneRoot>
    {
        #region btn    
        public void start_new_game()
        {
            Mission.init();
            EX_Utility.load_game_state("Camp.CampState", "Camp");
        }


        public void exit_game()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        #endregion


        void LateUpdate()
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

            Common_Formal.Localization.LanguageMgr.instance.load();
        }
    } 
}

