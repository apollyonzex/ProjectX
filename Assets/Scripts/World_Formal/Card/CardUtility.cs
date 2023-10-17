using Foundation.Tables;

namespace World_Formal.Card
{
    public static class CardUtility
    {
        public static ExprTreeConverter converter = new ExprTreeConverter("World_Formal.Card.", ", World_Formal", "World_Formal.Card.", ", World_Formal");
    }

    public interface IFunc
    {
        bool exec();
    }

    public interface IJudge
    {
        bool result();
    }

    public interface IData
    {
        float getData();
    }

    #region 运算符
    public class Not : IJudge
    {
        IJudge j;

        public Not(IJudge j)
        {
            this.j = j;
        }
        bool IJudge.result()
        {
            return !j.result();
        }
    }

    public class Or : IJudge
    {
        IJudge j1;
        IJudge j2;

        public Or(IJudge j1, IJudge j2)
        {
            this.j1 = j1;
            this.j2 = j2;
        }

        bool IJudge.result()
        {
            return j1.result() || j2.result();
        }
    }

    public class And : IFunc, IJudge
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

        IJudge j1;
        IJudge j2;

        public And(IJudge j1, IJudge j2)
        {
            this.j1 = j1;
            this.j2 = j2;
        }

        bool IJudge.result()
        {
            return j1.result() && j2.result();
        }
    }

    public class Greater : IJudge
    {
        IData d1;
        IData d2;
        bool hasData = false;
        float num;
        public Greater(IData d, ExprTreeInt _int)
        {
            d1 = d;
            num = _int.as_int;
        }

        public Greater(IData d, ExprTreeFloat _float)
        {
            d1 = d;
            num = _float.as_float;
        }

        public Greater(IData d1, IData d2)
        {
            this.d1 = d1;
            this.d2 = d2;
            hasData = true;
        }

        bool IJudge.result()
        {
            if (hasData)
            {
                return d1.getData() > d2.getData();
            }
            return d1.getData() > num;
        }
    }

    public class Eq : IJudge
    {
        IData d1;
        IData d2;
        bool hasData = false;
        float num;

        public Eq(IData d, ExprTreeInt _int)
        {
            d1 = d;
            num = _int.as_int;
        }

        public Eq(IData d, ExprTreeFloat _float)
        {
            d1 = d;
            num = _float.as_float;
        }

        public Eq(IData d1, IData d2)
        {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result()
        {
            if (hasData)
            {
                return d1.getData() == d2.getData();
            }
            return d1.getData() == num;
        }
    }

    public class GreaterEq : IJudge
    {
        IData d1;
        IData d2;
        bool hasData = false;
        float num;

        public GreaterEq(IData d, ExprTreeInt _int)
        {
            d1 = d;
            num = _int.as_int;
        }

        public GreaterEq(IData d, ExprTreeFloat _float)
        {
            d1 = d;
            num = _float.as_float;
        }

        public GreaterEq(IData d1, IData d2)
        {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result()
        {
            if (hasData)
            {
                return d1.getData() >= d2.getData();
            }
            return d1.getData() >= num;
        }
    }

    public class Less : IJudge
    {
        IData d1;
        IData d2;
        bool hasData = false;
        float num;

        public Less(IData d, ExprTreeInt _int)
        {
            d1 = d;
            num = _int.as_int;
        }

        public Less(IData d, ExprTreeFloat _float)
        {
            d1 = d;
            num = _float.as_float;
        }

        public Less(IData d1, IData d2)
        {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result()
        {
            if (hasData)
            {
                return d1.getData() < d2.getData();
            }
            return d1.getData() < num;
        }
    }
    #endregion

    public class DebugFunc  : IFunc
    {
        public string str;

        public DebugFunc(string str)
        {
            this.str = str;
        }

        bool IFunc.exec()
        {
            UnityEngine.Debug.Log(str);
            return true;
        }
    }
}
