using Battle.HandCards.Funcs;
using Foundation.Tables;

namespace Battle.HandCards.Funcs
{
    public class Utility
    {
        public static ExprTreeConverter converter = new("Battle.HandCards.Funcs.", ", Battle", "Battle.HandCards.Funcs.", ", Battle");

        //==================================================================================================

        public static bool expr_convert<T>(IExprTree value, out T t, out string err_msg) where T : class, IFunc 
        {
            t = null;
            if (!converter.convert(value, out var obj, out err_msg)) return false;

            t = obj as T;
            return true;
        }
    }


    public class And : IFunc
    {
        IFunc f1;
        IFunc f2;

        public And(IFunc f1, IFunc f2)
        {
            this.f1 = f1;
            this.f2 = f2;
        }

        bool IFunc.@do()
        {
            return f1.@do() && f2.@do();
        }
    }
}

