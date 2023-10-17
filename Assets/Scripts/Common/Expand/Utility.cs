using Foundation;
using Foundation.Tables;
using System.Linq;
using UnityEngine;


namespace Common.Expand
{
    public class Utility
    {
        /// <summary>
        /// 根据权重，随机返回list下标，用于分支选择
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int get_random_index_from_weight(params int[] weights)
        {
            int i = new System.Random().Next(0, weights.Sum());
            int max = 0;

            for (int n = 0; n < weights.Length; n++)
            {
                max += weights[n];
                if (i < max)
                {
                    i = n;
                    break;
                }
            }
            return i;
        }


        public static void convert_vector(Vector3 v3, out Vector2 v2)
        {
            v2 = new Vector2(v3.x, v3.y);
        }


        public static void convert_vector(Vector2 v2, out Vector3 v3)
        {
            v3 = new Vector3(v2.x, v2.y, 0);
        }


        public static void load_new_state<T>() where T : Game.State, new()
        {
            var loading = new LoadingState();
            loading.add_job(null, () =>
            {
                Game.instance.state.next = new T();
            }, 0);

            Game.instance.state.next = loading;
        }


        /// <summary>
        /// 加载asset
        /// </summary>
        public static bool try_load_asset<T>(string bundle, string path, out T asset) where T : UnityEngine.Object
        {
            asset = null;

            var err = AssetBundleManager.instance.load_asset<T>(bundle, path, out var e);
            if (ErrorExt.is_ok(err))
            {
                asset = e;
                return true;
            }
            else
            {
                Debug.LogWarning("路径错误：asset文件");
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
                Debug.LogWarning("路径错误：asset文件");
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
                Debug.LogWarning("路径错误：表格BinaryAsset");
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


        /// <summary>
        /// 获取射线在scene中的点击位置
        /// 以组件所在的gameobject为参照物
        /// </summary>
        public static bool try_get_point(Ray ray, Component target, out Vector3 point)
        {
            point = new();
            if (target == null) return false;

            var transform = target.transform;
            var plane = new Plane(transform.forward, -Vector3.Dot(transform.position, transform.forward));

            if (!plane.Raycast(ray, out var distance)) return false;

            point = ray.GetPoint(distance);
            point = transform.InverseTransformPoint(point); //获取点的位置
            return true;
        }
    }
}

