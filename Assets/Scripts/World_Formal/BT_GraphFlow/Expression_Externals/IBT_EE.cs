using CalcExpr;
using World_Formal.BT_GraphFlow;

namespace World_Formal.BT_GraphFlow.Expression_Externals
{
    public interface IBT_EE : IExpressionExternal
    {
        public IExpressionExternal init(System.Type module_type, string module_name, IExpressionExternal external);

        public bool get_value<T>(BT_Context ctx, out T value);    
    }
}

