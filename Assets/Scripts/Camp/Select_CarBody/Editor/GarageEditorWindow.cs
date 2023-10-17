using Common_Formal;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using World_Formal.CaravanBackPack;

namespace Camp.Select_CarBody
{
    public class GarageEditorWindow : EditorWindow
    {

        public uint car_id;

        [MenuItem("EditorWindow/GarageWindow")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(GarageEditorWindow));
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            if (SceneManager.GetActiveScene().name != "camp")
            {
                return;
            }

            car_id =(uint) EditorGUILayout.IntField((int)car_id);

            if (GUILayout.Button("添加车体"))
            {
                Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager cbMgr);
                if(cbMgr!=null)
                    cbMgr.AddCaravan(car_id);
            }
        }
    }
}
