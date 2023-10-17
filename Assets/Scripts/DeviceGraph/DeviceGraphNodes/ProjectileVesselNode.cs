using CalcExpr;
using Devices;
using GraphNode;

namespace DeviceGraph {
    public enum Init {
            Max,
            Custom,
    }
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileVesselNode : DeviceNode {

        [ShowInBody(format = "'{0}'")]
        public string module_id;

        public Init init_type;

        [ExpressionType(ValueType.Integer)]
        public Expression custom;

        [ShowInBody(format = "value.max = {0}")]
        [ExpressionType(ValueType.Integer)]
        public Expression max;

        [Input]
        public void add_module(DeviceContext ctx, Projectile p) {
            int value, max_value;
            if (max != null) {
                max.calc(ctx, typeof(DeviceContext), out max_value);
            } else {
                max_value = 1;
            }
            if (init_type == Init.Custom) {
                if (custom != null) {
                    custom.calc(ctx, typeof(DeviceContext), out value);
                } else {
                    value = 0;
                }
            } else {
                value = max_value;
            }
            p.add_component(new ProjectileVessel(this, value, max_value), false);
        }

    }


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileVesselActionNode : DeviceNode {
        [ShowInBody(format = "-> `{0}`")]
        public string module_id;

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public DeviceAction<ProjectileVessel> method;

        [Input]
        public void do_action(DeviceContext ctx,Projectile p) {
            if (method != null) {
                if (p.try_get_component<ProjectileVessel>(module_id, out var component)) {
                    method.invoke(typeof(DeviceContext), ctx, component, out var ret);
                    if (ret is bool b && b) {
                        _action.invoke(ctx, p);
                    }
                    
                }
            }
        }


        [Output]
        public DeviceEvent<Projectile> _action { get; } = new DeviceEvent<Projectile>();
    }

    
}
