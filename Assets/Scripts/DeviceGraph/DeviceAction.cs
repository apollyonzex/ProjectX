

using GraphNode;

namespace DeviceGraph {

    [System.Serializable]
    public class DeviceAction : Action {
        public override Action clone() {
            return clone_to(new DeviceAction());
        }
    }


    [System.Serializable]
    public class DeviceAction<T> : Action<T> {
        public override Action<T> clone() {
            return clone_to(new DeviceAction<T>());
        }
    }
}
