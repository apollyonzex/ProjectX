using CalcExpr;
using System.Collections.Generic;

namespace GraphNode.Editor {
    public static class Utility {

        public static IReadOnlyDictionary<string, ExpressionFunction> get_expression_functions(System.Type context_type) {
            if (!s_context_functions.TryGetValue(context_type, out var dict)) {
                dict = new Dictionary<string, ExpressionFunction>();
                foreach (var (name, mi, invoker) in FunctionInvoker.collect_functions_by_attribute(context_type)) {
                    if (dict.ContainsKey(name)) {
                        continue;
                    }
                    dict.Add(name, new ExpressionFunction(mi.Name, invoker));
                }
                s_context_functions.Add(context_type, dict);
            }
            return dict;
        }

        public static IReadOnlyDictionary<string, IExpressionExternal> get_expression_externals(System.Type context_type) {
            if (!s_context_externals.TryGetValue(context_type, out var dict)) {
                dict = new Dictionary<string, IExpressionExternal>();
                foreach (var (name, ee) in ExpressionExternal.collect_properties_by_attribute(context_type)) {
                    if (dict.ContainsKey(name)) {
                        continue;
                    }
                    dict.Add(name, ee);
                }
            }
            return dict;
        }
        public static void get_context_methods(System.Type context_type, out string[] names, out ActionMethod[] methods) {
            var key = (context_type, typeof(void));
            if (!s_context_methods.TryGetValue(key, out var ret)) {
                var list = new List<(string name, ActionMethod func)>();
                foreach (var (n, m) in ActionMethod.collect_methods(context_type)) {
                    list.Add((n, m));
                }
                list.Sort((a, b) => a.name.CompareTo(b.name));
                var len = list.Count + 1;
                ret.Item1 = new string[len];
                var ms = new ActionMethod[len];
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
            methods = (ActionMethod[])ret.Item2;
        }

        public static void get_context_methods<T>(System.Type context_type, out string[] names, out ActionMethod<T>[] methods) {
            var key = (context_type, typeof(T));
            if (!s_context_methods.TryGetValue(key, out var ret)) {
                var list = new List<(string name, ActionMethod<T> func)>();
                foreach (var (n, m) in ActionMethod<T>.collect_methods(context_type)) {
                    list.Add((n, m));
                }
                list.Sort((a, b) => a.name.CompareTo(b.name));
                var len = list.Count + 1;
                ret.Item1 = new string[len];
                var ms = new ActionMethod<T>[len];
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
            methods = (ActionMethod<T>[])ret.Item2;
        }

        public static void get_context_methods(System.Type context_type, System.Type ret_type, out string[] names, out ActionMethod[] methods) {
            var key = (context_type, ret_type, typeof(void));
            if (!s_context_func_methods.TryGetValue(key, out var ret)) {
                IEnumerable<(string, ActionMethod)> collection;
                if (typeof(TypeFilter).IsAssignableFrom(ret_type)) {
                    collection = ActionMethod.collect_methods(context_type, System.Activator.CreateInstance(ret_type) as TypeFilter);
                } else {
                    collection = ActionMethod.collect_methods(context_type, ret_type);
                }
                var list = new List<(string name, ActionMethod func)>();
                foreach (var (n, m) in collection) {
                    list.Add((n, m));
                }
                list.Sort((a, b) => a.name.CompareTo(b.name));
                var len = list.Count + 1;
                ret.Item1 = new string[len];
                var ms = new ActionMethod[len];
                ret.Item2 = ms;
                ret.Item1[0] = "<None>";
                var index = 1;
                foreach (var (n, m) in list) {
                    ret.Item1[index] = n;
                    ms[index] = m;
                    ++index;
                }
                s_context_func_methods.Add(key, ret);
            }
            names = ret.Item1;
            methods = (ActionMethod[])ret.Item2;
        }

        public static void get_context_methods<T>(System.Type context_type, System.Type ret_type, out string[] names, out ActionMethod<T>[] methods) {
            var key = (context_type, ret_type, typeof(T));
            if (!s_context_func_methods.TryGetValue(key, out var ret)) {
                IEnumerable<(string, ActionMethod<T>)> collection;
                if (typeof(TypeFilter).IsAssignableFrom(ret_type)) {
                    collection = ActionMethod<T>.collect_methods(context_type, System.Activator.CreateInstance(ret_type) as TypeFilter);
                } else {
                    collection = ActionMethod<T>.collect_methods(context_type, ret_type);
                }
                var list = new List<(string name, ActionMethod<T> func)>();
                foreach (var (n, m) in collection) {
                    list.Add((n, m));
                }
                list.Sort((a, b) => a.name.CompareTo(b.name));
                var len = list.Count + 1;
                ret.Item1 = new string[len];
                var ms = new ActionMethod<T>[len];
                ret.Item2 = ms;
                ret.Item1[0] = "<None>";
                var index = 1;
                foreach (var (n, m) in list) {
                    ret.Item1[index] = n;
                    ms[index] = m;
                    ++index;
                }
                s_context_func_methods.Add(key, ret);
            }
            names = ret.Item1;
            methods = (ActionMethod<T>[])ret.Item2;
        }

        private static Dictionary<System.Type, Dictionary<string, ExpressionFunction>> s_context_functions = new Dictionary<System.Type, Dictionary<string, ExpressionFunction>>();
        private static Dictionary<System.Type, Dictionary<string, IExpressionExternal>> s_context_externals = new Dictionary<System.Type, Dictionary<string, IExpressionExternal>>();
        private static Dictionary<(System.Type, System.Type), (string[], object)> s_context_methods = new Dictionary<(System.Type, System.Type), (string[], object)>();
        private static Dictionary<(System.Type, System.Type, System.Type), (string[], object)> s_context_func_methods = new Dictionary<(System.Type, System.Type, System.Type), (string[], object)>();
    }
}