using Foundation;
using Foundation.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Antlr3.Runtime.Tree.TreeWizard;

namespace Common
{
    public class EX_Utility
    {
        #region Base
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
        /// 异步加载场景，不销毁现有场景
        /// </summary>
        public static AssetBundleManager.ISceneRequest load_scene_async(string bundle, string path)
        {
            var ab = AssetBundleManager.instance;
            if (ab == null) return null;

            return ab.load_scene_async(bundle, path, new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }


        /// <summary>
        /// 异步销毁指定场景
        /// </summary>
        public static UnityEngine.AsyncOperation unload_scene_async(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            return SceneManager.UnloadSceneAsync(scene);
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
        #endregion


        #region Opr
        /// <summary>
        /// 字典添加 - 覆盖
        /// </summary>
        public static void dic_cover_add<K, V>(ref Dictionary<K, V> dic, K k, V v)
        {
            if (dic.ContainsKey(k))
            {
                dic.Remove(k);
            }

            dic.Add(k, v);
        }


        /// <summary>
        /// 字典删除 - 安全型
        /// </summary>
        public static void dic_safe_remove<K, V>(ref Dictionary<K, V> dic, K k)
        {
            if (dic.ContainsKey(k))
            {
                dic.Remove(k);
            }
        }


        /// <summary>
        /// float累加，不丢精度
        /// </summary>
        public static float float_safe_add(bool need_multi_1000, params float[] nums)
        {
            float ret = 0;
            int multi = need_multi_1000 ? 1000 : 1;

            foreach (var f in nums)
            {
                ret += (f * multi);
            }

            return ret / 1000;
        }
        #endregion


        #region Convert
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
        /// 转化：弧度 -> 方向
        /// </summary>
        public static Vector2 convert_rad_to_dir(float rad)
        {
            var dir =  new Vector2(1, MathF.Tan(rad)).normalized;

            if (rad < 180 * Mathf.Deg2Rad && rad > -180 * Mathf.Deg2Rad)
                return dir;
            else
                return new Vector2(-dir.x, -dir.y);
        }
        #endregion


        #region Calc
        /// <summary>
        /// 计算：转弧度后的新位置
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
        /// float随机数
        /// </summary>
        public static float rnd_float(float min, float max)
        {
            var _min = (int)(min * 10000);
            var _max = (int)(max * 10000);
            var ret = rnd_int(_min, _max);
            return (float)ret / 10000;
        }


        /// <summary>
        /// int随机数
        /// </summary>
        public static int rnd_int(int min, int max)
        {
            return new System.Random().Next(min, max);
        }


        /// <summary>
        /// 计算：向量旋转弧度
        /// </summary>
        public static float calc_rad_from_dirs(Vector2 from, Vector2 to)
        {
            var sub = to - from;
            return Mathf.Atan2(sub.y, sub.x);
        }
        #endregion


        #region Valid
        /// <summary>
        /// 检测：对象枚举中，是否包含元素
        /// </summary>
        public static bool valid_enum_contain_element<ENUM, E>(E e)
        {
            return Enum.IsDefined(typeof(ENUM), e);
        }
        #endregion
    }
}

