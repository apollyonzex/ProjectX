using CalcExpr;
using GraphNode;
using System;

namespace Battle.Enemys.BT_GraphFlow
{
    [Serializable]
    public class BT_Expression : Expression<BT_Expression>
    {
        public int do_calc_int<T>(T ctx) where T : BT_Context
        {
            if (constant.HasValue)
            {
                return (int)constant.Value;
            }

            var ees = externals;
            var cx = ExpressionCalculator.instance;

            var fns_ori = functions;
            ExpressionFunction[] fns_copy = null;
            if (fns_ori != null)
            {
                fns_copy = new ExpressionFunction[fns_ori.Length];
                Array.Copy(fns_ori, fns_copy, fns_ori.Length);
            }

            //获取ee的值
            if (ees != null)
            {
                var length = ees.Length;
                int[] values = new int[length];
                for (int i = 0; i < length; i++)
                {
                    var ee = ees[i] as BT_EE;

                    ee.try_get_value(ctx, out int value);
                    values[i] = value;
                }

                //加载公式
                cx.attach(code);

                //加载ee
                for (int i = 0; i < length; i++)
                {
                    var e = values[i];
                    cx.set_external(i, e);
                }
            }
            else
            {
                //加载公式
                cx.attach(code);
            }

            //加载函数
            if (functions != null)
            {
                for (int i = 0; i < functions.Length; ++i)
                {
                    functions[i].initialize(ctx.GetType());
                }
            }

            //运行计算器，并返回结果
            ExpressionFunction.fns = fns_copy;
            ExpressionFunction.obj = ctx;
            cx.run(ExpressionFunction.entry);
            ExpressionFunction.fns = null;
            ExpressionFunction.obj = null;

            cx.get_result(out int ret);

            return ret;
        }


        public float do_calc_float<T>(T ctx) where T : BT_Context
        {
            if (constant.HasValue)
            {
                return Utility.convert_float_from(constant.Value);
            }

            var ees = externals;
            var cx = ExpressionCalculator.instance;

            var fns_ori = functions;
            ExpressionFunction[] fns_copy = null;
            if (fns_ori != null)
            {
                fns_copy = new ExpressionFunction[fns_ori.Length];
                Array.Copy(fns_ori, fns_copy, fns_ori.Length);
            }

            //获取ee的值
            if (ees != null)
            {
                var length = ees.Length;
                float[] values = new float[length];
                for (int i = 0; i < length; i++)
                {
                    var ee = ees[i] as BT_EE;

                    ee.try_get_value(ctx, out float value);
                    values[i] = value;
                }

                //加载公式
                cx.attach(code);

                //加载ee
                for (int i = 0; i < length; i++)
                {
                    var e = values[i];
                    cx.set_external(i, e);
                }
            }
            else
            {
                cx.attach(code);
            }

            //加载函数
            if (functions != null)
            {
                for (int i = 0; i < functions.Length; ++i)
                {
                    functions[i].initialize(ctx.GetType());
                }
            }

            //运行计算器，并返回结果
            ExpressionFunction.fns = fns_copy;
            ExpressionFunction.obj = ctx;
            cx.run(ExpressionFunction.entry);
            ExpressionFunction.fns = null;
            ExpressionFunction.obj = null;

            cx.get_result(out float ret);

            return ret;
        }


        public bool do_calc_bool<T>(T ctx) where T : BT_Context
        {
            if (constant.HasValue)
            {
                return constant.Value != 0;
            }

            var ees = externals;
            var cx = ExpressionCalculator.instance;

            var fns_ori = functions;
            ExpressionFunction[] fns_copy = null;
            if (fns_ori != null)
            {
                fns_copy = new ExpressionFunction[fns_ori.Length];
                Array.Copy(fns_ori, fns_copy, fns_ori.Length);
            }

            //获取ee的值
            if (ees != null)
            {
                var length = ees.Length;
                (object obj, Type type)[] values = new (object obj, Type type)[length];
                
                Type ty = null;

                for (int i = 0; i < length; i++)
                {
                    var item = ees[i];
                    if (item is ExpressionExternal expr)
                    {
                        var t = ctx.GetType().GetProperty(expr.name).GetValue(ctx);
                        var ret_type = expr.ret_type;
                        
                        if (ret_type == CalcExpr.ValueType.Integer)
                            ty = typeof(int);
                        if (ret_type == CalcExpr.ValueType.Floating)
                            ty = typeof(float);
                        if (ret_type == CalcExpr.ValueType.Boolean)
                            ty = typeof(bool);

                        values[i] = (t, ty);
                        continue;
                    }

                    var ee = item as BT_EE;
                    ty = ee.ret_type;

                    var mi = ee.GetType().GetMethod("try_get_value").MakeGenericMethod(ty);
                    var prms = new object[] { ctx, null };
                    mi.Invoke(ee, prms);
                    values[i] = (prms[1], ty);
                }

                //加载公式
                cx.attach(code);

                //加载ee
                for (int i = 0; i < length; i++)
                {
                    var (obj, type) = values[i];

                    if (type == typeof(float))
                    {
                        cx.set_external(i, (float)obj);
                        continue;
                    }

                    if (type == typeof(int))
                    {
                        cx.set_external(i, (int)obj);
                        continue;
                    }

                    if (type == typeof(bool))
                    {
                        cx.set_external(i, (bool)obj);
                        continue;
                    }
                }
            }
            else
            {
                //加载公式
                cx.attach(code);
            }

            //加载函数
            if (functions != null)
            {
                for (int i = 0; i < functions.Length; ++i)
                {
                    functions[i].initialize(ctx.GetType());
                }
            }

            //运行计算器，并返回结果
            ExpressionFunction.fns = fns_copy;
            ExpressionFunction.obj = ctx;
            cx.run(ExpressionFunction.entry);
            ExpressionFunction.fns = null;
            ExpressionFunction.obj = null;

            cx.get_result(out bool ret);

            return ret;
        }
    }
}



