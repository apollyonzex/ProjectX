
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Foundation {

    public static class PlayUsingFirstScene {

        [UnityEditor.InitializeOnLoadMethod]
        private static void init() {
            EditorApplication.playModeStateChanged += on_play_mode_state_changed;
        }

        private const string MENU_ITEM_NAME = "Foundation/PlayUsingFirstScene";

        [MenuItem(MENU_ITEM_NAME, true)]
        private static bool toggle_validate() {
            Menu.SetChecked(MENU_ITEM_NAME, EditorPrefs.GetBool(EDITOR_PREFS_KEY));
            return true;
        }

        [MenuItem("Foundation/PlayUsingFirstScene")]
        public static void toggle() {
            if (EditorPrefs.GetBool(EDITOR_PREFS_KEY)) {
                EditorPrefs.SetBool(EDITOR_PREFS_KEY, false);
            } else {
                EditorPrefs.SetBool(EDITOR_PREFS_KEY, true);
            }
        }

        private const string EDITOR_PREFS_KEY = "play_using_first_scene";

        private static void on_play_mode_state_changed(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingEditMode) {
                if (EditorPrefs.GetBool(EDITOR_PREFS_KEY)) {
                    set_first_scene_as_start_scene();
                } else {
                    EditorSceneManager.playModeStartScene = null;
                }
            }
        }

        public static void set_first_scene_as_start_scene() {
            string path = null;
            foreach (var scene in EditorBuildSettings.scenes) {
                if (scene.enabled) {
                    path = scene.path;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(path)) {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            }
        }

    }

}