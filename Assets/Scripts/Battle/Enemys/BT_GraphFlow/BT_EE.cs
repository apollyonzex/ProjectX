using CalcExpr;
using System;

namespace Battle.Enemys.BT_GraphFlow
{
    [Serializable]
    public class BT_EE : IExpressionExternal
    {
        Type m_module_type;
        string m_module_name;
        IExpressionExternal m_external;

        CalcExpr.ValueType IExpressionExternal.ret_type => m_external.ret_type;
        public Type ret_type;

        //==================================================================================================

        public IExpressionExternal init(Type module_type, string module_name, IExpressionExternal external)
        {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;

            IExpressionExternal iee = this;
            if (iee.ret_type == CalcExpr.ValueType.Integer)
                ret_type = typeof(int);
            if (iee.ret_type == CalcExpr.ValueType.Floating)
                ret_type = typeof(float);
            if (iee.ret_type == CalcExpr.ValueType.Boolean)
                ret_type = typeof(bool);

            return this;
        }


        bool IExpressionExternal.get_value(object obj, Type obj_type, out object value)
        {
            throw new NotImplementedException();
        }


        bool IExpressionExternal.set_external(Calculator calculator, object obj, Type obj_type, int index)
        {
            throw new NotImplementedException();
        }


        public bool try_get_value<T>(BT_Context ctx, out T value)
        {
            value = default;

            if (!ctx.cpns_dic.TryGetValue(m_module_name, out var cpn)) return false;

            if (m_external.get_value(cpn, m_module_type, out var v))
            {
                try
                {
                    value = (T)v;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

