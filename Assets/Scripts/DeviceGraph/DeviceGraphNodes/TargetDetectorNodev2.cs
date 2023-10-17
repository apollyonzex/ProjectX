using GraphNode;
using UnityEngine;
using Worlds.Missions.Battles;
using static Devices.Device;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class TargetDetectorNodev2 : DeviceComponentNode {

        public float radius;
        [Input]
        public System.Func<DeviceContext, Vector2?> center { get; set; }

        [Output(can_multi_connect = true)]
        [Display("target_position")]
        public Vector2? get_target_position(DeviceContext ctx) {
            if (ctx.device.try_get_component(this, out Devices.TargetDetectorv2 component)) {
                var target = component.target;
                if (target != null) {
                    return (Vector2)target.position;
                }
            }
            return null;
        }


        [Output]
        public DeviceEvent<Devices.ITarget> target_enter { get; } = new();

        [Output]
        public DeviceEvent<Devices.ITarget> target_stay { get; } = new();

        [Output]
        public DeviceEvent<Devices.ITarget> target_leave { get; } = new();

        [Output(can_multi_connect = true)]
        public bool has_target(DeviceContext ctx) {
            if (ctx.device.try_get_component(this, out Devices.TargetDetectorv2 component)) {
                var target = component.target;
                if (target != null) {
                    return true;
                }
            }
            return false;
        }


        public override void init(DeviceContext ctx, Devices.DeviceConfig[] config) {
            ctx.device.add_component(new Devices.TargetDetectorv2(this), true);
        }
    }
}
