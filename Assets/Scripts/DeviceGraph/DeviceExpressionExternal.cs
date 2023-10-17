

using CalcExpr;
using Devices;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using Worlds.Missions.Battles.Enemies;

namespace DeviceGraph {

    [System.Serializable]
    public class DeviceExpression_ComponentExternal : IExpressionExternal {

        private System.Type m_module_type;
        private string m_module_name;
        private IExpressionExternal m_external;

        public DeviceExpression_ComponentExternal(System.Type module_type, string module_name, IExpressionExternal external) {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;
        }


        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.get_value(component, m_module_type, out value);
                }
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.set_external(calculator, component, m_module_type, index);
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class DeviceExpression_CaravanExternal : IExpressionExternal {

        private System.Type m_module_type;
        private string m_module_name;
        private IExpressionExternal m_external;

        public DeviceExpression_CaravanExternal(System.Type module_type, string module_name, IExpressionExternal external) {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;
        }


        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.get_value(component, m_module_type, out value);
                }
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.set_external(calculator, component, m_module_type, index);
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class DeviceExpression_FloatComponentExternal : IExpressionExternal {

        private System.Type m_module_type;
        private string m_module_name;
        private IExpressionExternal m_external;

        public DeviceExpression_FloatComponentExternal(System.Type module_type, string module_name, IExpressionExternal external) {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;
        }


        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.get_value(component, m_module_type, out value);
                }
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.set_external(calculator, component, m_module_type, index);
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class DeviceExpression_Vector2ComponentExternal : IExpressionExternal {

        private System.Type m_module_type;
        private string m_module_name;
        private IExpressionExternal m_external;

        public DeviceExpression_Vector2ComponentExternal(System.Type module_type, string module_name, IExpressionExternal external) {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;
        }


        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.get_value(component, m_module_type, out value);
                }
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.set_external(calculator, component, m_module_type, index);
                }
            }
            return false;
        }
    }



    [System.Serializable]
    public class DeviceExpression_ProjectileExternal : IExpressionExternal {
        private IExpressionExternal m_external;

        public DeviceExpression_ProjectileExternal(IExpressionExternal external) {
            m_external = external;
        }

        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx && ctx.current_projectile != null) {
                return m_external.get_value(ctx.current_projectile, typeof(Projectile), out value);
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx && ctx.current_projectile != null) {
                return m_external.set_external(calculator, ctx.current_projectile, typeof(Projectile), index);
            }
            return false;
        }
    }

    [System.Serializable]
    public class DeviceExpression_ProjectileComponentExternal : IExpressionExternal {

        private System.Type m_module_type;
        private string m_module_name;
        private IExpressionExternal m_external;

        public DeviceExpression_ProjectileComponentExternal(System.Type module_type, string module_name, IExpressionExternal external) {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;
        }


        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx && ctx.current_projectile != null) {
                if (ctx.current_projectile.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.get_value(component, m_module_type, out value);
                }
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx && ctx.current_projectile != null) {
                if (ctx.current_projectile.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.set_external(calculator, component, m_module_type, index);
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class DeviceExpression_EnemyExternal : IExpressionExternal {
        private IExpressionExternal m_external;

        public DeviceExpression_EnemyExternal(IExpressionExternal external) {
            m_external = external;
        }

        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx && ctx.current_enemy != null) {
                return m_external.get_value(ctx.current_enemy, typeof(Enemy), out value);
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx && ctx.current_enemy != null) {
                return m_external.set_external(calculator, ctx.current_enemy, typeof(Enemy), index);
            }
            return false;
        }
    }
    [System.Serializable]
    public class DeviceExpression_BooleanComponentExternal : IExpressionExternal {

        private System.Type m_module_type;
        private string m_module_name;
        private IExpressionExternal m_external;

        public DeviceExpression_BooleanComponentExternal(System.Type module_type, string module_name, IExpressionExternal external) {
            m_module_type = module_type;
            m_module_name = module_name;
            m_external = external;
        }


        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.get_value(component, m_module_type, out value);
                }
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx) {
                if (ctx.device.try_get_component(m_module_type, m_module_name, out var component)) {
                    return m_external.set_external(calculator, component, m_module_type, index);
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class DeviceExpression_NormalExternal : IExpressionExternal {
        private IExpressionExternal m_external;

        public DeviceExpression_NormalExternal(IExpressionExternal external) {
            m_external = external;
        }

        ValueType IExpressionExternal.ret_type => m_external.ret_type;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            if (obj is DeviceContext ctx && ctx.current_normal != null) {
                return m_external.get_value(ctx.current_normal, typeof(Vector2), out value);
            }
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is DeviceContext ctx && ctx.current_normal != null) {
                return m_external.set_external(calculator, ctx.current_normal, typeof(Vector2), index);
            }
            return false;
        }
    }
}
