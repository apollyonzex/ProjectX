using GraphNode;
using UnityEngine;

namespace DeviceGraph {
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class DevicePositionNode : DeviceNode {

        [Output(can_multi_connect = true)]
        public Vector2? get_position(DeviceContext context) {
            return (Vector2)context.device.position;
        }
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]

    public class DebugNode :DeviceNode{

        [Input]
        public void OutputDebug(DeviceContext context) {
            Debug.Log("test");
        }
    }

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]

    public class DebugEmittingNode : DeviceNode {

        [Input]
        public void OutputDebug(DeviceContext context,Emitting e) {
            Debug.Log(e);
        }
    }
}
