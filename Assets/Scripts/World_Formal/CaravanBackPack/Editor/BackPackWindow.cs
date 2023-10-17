using UnityEngine;
using UnityEditor;
using Common_Formal;
using UnityEngine.SceneManagement;
using World_Formal.Caravans.Devices;
using World_Formal.CaravanBackPack;

namespace Camp.CaravanBackPack
{
    public class BackPackWindow : EditorWindow {

        public int item_id;

        [MenuItem("EditorWindow/BackPackWindow")]
        public static void ShowWindow() {
            var window = EditorWindow.GetWindow(typeof(BackPackWindow));
        }

        private void OnGUI() {
            if (!Application.isPlaying) {
                GUILayout.Label("请先运行游戏");
                return;
            }
            if (SceneManager.GetActiveScene().name != "camp") {
                return;
            }
            GUILayout.Label("BackPack");
            
            if (Mission.instance == null)
                return;
            Mission.instance.try_get_mgr("backpack", out var mgr);
            if (mgr == null)
                return;
            item_id = EditorGUILayout.IntField(item_id);
            if (GUILayout.Button("添加物体")) {
                if(mgr is BackPackMgr bpmgr) {
                    Device device = new((uint)item_id);
                    bpmgr.put_device(device);
                }
            }
            
            var areas = (mgr as BackPackMgr).data.areas;
            for(int i = 0; i < areas.Count; i++) {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"area{i}");

                if (GUILayout.Button("unlock")) {
                    (mgr as BackPackMgr).unlock_area(i);
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
