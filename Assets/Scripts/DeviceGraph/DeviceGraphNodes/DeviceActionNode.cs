

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceActionNode : DeviceNode{

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public DeviceAction method;

        [Input(can_multi_connect = true)]
        public void do_action(DeviceContext ctx) {
            if (method != null) {
                method.invoke(typeof(DeviceContext), ctx, out var ret);
                if (ret is bool b && b) {
                    _true.invoke(ctx);
                } else {
                    _false.invoke(ctx);
                }
            }
        }

        [Output]
        public DeviceEvent _true { get; } = new DeviceEvent();

        [Output]
        public DeviceEvent _false { get; } = new DeviceEvent();
    }
}
