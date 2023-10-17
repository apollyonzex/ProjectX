

using DeviceGraph;

namespace Devices {
    public class DeviceComponent {

        public virtual string name => null;
        public virtual DeviceNode graph_node => null;
        public virtual int tick_order => 0;

        public virtual void start(DeviceContext ctx) {

        }

        public virtual void tick(DeviceContext ctx) {

        }

        public virtual void late_tick(DeviceContext ctx) {

        }
    }
}
