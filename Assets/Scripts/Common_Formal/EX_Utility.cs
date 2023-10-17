using Foundation;
using Foundation.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Common_Formal
{
    public class EX_Utility
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
        public static void load_scene(string bundle ,string path)
        {
            var ab = AssetBundleManager.instance;
            if (ab == null) return;

            ab.load_scene(bundle, path, default);
        }


        /// <summary>
        /// 异步加载场景，不销毁现有场景
        /// </summary>
        public static AssetBundleManager.ISceneRequest load_scene_async(string path)
        {
            var ab = AssetBundleManager.instance;
            if (ab == null) return null;

            return ab.load_scene_async("scene_Formal", path, new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }


        /// <summary>
        /// 异步销毁指定场景
        /// </summary>
        public static UnityEngine.AsyncOperation unload_scene_async(UnityEngine.SceneManagement.Scene scene)
        {
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
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


        /// <summary>
        /// 快捷创建节点
        /// </summary>
        public static void create_cell_in_scene<T, V, Concrete_V>(T mgr, (string,string) view_path, Transform parent, out Concrete_V view) where T : class, IModel<T, V> where V : class, IModelView<T> where Concrete_V : UnityEngine.Object
        {
            try_load_asset(view_path.Item1, view_path.Item2, out Concrete_V prefab);
            view = UnityEngine.Object.Instantiate(prefab, parent);
            mgr.add_view(view as V);
        }


        /// <summary>
        /// 字典添加操作 - 覆盖
        /// </summary>
        public static void dic_cover_add<K, V>(ref Dictionary<K, V> dic, K k, V v, Action<V> action = null) 
        {
            if (dic.TryGetValue(k, out var value))
            {
                dic.Remove(k);
                action?.Invoke(value);
            } 

            dic.Add(k, v);
        }


        /// <summary>
        /// 快捷切换game_state
        /// </summary>
        public static void load_game_state(Type t)
        {
            var type = t;
            var obj = Activator.CreateInstance(type);

            if (obj is Game.State e)
            {
                var loading = new LoadingState();
                loading.add_job(null, () => {
                    Game.instance.state.next = e;
                }, 0);
                Game.instance.state.next = loading;
            }
        }


        /// <summary>
        /// 快捷计算float累加
        /// </summary>
        public static float add_and_divide_1000(params float[] nums)
        {
            float result = 0;
            foreach (var num in nums)
            {
                result += num;
            }
            return result / 1000;
        }


        /// <summary>
        /// LookRotation增强
        /// </summary>
        public static Quaternion quick_look_rotation_from_left(Vector2 dir)
        {
            return Quaternion.LookRotation(Vector3.forward, new Vector2(-dir.y, dir.x));
        }


        public static Quaternion quick_look_rotation_from_up(Vector2 dir)
        {
            return quick_look_rotation_from_left(new Vector2(-dir.y, dir.x));
        }


        /// <summary>
        /// 水平翻转
        /// </summary>
        public static Quaternion quick_flipX(Quaternion q)
        {
            return Quaternion.Euler(q.eulerAngles.x, -180f, -q.eulerAngles.z);
        }


        /// <summary>
        /// 计算转弧度后的新位置
        /// </summary>
        public static void calc_pos_from_rotate(ref Vector2 pos, float rad)
        {
            var mg = pos.magnitude;

            float zero_rad = MathF.Acos(Vector2.Dot(pos.normalized, Vector2.right));
            if (pos.y < 0)
                zero_rad *= -1;
            rad += zero_rad;
            pos = new(MathF.Cos(rad) * mg, MathF.Sin(rad) * mg);
        }


        /// <summary>
        /// v2 - v3增强
        /// </summary>
        public static Vector3 v2_to_v3(Vector2 v2, float z)
        {
            return new Vector3(v2.x, v2.y, z);
        }

        public static void v2_cover_v3(Vector2 v2, ref Vector3 v3)
        {
            v3 = v2_to_v3(v2, v3.z);
        }

        public static Vector3 v2_cover_v3(Vector2 v2, Vector3 v3)
        {
            return v2_to_v3(v2, v3.z);
        }


        /// <summary>
        /// 检测：对象枚举中，是否包含元素
        /// </summary>
        public static bool valid_enum_contain_element<T, V>(V v)
        {
            return System.Enum.IsDefined(typeof(T), v);
        }


        /// <summary>
        /// 根据uint获取文本,获取失败则返回uint本身
        /// </summary>
        public static string get_uint_localization(uint num)          
        {
            EX_Utility.try_load_table("uint_lz", out AutoCode.Tables.UintLz record);
            record.try_get(num, out var r);
            if (r != null)
            {
                switch (PlayerPrefs.GetString("Language"))
                {
                    case "CN":
                        return r.f_zh;
                    case "EN":
                        return r.f_en;
                }
            }

            return num.ToString();
        }

        public static string get_string_localization(string str)
        {
            EX_Utility.try_load_table("string_lz", out AutoCode.Tables.StringLz record);
            record.try_get(str, out var r);
            if (r != null)
            {
                switch (PlayerPrefs.GetString("Language"))
                {
                    case "CN":
                        return r.f_zh;
                    case "EN":
                        return r.f_en;
                }
            }

            return str;
        }


        /// <summary>
        /// float随机数
        /// </summary>
        public static float rnd_float(float min, float max)
        {
            var _min = (int)(min * 10000);
            var _max = (int)(max * 10000);
            var r = new System.Random().Next(_min, _max);
            return (float)r / 10000;
        }
    }
}

