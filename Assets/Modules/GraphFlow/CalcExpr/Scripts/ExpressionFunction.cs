
using System.Collections.Generic;

namespace CalcExpr {

    [System.Serializable]
    public class ExpressionFunction {

        public ExpressionFunction(string method_name, FunctionInvoker invoker) {
            m_method_name = method_name;
            m_invoker = invoker;
        }

        public FunctionInvoker invoker => m_invoker;
        public string name => m_method_name;

        public void initialize(System.Type type) {
            if (m_initialized == type) {
                return;
            }
            m_initialized = type;
            if (type != null) {
                m_invoker.get_method(type, m_method_name ?? string.Empty, out m_method_info);
            }
        }

        public static IIndexFunction entry => s_mgr;
        public static object obj {
            get => s_mgr.obj;
            set => s_mgr.obj = value;
        }

        public static ExpressionFunction[] fns {
            get => s_mgr.fns;
            set => s_mgr.fns = value;
        }

        private string m_method_name;
        private FunctionInvoker m_invoker;

        [System.NonSerialized]
        private System.Type m_initialized = null;
        [System.NonSerialized]
        private System.Reflection.MethodInfo m_method_info;


        private class Manager : IIndexFunction {
            public object obj;
            public ExpressionFunction[] fns;
            bool IIndexFunction.call(int index, uint[] argv, out uint ret) {
                try {
                    var fn = fns[index];
                    if (fn.m_method_info == null) {
                        ret = 0;
                        return false;
                    }
                    return fn.m_invoker.call(obj, fn.m_method_info, argv, out ret);
                } catch (System.Exception) {
                    ret = 0;
                    return false;
                }
            }
        }

        private static Manager s_mgr = new Manager();
    }
}