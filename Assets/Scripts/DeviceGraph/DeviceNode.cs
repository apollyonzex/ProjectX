
using GraphNode;
using UnityEngine;

namespace DeviceGraph {

    [System.Serializable]
    public class DeviceNode : Node {

        public virtual void init(DeviceContext ctx, Devices.DeviceConfig[] config) {

        }
    }

    [System.Serializable]
    public class DeviceComponentNode : DeviceNode {

        public int tick_order;

    }

    /*

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DeviceEmitter : DeviceNode {

        [Input]
        [Display("position")]
        public System.Func<DeviceContext, Vector2> emit_position { get; set; }

        [Input]
        public void emit(DeviceContext context) {

        }

    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DevicePosition : DeviceNode {

        [Output(can_multi_connect = true)]
        [Display("position")]
        public Vector2 get_position(DeviceContext context) {
            return Vector2.zero;
        }
    }


    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class EveryTick : DeviceNode {


        [Output]
        public DeviceEvent tick { get; } = new DeviceEvent();
    }

    public class Particle {

    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Projectile : DeviceNode {


        [Output]
        public DeviceEvent<Particle> create { get; } = new DeviceEvent<Particle>();
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class Moving : DeviceNode {

        [Input]
        public void move(DeviceContext context, Particle particle) {

        }

    }

    */
}
