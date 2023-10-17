
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using GraphNode;

namespace InvokeFlow {

    public enum FunctionParameterType {
        Content = 0,
        Integer = 1,
        Floating = 2,
        Boolean = 3,
        IntegerRef = 4,
        FloatingRef = 5,
        BooleanRef = 6,
    }

    [System.Serializable]
    public class FunctionMethod {
        public FunctionParameterType[] parameter_types;
        public string method_name;

        public MethodInfo method_info => m_method_info;

        [System.NonSerialized]
        protected MethodInfo m_method_info;

        [System.NonSerialized]
        protected bool m_initialized;

        public int[] input_indices;

        public int[] output_indices;

        public virtual void initialize(System.Type obj_type) {
            if (!m_initialized) {
                m_initialized = true;
                if (obj_type == null || string.IsNullOrEmpty(method_name) || parameter_types == null) {
                    return;
                }
                System.Type[] ps;
                if (parameter_types.Length == 0) {
                    ps = System.Type.EmptyTypes;
                } else {
                    ps = new System.Type[parameter_types.Length];
                    for (int i = 0; i < parameter_types.Length; ++i) {
                        switch (parameter_types[i]) {
                            case FunctionParameterType.Content:
                                ps[i] = typeof(string);
                                break;
                            case FunctionParameterType.Integer:
                                ps[i] = typeof(int);
                                break;
                            case FunctionParameterType.Floating:
                                ps[i] = typeof(float);
                                break;
                            case FunctionParameterType.Boolean:
                                ps[i] = typeof(bool);
                                break;
                            case FunctionParameterType.IntegerRef:
                                ps[i] = typeof(int).MakeByRefType();
                                break;
                            case FunctionParameterType.FloatingRef:
                                ps[i] = typeof(float).MakeByRefType();
                                break;
                            case FunctionParameterType.BooleanRef:
                                ps[i] = typeof(bool).MakeByRefType();
                                break;
                            default:
                                return;
                        }
                    }
                }
                m_method_info = obj_type.GetMethod(method_name, ps);
            }
        }

        public virtual bool invoke<E>(System.Type obj_type, object obj, E[] parameters, out object ret, out FunctionOutputs outputs) where E : ExpressionBase<E>, new() {
            initialize(obj_type);
            if (m_method_info == null || parameters.Length != input_indices.Length) {
                goto l_failed;
            }
            object[] ps;
            if (parameter_types.Length == 0) {
                ps = null;
            } else {
                ps = new object[parameter_types.Length];
                for (int i = 0; i < input_indices.Length; ++i) {
                    var idx = input_indices[i];
                    switch (parameter_types[idx]) {
                        case FunctionParameterType.Content:
                            ps[idx] = parameters[i].content;
                            break;
                        case FunctionParameterType.Integer:
                        case FunctionParameterType.IntegerRef:
                            if (parameters[i].calc(obj, obj_type, out int int_val)) {
                                ps[idx] = int_val;
                            } else {
                                goto l_failed;
                            }
                            break;

                        case FunctionParameterType.Floating:
                        case FunctionParameterType.FloatingRef:
                            if (parameters[i].calc(obj, obj_type, out float float_val)) {
                                ps[idx] = float_val;
                            } else {
                                goto l_failed;
                            }
                            break;

                        case FunctionParameterType.Boolean:
                        case FunctionParameterType.BooleanRef:
                            if (parameters[i].calc(obj, obj_type, out bool bool_val)) {
                                ps[idx] = bool_val;
                            } else {
                                goto l_failed;
                            }
                            break;
                    }
                }
            }
            ret = m_method_info.Invoke(obj, ps);
            outputs = new FunctionOutputs(this, ps, 0);
            return true;
        l_failed:
            ret = null;
            outputs = default;
            return false;
        }

