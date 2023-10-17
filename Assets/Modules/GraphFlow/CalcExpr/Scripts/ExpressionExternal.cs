

using System.Collections.Generic;
using System.Reflection;

namespace CalcExpr {

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ExprConstAttribute : System.Attribute {
        public string name { get; }
        public ExprConstAttribute() { }
        public ExprConstAttribute(string name) { this.name = name; }
    }

    public interface IExpressionExternal {
        bool get_value(object obj, System.Type obj_type, out object value);
        bool set_external(Calculator calculator, object obj, System.Type obj_type, int index);
        ValueType ret_type { get; }
    }

    [System.Serializable]
    public class ExpressionExternal : IExpressionExternal {
        public static ExpressionExternal create(System.Type type, string name) {
            var property = type.GetProperty(name);
            if (property == null) {
                return null;
            }
            if (!Utility.get_value_type(property.PropertyType, out var ret)) {
                return null;
            }
            return new ExpressionExternal {
                m_ret = ret,
                m_name = name,
                m_property = property,
                m_initialized = type,
            };
        }

        void initialize(System.Type obj_type) {
            if (m_initialized != obj_type) {
                m_initialized = obj_type;
                if (obj_type != null) {
                    var ret_type = Utility.get_type(m_ret);
                    var pi = obj_type.GetProperty(m_name);
                    if (pi != null && pi.PropertyType == ret_type) {
                        m_property = pi;
                    } else if (!obj_type.IsInterface && !get_interface_property_info(obj_type, ret_type)) {
                        m_property = null;
                    }
                }
            }
        }

        bool get_interface_property_info(System.Type ty, System.Type ret) {
            foreach (var t in ty.GetInterfaces()) {
                var pi = t.GetProperty(m_name);
                if (pi != null && pi.PropertyType == ret) {
                    m_property = pi;
                    return true;
                }
                if (get_interface_property_info(t, ret)) {
                    return true;
                }
            }
            return false;
        }

        public string name => m_name;

        public bool get_value(object obj, System.Type obj_type, out object value) {
            initialize(obj_type);
            if (m_property == null) {
                value = null;
                return false;
            } 
            value = m_property.GetValue(obj);
            return true;
        }

        public bool set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (!get_value(obj, obj_type, out var value)) {
                return false;
            }
            switch (m_ret) {
                case ValueType.Integer:
                    calculator.set_external(index, (int)value);
                    break;
                case ValueType.Floating:
                    calculator.set_external(index, (float)value);
                    break;
                case ValueType.Boolean:
                    calculator.set_external(index, (bool)value);
                    break;
            }
            return true;
        }

        public static IEnumerable<(string, ExpressionExternal)> collect_properties_by_attribute(System.Type type) {
            foreach (var pi in type.GetProperties()) {
                var attr = pi.GetCustomAttribute<ExprConstAttribute>();
                if (attr == null) {
                    continue;
                }

                if (!Utility.get_value_type(pi.PropertyType, out var ret)) {
                    continue;
                }

                yield return (attr.name ?? pi.Name, new ExpressionExternal {
                    m_ret = ret,
                    m_name = pi.Name,
                    m_property = pi,
                    m_initialized = type,
                });
            }
        }

        private ExpressionExternal() {

        }

        public ValueType ret_type => m_ret;

        private ValueType m_ret;

        private string m_name;

        [System.NonSerialized]
        PropertyInfo m_property = null;

        [System.NonSerialized]
        System.Type m_initialized = null;
    }

    [System.Serializable]
    public class Constant : IExpressionExternal {
        public ValueType type;
        public string name;
        public uint value;

        ValueType IExpressionExternal.ret_type => type;

        public bool get_value(object obj, System.Type obj_type, out object value) {
            value = Utility.convert_from(type, this.value);
            return true;
        }

        public bool set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            calculator.set_external(index, (int)value);
            return true;
        }
    }
}