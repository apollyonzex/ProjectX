using Foundation;
using Foundation.Tables;
using System;
using System.Linq;
using System.Reflection;

namespace Common
{
    public class Base_Utility
    {
        /// <summary>
        /// 加载游戏state（状态机）
        /// </summary>
        public static void load_game_state(string class_name, string assembly_name)
        {
            var type = Assembly.Load(assembly_name).GetType($"{class_name}");
            var obj = Activator.CreateInstance(type);

            if (obj is Game.State e)
            {
                var loading = new LoadingState();
                loading.add_job(null, () =>
                {
                    Game.instance.state.next = e;
                }, 0);

                Game.instance.state.next = loading;
            }
        }


        /// <summary>
        /// 加载场景
        /// </summary>
        public static void load_scene(string bundle, string path)
        {
            var ab = AssetBundleManager.instance;
            if (ab == null) return;

            ab.load_scene(bundle, path, default);
        }


        /// <summary>
        /// 加载asset
        /// </summary>
        public static bool try_load_asset<T>((string bundle, string path) p, out T asset) where T : UnityEngine.Object
        {
            var err = AssetBundleManager.instance.load_asset(p.bundle, p.path, out asset);
            if (ErrorExt.is_ok(err))
            {
                return true;
            }
            else
            {
                UnityEngine.Debug.LogWarning("路径错误：asset文件");
                return false;
            }
        }


        /// <summary>
        /// 不启动游戏，加载asset
        /// path带文件后缀
        /// </summary>
        public static bool try_load_asset_without_running<T>(string path, bool is_complete_path, out T asset) where T : UnityEngine.Object
        {
            if (!is_complete_path)
                path = $"Assets/Resources/RawResources/{path}";

            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                UnityEngine.Debug.LogWarning("路径错误：asset文件");
                return false;
            }

            return true;
        }


        /// <summary>
        /// 加载table
        /// </summary>
        public static bool try_load_table<T>(string binaryAsset_name, out T t) where T : ITable, new()
        {
            t = new();

            var err = AssetBundleManager.instance.load_asset<BinaryAsset>("db", binaryAsset_name, out var e);
            if (ErrorExt.is_ok(err))
            {
                t.load_from(e);
                return true;
            }
            else
            {
                UnityEngine.Debug.LogWarning("路径错误：BinaryAsset");
                return false;
            }
        }


        /// <summary>
        /// 不启动游戏，加载table
        /// </summary>
        public static bool try_load_table_without_running<T>(string excel_name, string sheet_name, out T t) where T : ITable, new()
        {
            t = new();

            var xlsx = UnityEditor.AssetDatabase.LoadAssetAtPath<ExcelFileAsset>($"Assets/Tables/{excel_name}.xlsx");
            if (xlsx == null)
            {
                return false;
            }

            bool e;
            foreach (var asset in xlsx.assets.Where(t => t.name == sheet_name))
            {
                e = t.load_from(asset.bytes);
                if (!e) return false;
            }

            return true;
        }
    }
}

