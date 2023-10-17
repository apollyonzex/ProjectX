

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2AddNode2 : DeviceNode {

        [Input]
        [Display("1")]
        public System.Func<DeviceContext, Vector2?> a { get; set; }

        [Input]
        [Display("2")]
        public System.Func<DeviceContext, Vector2?> b { get; set; }

        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            var a = this.a?.Invoke(ctx);
            var b = this.b?.Invoke(ctx);
            if (a != null && b != null) {
                return (Vector2)(a.Value.v + b.Value.v);
            }
            return null;
        }

    }


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2AddNode3 : DeviceNode {

        [Input]
        [Display("1")]
        public System.Func<DeviceContext, Vector2?> a { get; set; }

        [Input]
        [Display("2")]
        public System.Func<DeviceContext, Vector2?> b { get; set; }

        [Input]
        [Display("3")]
        public System.Func<DeviceContext, Vector2?> c { get; set; }

        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            var a = this.a?.Invoke(ctx);
            var b = this.b?.Invoke(ctx);
            var c = this.c?.Invoke(ctx);
            if (a != null && b != null&&c!=null) {
                return (Vector2)(a.Value.v + b.Value.v + c.Value.v);
            }
            return null;
        }

    }


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Vector2AddNode4 : DeviceNode {

        [Input]
        [Display("1")]
        public System.Func<DeviceContext, Vector2?> a { get; set; }

        [Input]
        [Display("2")]
        public System.Func<DeviceContext, Vector2?> b { get; set; }

        [Input]
        [Display("3")]
        public System.Func<DeviceContext, Vector2?> c { get; set; }

        [Input]
        [Display("4")]
        public System.Func<DeviceContext, Vector2?> d { get; set; }

        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            var a = this.a?.Invoke(ctx);
            var b = this.b?.Invoke(ctx);
            var c = this.c?.Invoke(ctx);
            var d = this.d?.Invoke(ctx);
            if (a != null && b != null && c != null) {
                return (Vector2)(a.Value.v + b.Value.v  + c.Value.v + d.Value.v);
            }
            return null;
        }

    }
}
