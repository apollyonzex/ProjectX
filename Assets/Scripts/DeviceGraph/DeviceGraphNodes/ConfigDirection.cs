using GraphNode;


namespace DeviceGraph
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ConfigDirection : DeviceNode
    {
        public float y;
        public float x;      


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx)
        {
            return (Vector2?)new UnityEngine.Vector2(x, y);
        }
    }
}
