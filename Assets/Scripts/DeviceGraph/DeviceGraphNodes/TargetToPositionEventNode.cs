

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class TargetToPositionEventNode : DeviceNode {

        [Output]
        public DeviceEvent<Vector2?> output { get; } = new();

        [Input(can_multi_connect = true)]
        public void input(DeviceContext ctx, Devices.ITarget target) {
            Vector2? pos;
            if (target != null) {
                pos = (Vector2)target.position;
            } else {
                pos = null;
            }
            output.invoke(ctx, pos);
        } 

    }
}
