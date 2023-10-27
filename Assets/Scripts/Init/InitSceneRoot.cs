using Common;

namespace Init
{
    public class InitSceneRoot : SceneRoot<InitSceneRoot>
    {
        #region btn    
        public void btn_start()
        {
            Mission._init();
            EX_Utility.load_game_state("World.WorldState", "World");
        }


        public void btn_exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        #endregion
    }
}