        public static IEnumerable<(string, FunctionMethod)> collect_methods(System.Type type) {
            var int_type = typeof(int);
            var int_ref_type = typeof(int).MakeByRefType();
            var float_type = typeof(float);
            var float_ref_type = typeof(float).MakeByRefType();
            var bool_type = typeof(bool);
            var bool_ref_type = typeof(bool).MakeByRefType();
            var string_type = typeof(string);
            var void_type = typeof(void);
            var pts = new List<FunctionParameterType>();
            var inputs = new List<int>();
            var outputs = new List<int>();
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null || mi.ReturnType != void_type) {
                    continue;
                }
                var ps = mi.GetParameters();
                bool failed = false;
                for (int i = 0; i < ps.Length; ++i) {
                    var pi = ps[i];
                    if (pi.ParameterType == string_type) {
                        pts.Add(FunctionParameterType.Content);
                        inputs.Add(i);
                    } else if (pi.ParameterType == int_type) {
                        pts.Add(FunctionParameterType.Integer);
                        inputs.Add(i);
                    } else if (pi.ParameterType == int_ref_type) {
                        pts.Add(FunctionParameterType.IntegerRef);
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            inputs.Add(i);
                        }
                        outputs.Add(i);
                    } else if (pi.ParameterType == float_type) {
                        pts.Add(FunctionParameterType.Floating);
                        inputs.Add(i);
                    } else if (pi.ParameterType == float_ref_type) {
                        pts.Add(FunctionParameterType.FloatingRef);
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            inputs.Add(i);
                        }
                        outputs.Add(i);
                    } else if (pi.ParameterType == bool_type) {
                        pts.Add(FunctionParameterType.Boolean);
                        inputs.Add(i);
                    } else if (pi.ParameterType == bool_ref_type) {
                        pts.Add(FunctionParameterType.BooleanRef);
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            inputs.Add(i);
                        }
                        outputs.Add(i);
                    } else {
                        failed = true;
                        break;
                    }
                }
                if (!failed) {
                    yield return (attr.name ?? mi.Name, new FunctionMethod {
                        parameter_types = pts.ToArray(),
                        input_indices = inputs.ToArray(),
                        output_indices = outputs.ToArray(),
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = true,
                    });
                }
                pts.Clear();
                inputs.Clear();
                outputs.Clear();
            }
        }

        public static IEnumerable<(string, FunctionMethod<T>)> collect_methods<T>(System.Type type) {
            var int_type = typeof(int);
            var int_ref_type = typeof(int).MakeByRefType();
            var float_type = typeof(float);
            var float_ref_type = typeof(float).MakeByRefType();
            var bool_type = typeof(bool);
            var bool_ref_type = typeof(bool).MakeByRefType();
            var string_type = typeof(string);
            var void_type = typeof(void);
            var param_type = typeof(T);
            var pts = new List<FunctionParameterType>();
            var inputs = new List<int>();
            var outputs = new List<int>();
            
            foreach (var mi in type.GetMethods()) {
                var attr = mi.GetCustomAttribute<GraphActionAttribute>();
                if (attr == null || mi.ReturnType != void_type) {
                    continue;
                }
                var ps = mi.GetParameters();
                if (ps.Length == 0 || ps[0].ParameterType != param_type) {
                    continue;
                }
                bool failed = false;
                for (int i = 1; i < ps.Length; ++i) {
                    var pi = ps[i];
                    var idx = i - 1;
                    if (pi.ParameterType == string_type) {
                        pts.Add(FunctionParameterType.Content);
                        inputs.Add(idx);
                    } else if (pi.ParameterType == int_type) {
                        pts.Add(FunctionParameterType.Integer);
                        inputs.Add(idx);
                    } else if (pi.ParameterType == int_ref_type) {
                        pts.Add(FunctionParameterType.IntegerRef);
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            inputs.Add(idx);
                        }
                        outputs.Add(idx);
                    } else if (pi.ParameterType == float_type) {
                        pts.Add(FunctionParameterType.Floating);
                        inputs.Add(idx);
                    } else if (pi.ParameterType == float_ref_type) {
                        pts.Add(FunctionParameterType.FloatingRef);
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            inputs.Add(idx);
                        }
                        outputs.Add(idx);
                    } else if (pi.ParameterType == bool_type) {
                        pts.Add(FunctionParameterType.Boolean);
                        inputs.Add(idx);
                    } else if (pi.ParameterType == bool_ref_type) {
                        pts.Add(FunctionParameterType.BooleanRef);
                        if (!pi.Attributes.HasFlag(ParameterAttributes.Out)) {
                            inputs.Add(idx);
                        }
                        outputs.Add(idx);
                    } else {
                        failed = true;
                        break;
                    }
                }
                if (!failed) {
                    yield return (attr.name ?? mi.Name, new FunctionMethod<T> {
                        parameter_types = pts.ToArray(),
                        input_indices = inputs.ToArray(),
                        output_indices = outputs.ToArray(),
                        method_name = mi.Name,
                        m_method_info = mi,
                        m_initialized = true,
                    });
                }
                pts.Clear();
                inputs.Clear();
                outputs.Clear();
            }
        }
    }

    [System.Serializable]
    public class FunctionMethod<T> : FunctionMethod {
        public override void initialize(System.Type obj_type) {
            if (!m_initialized) {
                m_initialized = true;
                if (obj_type == null || string.IsNullOrEmpty(method_name) || parameter_types == null) {
                    return;
                }
                System.Type[] ps;
                if (parameter_types.Length == 0) {
                    ps = new System.Type[] { typeof(T) };
                } else {
                    ps = new System.Type[parameter_types.Length + 1];
                    ps[0] = typeof(T);
                    for (int i = 0; i < parameter_types.Length; ++i) {
                        switch (parameter_types[i]) {
                            case FunctionParameterType.Content:
                                ps[i + 1] = typeof(string);
                                break;
                            case FunctionParameterType.Integer:
                                ps[i + 1] = typeof(int);
                                break;
                            case FunctionParameterType.Floating:
                                ps[i + 1] = typeof(float);
                                break;
                            case FunctionParameterType.Boolean:
                                ps[i + 1] = typeof(bool);
                                break;
                            case FunctionParameterType.IntegerRef:
                                ps[i + 1] = typeof(int).MakeByRefType();
                                break;
                            case FunctionParameterType.FloatingRef:
                                ps[i + 1] = typeof(float).MakeByRefType();
                                break;
                            case FunctionParameterType.BooleanRef:
                                ps[i + 1] = typeof(bool).MakeByRefType();
                                break;
                            default:
                                return;
                        }
                    }
                }
                m_method_info = obj_type.GetMethod(method_name, ps);
            }
        }

        public override bool invoke<E>(System.Type obj_type, object obj, E[] parameters, out object ret, out FunctionOutputs outputs) {
            return invoke(obj_type, obj, default, parameters, out ret, out outputs);
        }

        public bool invoke<E>(System.Type obj_type, object obj, T param, E[] parameters, out object ret, out FunctionOutputs outputs) where E : ExpressionBase<E>, new() {
            initialize(obj_type);
            if (m_method_info == null || parameters.Length != input_indices.Length) {
                goto l_failed;
            }
            object[] ps = new object[parameter_types.Length + 1];
            ps[0] = param;
            for (int i = 0; i < input_indices.Length; ++i) {
                var idx = input_indices[i];
                switch (parameter_types[idx]) {
                    case FunctionParameterType.Content:
                        ps[idx + 1] = parameters[i].content;
                        break;
                    case FunctionParameterType.Integer:
                    case FunctionParameterType.IntegerRef:
                        if (parameters[i].calc(obj, obj_type, out int int_val)) {
                            ps[idx + 1] = int_val;
                        } else {
                            goto l_failed;
                        }
                        break;

                    case FunctionParameterType.Floating:
                    case FunctionParameterType.FloatingRef:
                        if (parameters[i].calc(obj, obj_type, out float float_val)) {
                            ps[idx + 1] = float_val;
                        } else {
                            goto l_failed;
                        }
                        break;

                    case FunctionParameterType.Boolean:
                    case FunctionParameterType.BooleanRef:
                        if (parameters[i].calc(obj, obj_type, out bool bool_val)) {
                            ps[idx + 1] = bool_val;
                        } else {
                            goto l_failed;
                        }
                        break;
                }
            }
            
            ret = m_method_info.Invoke(obj, ps);
            outputs = new FunctionOutputs(this, ps, 1);
            return true;
        l_failed:
            ret = null;
            outputs = default;
            return false;
        }
    }

    public struct FunctionOutputs : IReadOnlyList<uint> {
        FunctionMethod method;
        object[] parameters;
        int offset;

        public FunctionOutputs(FunctionMethod method, object[] parameters, int offset) {
            this.method = method;
            this.parameters = parameters;
            this.offset = offset;
        }

        public int Count => method.output_indices.Length;
        public uint this[int index] {
            get {
                int idx = method.output_indices[index];
                switch (method.parameter_types[idx]) {
                    case FunctionParameterType.IntegerRef:
                        return (uint)(int)parameters[idx + offset];

                    case FunctionParameterType.FloatingRef:
                        return CalcExpr.Utility.convert_to((float)parameters[idx + offset]);

                    case FunctionParameterType.BooleanRef:
                        return (bool)parameters[idx + offset] ? 1u : 0;

                    default:
                        return 0;
                }
            }
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<uint> IEnumerable<uint>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<uint> {
            FunctionOutputs outputs;
            int index;

            public Enumerator(FunctionOutputs outputs) {
                this.outputs = outputs;
                index = -1;
            }

            public bool MoveNext() {
                return ++index < outputs.Count;
            }

            public void Reset() {
                index = -1;
            }

            public uint Current => outputs[index];

            object IEnumerator.Current => Current;

            void System.IDisposable.Dispose() {

            }
        }
    }


    [System.Serializable]
    public class Function {

        public Expression[] parameters;
        public FunctionMethod method;
        public string[] out_names;

        public Function clone() {
            var ret = new Function {
                method = method,
                parameters = (Expression[])parameters.Clone(),
                out_names = (string[])out_names.Clone(),
            };
            foreach (ref var e in Foundation.ArraySlice.create(ret.parameters)) {
                e = e.clone() as Expression;
            }
            return ret;
        }

        public bool invoke(System.Type obj_type, object obj, out object ret, out FunctionOutputs outputs) {
            if (method == null) {
                ret = null;
                outputs = default;
                return false;
            }
            return method.invoke(obj_type, obj, parameters, out ret, out outputs);
        }
    }

    [System.Serializable]
    public class Function<T> {
        public Expression[] parameters;
        public FunctionMethod<T> method;
        public string[] out_names;

        public Function<T> clone() {
            var ret = new Function<T> {
                method = method,
                parameters = (Expression[])parameters.Clone(),
                out_names = (string[])out_names.Clone(),
            };
            foreach (ref var e in Foundation.ArraySlice.create(ret.parameters)) {
                e = e.clone() as Expression;
            }
            return ret;
        }

        public bool invoke(System.Type obj_type, object obj, T param, out object ret, out FunctionOutputs outputs) {
            if (method == null) {
                ret = null;
                outputs = default;
                return false;
            }
            return method.invoke(obj_type, obj, param, parameters, out ret, out outputs);
        }
    }
}