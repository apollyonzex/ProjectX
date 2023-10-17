using UnityEngine;
using World_Formal;

namespace Editor.Windows
{
    public class Utility
    {
        public static bool valid_is_world()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return false;
            }

            if (WorldSceneRoot.instance == null)
            {
                GUILayout.Label("请先进入world");
                return false;
            }

            return true;
        }
    }
}

