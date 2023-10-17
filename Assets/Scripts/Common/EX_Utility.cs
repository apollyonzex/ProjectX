using Foundation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class EX_Utility
    {
        public class Opr
        {
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


            /// <summary>
            /// 快捷创建view
            /// </summary>
            public static void quick_add_view<Mgr, IView, View>((string, string) p, Mgr mgr, Transform parent, out View view) where Mgr : class, IModel<Mgr, IView> where View : UnityEngine.Object, IView where IView : class, IModelView<Mgr>
            {
                Base_Utility.try_load_asset(p, out view);
                view = UnityEngine.Object.Instantiate(view, parent);
                mgr.add_view(view);
            }
        }


        public class Convert
        {
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
        }


        public class Calc
        {
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
        }


        public class Valid
        {
            /// <summary>
            /// 检测：对象枚举中，是否包含元素
            /// </summary>
            public static bool valid_enum_contain_element<T, V>(V v)
            {
                return Enum.IsDefined(typeof(T), v);
            }
        }
    }
}

