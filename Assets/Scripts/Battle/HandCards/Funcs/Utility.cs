using Battle.HandCards.Funcs;
using Foundation.Tables;

namespace Battle.HandCards.Funcs
{
    public class Utility
    {
        public static ExprTreeConverter converter = new("Battle.HandCards.Funcs.", ", Battle", "Battle.HandCards.Funcs.", ", Battle");

        //==================================================================================================

        public static bool expr_convert(IExprTree value, out object obj, out string err_msg)
        {
            return converter.convert(value, out obj, out err_msg);
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

        bool IFunc.exec()
        {
            return f1.exec() && f2.exec();
        }
    }
}

