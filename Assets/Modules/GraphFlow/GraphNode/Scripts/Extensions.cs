
namespace GraphNode {

    public static class Extensions {
        public static bool can_connect_with(this NodePort.IO self, NodePort.IO other) {
            return (int)self + (int)other == 3;
        }
        public static bool is_input(this NodePort.IO self) => self == NodePort.IO.Input;
        public static bool is_output(this NodePort.IO self) => self == NodePort.IO.Output;
    }

}