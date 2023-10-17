using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

namespace Foundation.Editor {
    public static class AssetBundleManagerMenu {


        [MenuItem("Foundation/AssetBundleManager/Config", priority = 100)]
        static void menu_item_config() {
            Selection.activeObject = AssetBundleManagerConfig.instance;
        }

        [MenuItem("Foundation/AssetBundleManager/RawRes", false, 200)]
        static void menu_item_raw_res() {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
            List<string> ds = new List<string>(defines.Length);
            foreach (var define in defines) {
                if (!define.Equals("FORCE_USE_RES", System.StringComparison.Ordinal) && !define.Equals("FORCE_USE_AB", System.StringComparison.Ordinal)) {
                    ds.Add(define);
                }
            }
            string s;
            if (ds.Count == 0) {
                s = string.Empty;
            } else {
                s = ds[0];
                for (int i = 1; i < ds.Count; ++i) {
                    s += ";" + ds[i];
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, s);
        }

        [MenuItem("Foundation/AssetBundleManager/RawRes", true, 200)]
        static bool menu_item_raw_res_validate() {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');

            foreach (var define in defines) {
                if (define.Equals("FORCE_USE_RES", System.StringComparison.Ordinal) || define.Equals("FORCE_USE_AB", System.StringComparison.Ordinal)) {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Foundation/AssetBundleManager/ForceUseAB", false, 201)]
        static void menu_item_force_use_ab() {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
            List<string> ds = new List<string>(defines.Length);
            foreach (var define in defines) {
                if (!define.Equals("FORCE_USE_RES", System.StringComparison.Ordinal) && !define.Equals("FORCE_USE_AB", System.StringComparison.Ordinal)) {
                    ds.Add(define);
                }
            }
            ds.Add("FORCE_USE_AB");
            string s;
            s = ds[0];
            for (int i = 1; i < ds.Count; ++i) {
                s += ";" + ds[i];
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, s);
        }

        [MenuItem("Foundation/AssetBundleManager/ForceUseAB", true, 201)]
        static bool menu_item_force_use_ab_validate() {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');

            foreach (var define in defines) {
                if (define.Equals("FORCE_USE_AB", System.StringComparison.Ordinal)) {
                    return false;
                }
            }
            return true;
        }

        [MenuItem("Foundation/AssetBundleManager/ForceUseRes", false, 202)]
        static void menu_item_force_use_res() {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
            List<string> ds = new List<string>(defines.Length);
            foreach (var define in defines) {
                if (!define.Equals("FORCE_USE_RES", System.StringComparison.Ordinal) && !define.Equals("FORCE_USE_AB", System.StringComparison.Ordinal)) {
                    ds.Add(define);
                }
            }
            ds.Add("FORCE_USE_RES");
            string s;
            s = ds[0];
            for (int i = 1; i < ds.Count; ++i) {
                s += ";" + ds[i];
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, s);
        }

        [MenuItem("Foundation/AssetBundleManager/ForceUseRes", true, 202)]
        static bool menu_item_force_use_res_validate() {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');

            foreach (var define in defines) {
                if (define.Equals("FORCE_USE_RES", System.StringComparison.Ordinal)) {
                    return false;
                }
            }
            return true;
        }

        [MenuItem("Foundation/AssetBundleManager/Build AssetBundles")]
        static void menu_item_build_asset_bundles() {
            var config = AssetBundleManagerConfig.instance;
            build_asset_bundles(config.asset_bundle_path, config.build_options, EditorUserBuildSettings.activeBuildTarget);
        }

        static void build_asset_bundles(string output_path, BuildAssetBundleOptions options, BuildTarget build_target) {
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            var source = "Assets/Resources/RawResources";
            var target = "Assets/RawResources";
            FileUtil.MoveFileOrDirectory(source, target);
            FileUtil.MoveFileOrDirectory(source + ".meta", target + ".meta");
            AssetDatabase.Refresh();
            try {
                foreach (var name in Directory.EnumerateDirectories(target)) {
                    builds.Add(create_ab_build(target, Path.GetFileName(name)));
                }
                BuildPipeline.BuildAssetBundles(output_path, builds.ToArray(), options, build_target);
            }
            finally {
                FileUtil.MoveFileOrDirectory(target, source);
                FileUtil.MoveFileOrDirectory(target + ".meta", source + ".meta");
                AssetDatabase.Refresh();
            }
        }

        static AssetBundleBuild create_ab_build(string parent, string name) {

            Stack<string> folders = new Stack<string>();

            folders.Push("");

            List<string> assets = new List<string>();
            List<string> asset_addrs = new List<string>();

            string root = Path.Combine(parent, name);

            while (folders.Count != 0) {
                var folder = folders.Pop();
                var folder_path = Path.Combine(root, folder);
                foreach (var file_path in Directory.EnumerateFiles(folder_path)) {
                    var asset_path = file_path;
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(asset_path);
                    if (asset != null) {
                        if (asset is AssetRef r) {
                            if (r.asset == null) {
                                Debug.LogError($"Null AssetRef \'{asset_path}\'", asset);
                                continue;
                            }
                        }
                        assets.Add(asset_path);
                        asset_addrs.Add(Path.Combine(folder, Path.GetFileNameWithoutExtension(file_path)).Replace('\\', '/'));
                    }
                }

                foreach (var dir_path in Directory.EnumerateDirectories(folder_path)) {
                    folders.Push(Path.Combine(folder, Path.GetFileName(dir_path)));
                }
            }

            var build = new AssetBundleBuild();
            build.assetBundleName = name;
            build.assetNames = assets.ToArray();
            build.addressableNames = asset_addrs.ToArray();
            return build;
        }
    }
}


