
using System.Reflection;
using System.Collections.Generic;

namespace CalcExpr {

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ExprFuncAttribute : System.Attribute {
        public string name { get; }
        public ExprFuncAttribute() { }
        public ExprFuncAttribute(string name) { this.name = name; }
    }
    
    [System.Serializable]
    public class FunctionInvoker {
        
        public static FunctionInvoker create(MethodInfo method) {
            if (!Utility.get_value_type(method.ReturnType, out var ret)) {
                return null;
            }
            var ps = method.GetParameters();
            var args = new ValueType[ps.Length];
            for (int i = 0; i < ps.Length; ++i) {
                if (!Utility.get_value_type(ps[i].ParameterType, out args[i])) {
                    return null;
                }
            }
            return new FunctionInvoker {
                m_ret = ret,
                m_args = args,
            };
        }

        public ValueType ret_type => m_ret;

        public bool check(ValueType[] args) {
            if (args.Length != m_args.Length) {
                return false;
            }
            unsafe {
                fixed (ValueType* a = args, b = m_args) {
                    for (ValueType * _a = a, _b = b, _end = a + args.Length; _a < _end; ++_a, ++_b) {
                        if (*_a != *_b) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool get_method(System.Type type, string name, out MethodInfo method_info) {
            var pts = new System.Type[m_args.Length];
            for (int i = 0; i < m_args.Length; ++i) {
                pts[i] = Utility.get_type(m_args[i]);
            }
            method_info = type.GetMethod(name, pts);
            if (method_info == null) {
                return false;
            }
            if (method_info.ReturnType != Utility.get_type(m_ret)) {
                method_info = null;
                return false;
            }
            return true;
        }

        public static IEnumerable<(string, MethodInfo, FunctionInvoker)> collect_functions_by_attribute(System.Type type) {
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<ExprFuncAttribute>();
                if (attr == null) {
                    continue;
                }
                var name = attr.name ?? mi.Name;
               
                var invoker = create(mi);
                if (invoker != null) {
                    yield return (name, mi, invoker);
                }
            }
        }

        public bool call(object obj, MethodInfo mi, uint[] args, out uint ret) {
            if (m_args.Length != args.Length) {
                ret = 0;
                return false;
            }
            var ps = new object[args.Length];
            for (int i = 0; i < args.Length; ++i) {
                ps[i] = Utility.convert_from(m_args[i], args[i]);
            }
            ret = Utility.convert_to(m_ret, mi.Invoke(obj, ps));
            return true;
        }

        private FunctionInvoker() {

        }

        private ValueType m_ret;
        private ValueType[] m_args;
    }
}