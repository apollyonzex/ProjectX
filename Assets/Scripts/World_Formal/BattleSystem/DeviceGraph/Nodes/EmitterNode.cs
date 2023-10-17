using GraphNode;
using CalcExpr;
using World_Formal.BattleSystem.Device;
using Common_Formal;
using World_Formal.DS;

namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    
    public struct ProjectileEvent
    {
        public Projectile p;
        public object target;
        public DeviceVector2 normal;
    }


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class EmitterNode : DeviceComponentNode
    {
        [ShowInBody(format = "[projectile_id] -> {0}")]
        public int projectile_id;

        [ShowInBody(format = "[dispersion_angle] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression dispersion_angle;

        [ShowInBody(format = "[shot_speed] -> {0}")]
        [ExpressionType(ValueType.Floating)]
        public DeviceExpression shot_speed;

        [ShowInBody(format = "[shot_count] -> {0}")]
        [ExpressionType(ValueType.Integer)]
        public DeviceExpression shot_count;


        [Input]
        [Display("position")]
        public System.Func<DeviceContext, DeviceVector2> emit_position { get; set; }


        [Input]
        [Display("direction")]
        public System.Func<DeviceContext, DeviceVector2> emit_direction { get; set; }

        [Input]
        public void start_fire(DeviceContext ctx)
        {
            UnityEngine.Debug.Log("开炮!");
            var pos = emit_position?.Invoke(ctx);
            var dir = emit_direction?.Invoke(ctx);
            var fireable = can_fire?.Invoke(ctx);
            if(pos == null || dir  == null )
            {
                return;
            }

            if (fireable == null || fireable == false)
            {
                return;
            }
            dispersion_angle.calc(ctx, typeof(DeviceContext), out float angle);
            shot_speed.calc(ctx, typeof(DeviceContext), out float speed);
            shot_count.calc(ctx, typeof(DeviceContext), out int count);

            while (count > 0)
            {
                var rnd_angle = UnityEngine.Random.Range(-angle, angle);
                var direction = UnityEngine.Quaternion.AngleAxis(rnd_angle, UnityEngine.Vector3.forward) * dir.get_normalized();

                var p = new Projectile(pos.v, speed * direction, direction, Enum.EN_faction.player);
                p.init(ctx,projectile_id);
                p.add_component(new ProjectileDefaultComponent(this, p), true);
                Mission.instance.try_get_mgr(Common.Config.ProjectileMgr_Name, out ProjectileMgr pmgr);
                pmgr.AddProjectile(p);
                count--;
            }
        }

        [Input]
        [Display("can_fire")]
        public System.Func<DeviceContext, bool> can_fire { get; set; }

        [Output]
        public DeviceEvent<ProjectileEvent> on_hit_target { get; } = new();

        [Output]
        public DeviceEvent<ProjectileEvent> on_hit_projectile { get; } = new();
        //===============================================================================  init && component

        public override void init(DeviceContext ctx)
        {
            ctx.device.add_component(new EmitterComponent(this),true);
        }
    }
}
