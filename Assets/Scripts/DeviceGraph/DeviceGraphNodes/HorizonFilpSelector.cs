using UnityEngine;
using GraphNode;


namespace DeviceGraph 
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class HorizonFilpSelector : DeviceNode
    {
        [Input]
        public System.Func<DeviceContext, Vector2?> left { get; set; }

        [Input]
        public System.Func<DeviceContext, Vector2?> right { get; set; }

        [Input]
        public System.Func<DeviceContext, Vector2?> direction { get; set; }


        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx)
        {
            var dir = this.direction?.Invoke(ctx);
            var left = this.left?.Invoke(ctx);
            var right = this.right?.Invoke(ctx);

            UnityEngine.Vector2 v;
            if (dir == null) v = UnityEngine.Vector2.right;
            else v = dir.Value.v;

            if (v.x > 0) return right;
            else return left;         
        }
    }


}

