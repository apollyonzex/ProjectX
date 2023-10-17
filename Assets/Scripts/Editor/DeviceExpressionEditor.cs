

using DeviceGraph;
using Devices;
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using Worlds.Missions.Battles.Enemies;

namespace Editor {
    [PropertyEditor(typeof(DeviceExpression))]
    public class DeviceExpressionEditor : ExpressionEditor<DeviceExpression> {

        public override ExpressionBase create_expression() {
            return new DeviceExpression();
        }

        protected override bool get_external(string name, out CalcExpr.ValueType ty, out CalcExpr.IExpressionExternal external) {                       //Expression扩展
            var p1 = name.IndexOf('.');
            if (p1 >= 0) {
                var n1 = name.Substring(0, p1);
                var t1 = name.Substring(p1 + 1);

                if (n1 == "projectile") {
                    var dict = Utility.get_expression_externals(typeof(Projectile));
                    if (dict.TryGetValue(t1, out var ee)) {
                        ty = ee.ret_type;
                        external = new DeviceExpression_ProjectileExternal(ee);
                        return true;
                    }
                } else if (n1 == "enemy") {
                    var dict = Utility.get_expression_externals(typeof(Enemy));
                    if (dict.TryGetValue(t1, out var ee)) {
                        ty = ee.ret_type;
                        external = new DeviceExpression_EnemyExternal(ee);
                        return true;
                    }
                }else if(n1 == "normal") {
                    var dict = Utility.get_expression_externals(typeof(Vector2));
                    if (dict.TryGetValue(t1, out var ee)) {
                        ty = ee.ret_type;
                        external = new DeviceExpression_NormalExternal(ee);
                        return true;
                    }
                }
                else {
                    var p2 = t1.IndexOf(".");
                    if (p2 >= 0) {
                        var n2 = t1.Substring(0, p2);
                        var n3 = t1.Substring(p2 + 1);
                        if (get_component_external(n1, n2, n3, out ty, out external)) {
                            return true;
                        }
                    }
                }
            }
            return base.get_external(name, out ty, out external);
        }


        private bool get_component_external(string component, string module_name, string property_name, out CalcExpr.ValueType ty, out CalcExpr.IExpressionExternal external) {
            if (s_component_types.TryGetValue(component, out var info)) {
                var dict = Utility.get_expression_externals(info.cty);
                if (dict.TryGetValue(property_name, out var cee)) {
                    ty = cee.ret_type;
                    external = info.convert(info.cty, module_name, cee);
                    return true;
                }
            }
            ty = CalcExpr.ValueType.Unknown;
            external = null;
            return false;
        }

        static CalcExpr.IExpressionExternal device_component(System.Type cty, string name, CalcExpr.IExpressionExternal cee) {
            return new DeviceExpression_ComponentExternal(cty, name, cee);
        }

        static CalcExpr.IExpressionExternal projectile_component(System.Type cty, string name, CalcExpr.IExpressionExternal cee) {
            return new DeviceExpression_ProjectileComponentExternal(cty, name, cee);
        }

        static CalcExpr.IExpressionExternal device_boolean_component(System.Type cty, string name, CalcExpr.IExpressionExternal cee) {
            return new DeviceExpression_BooleanComponentExternal(cty, name, cee);
        }
        static CalcExpr.IExpressionExternal caravan_component(System.Type cty, string name, CalcExpr.IExpressionExternal cee) {
            return new DeviceExpression_CaravanExternal(cty, name, cee);
        }
        static CalcExpr.IExpressionExternal device_float_component(System.Type cty, string name, CalcExpr.IExpressionExternal cee) {
            return new DeviceExpression_FloatComponentExternal(cty, name, cee);
        }
        static CalcExpr.IExpressionExternal device_vector2_component(System.Type cty, string name, CalcExpr.IExpressionExternal cee) {
            return new DeviceExpression_Vector2ComponentExternal(cty, name, cee);
        }

        static DeviceExpressionEditor() {
            s_component_types = new Dictionary<string, (System.Type, System.Func<System.Type, string, CalcExpr.IExpressionExternal, CalcExpr.IExpressionExternal>)> {
                { "device_vessel", (typeof(DeviceVessel), device_component) },
                { "projectile_vessel", (typeof(ProjectileVessel), projectile_component) },
                { "device_bool",(typeof(DeviceBoolean),device_boolean_component)},
                { "caravan",(typeof(CaravanData),caravan_component)},
                { "device_float",(typeof(DeviceFloat), device_float_component)},
                { "device_vector2",(typeof(DeviceVector2),device_vector2_component)}
            };
    }

        static Dictionary<string, (System.Type cty, System.Func<System.Type, string, CalcExpr.IExpressionExternal, CalcExpr.IExpressionExternal> convert)> s_component_types;
    }
}
