

using Devices;
using GraphNode;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CaravanActionNode : DeviceNode {

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public DeviceAction<CaravanData> method;

        [Input(can_multi_connect = true)]
        public void do_action(DeviceContext ctx) {
            if (method != null) {
                method.invoke(typeof(DeviceContext), ctx, null, out var ret);   //这里篷车会直接取单例里面的,但是为了把method和 device中变量的method 区分;加了一个没有意义的caravan_Data
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
