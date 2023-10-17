using CalcExpr;
using System;

namespace World_Formal.BT_GraphFlow.Expression_Externals
{
    [Serializable]
    public class BT_Float_EE : IBT_EE
    {
        Type m_module_type;
        string m_module_name;
        IExpressionExternal m_external;

        CalcExpr.ValueType IExpressionExternal.ret_type => m_external.ret_type;

        //================================================================================================

        IExpressionExternal IBT_EE.init(Type module_type, string module_name, IExpressionExternal external)
        {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;

            return this;
        }


        bool IExpressionExternal.get_value(object obj, Type obj_type, out object value)
        {
            value = null;
            return false;
        }


        bool IExpressionExternal.set_external(Calculator calculator, object obj, Type obj_type, int index)
        {
            return false;
        }


        bool IBT_EE.get_value<T>(BT_Context ctx, out T value)
        {
            value = default;

            if (ctx.try_get_cpn(m_module_name, out var cpn))
            {
                if (m_external.get_value(cpn, m_module_type, out var v))
                {
                    value = (T)v;
                    return true;
                }
            }

            return false;
        }
    }
}

