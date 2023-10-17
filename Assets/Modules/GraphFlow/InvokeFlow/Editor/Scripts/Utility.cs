using CalcExpr;
using System.Collections.Generic;

namespace InvokeFlow.Editor {
    public static class Utility {

        public static void get_context_methods(System.Type context_type, out string[] names, out FunctionMethod[] methods) {
            var key = (context_type, typeof(void));
            if (!s_context_methods.TryGetValue(key, out var ret)) {
                var list = new List<(string name, FunctionMethod func)>();
                foreach (var (n, m) in FunctionMethod.collect_methods(context_type)) {
                    list.Add((n, m));
                }
                list.Sort((a, b) => a.name.CompareTo(b.name));
                var len = list.Count + 1;
                ret.Item1 = new string[len];
                var ms = new FunctionMethod[len];
                ret.Item2 = ms;
                ret.Item1[0] = "<None>";
                var index = 1;
                foreach (var (n, m) in list) {
                    ret.Item1[index] = n;
                    ms[index] = m;
                    ++index;
                }
                s_context_methods.Add(key, ret);
            }
            names = ret.Item1;
            methods = (FunctionMethod[])ret.Item2;
        }

        public static void get_context_methods<T>(System.Type context_type, out string[] names, out FunctionMethod<T>[] methods) {
            var key = (context_type, typeof(T));
            if (!s_context_methods.TryGetValue(key, out var ret)) {
                var list = new List<(string name, FunctionMethod<T> func)>();
                foreach (var (n, m) in FunctionMethod.collect_methods<T>(context_type)) {
                    list.Add((n, m));
                }
                list.Sort((a, b) => a.name.CompareTo(b.name));
                var len = list.Count + 1;
                ret.Item1 = new string[len];
                var ms = new FunctionMethod<T>[len];
                ret.Item2 = ms;
                ret.Item1[0] = "<None>";
                var index = 1;
                foreach (var (n, m) in list) {
                    ret.Item1[index] = n;
                    ms[index] = m;
                    ++index;
                }
                s_context_methods.Add(key, ret);
            }
            names = ret.Item1;
            methods = (FunctionMethod<T>[])ret.Item2;
        }

        private static Dictionary<(System.Type, System.Type), (string[], object)> s_context_methods = new Dictionary<(System.Type, System.Type), (string[], object)>();
    }
}