
using CalcExpr;
using GraphNode;

namespace BehaviourFlow {

    [System.Serializable]
    public class Expression : Expression<Expression> {

        public AutoCode.Packets.BehaviourFlowExports.ExpressionRef export_as_expression(Exports.Exporter exporter) {
            if (code != null) {
                return new AutoCode.Packets.BehaviourFlowExports.ExpressionRef {
                    ty = AutoCode.Packets.BehaviourFlowExports.ExpressionType.Expression,
                    index = (ulong)exporter.add_expression(code, externals, functions),
                };
            }

            return new AutoCode.Packets.BehaviourFlowExports.ExpressionRef {
                ty = AutoCode.Packets.BehaviourFlowExports.ExpressionType.Constant,
                index = (ulong)exporter.get_constant_index(constant.GetValueOrDefault()),
            };
        }

        public AutoCode.Packets.BehaviourFlowExports.ParamRef export_as_param(Exports.Exporter exporter) {
            if (code != null) {
                return new AutoCode.Packets.BehaviourFlowExports.ParamRef {
                    ty = AutoCode.Packets.BehaviourFlowExports.ParamType.Expression,
                    index = (ulong)exporter.add_expression(code, externals, functions),
                };
            }

            if (constant.HasValue) {
                return new AutoCode.Packets.BehaviourFlowExports.ParamRef {
                    ty = AutoCode.Packets.BehaviourFlowExports.ParamType.Constant,
                    index = (ulong)exporter.get_constant_index(constant.Value),
                };
            }

            return new AutoCode.Packets.BehaviourFlowExports.ParamRef {
                ty = AutoCode.Packets.BehaviourFlowExports.ParamType.String,
                index = (ulong)exporter.get_string_index(content ?? string.Empty),
            };
        }
    }

    [System.Serializable]
    public class ContextSharedInt : IExpressionExternal {
        public readonly string name;
        public ContextSharedInt(string name) {
            this.name = name;
        }

        public ValueType ret_type => ValueType.Integer;

        public bool get_value(object obj, System.Type obj_type, out object value) {
            if (obj is IContext context) {
                value = context.get_shared_int(name);
                return true;
            }
            value = null;
            return false;
        }

        public bool set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is IContext context) {
                calculator.set_external(index, context.get_shared_int(name));
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class ContextSharedFloat : IExpressionExternal {
        public readonly string name;
        public ContextSharedFloat(string name) {
            this.name = name;
        }

        public ValueType ret_type => ValueType.Floating;

        public bool get_value(object obj, System.Type obj_type, out object value) {
            if (obj is IContext context) {
                value = context.get_shared_float(name);
                return true;
            }
            value = null;
            return false;
        }

        public bool set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is IContext context) {
                calculator.set_external(index, context.get_shared_float(name));
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class Constant : CalcExpr.Constant {

    }
}