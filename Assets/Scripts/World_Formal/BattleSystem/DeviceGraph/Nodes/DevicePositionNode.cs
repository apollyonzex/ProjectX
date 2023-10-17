using GraphNode;


namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DevicePositionNode : DeviceNode
    {
        [Output(can_multi_connect =  true)]
        public DeviceVector2 output(DeviceContext ctx)
        {
            return (DeviceVector2)ctx.device.device.position;
        }
    }
}
