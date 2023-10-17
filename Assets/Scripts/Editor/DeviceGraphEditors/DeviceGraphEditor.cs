using GraphNode.Editor;
using GraphNode;
using UnityEngine;


namespace World_Formal.BattleSystem.DeviceGraph             //因为demo版本把类名占用了,这个就先放在图流命名空间下
{
    [GraphEditor(typeof(DeviceGraph))]
    public class DeviceGraphEditor : GenericGraphEditor {

        public override Color query_node_port_color(NodePort port) {
            var event_color = new Color32(0, 150, 200, 255); 
            var data_color = new Color32(0, 240, 120, 255);
            var v2_color = new Color32(0, 140, 70, 255);            //vector2数据的颜色
            var bool_color = new Color32(120, 200, 100, 255);       //bool数据的颜色
            var float_color = new Color32(130, 160, 0, 255);       //float数据的颜色
            if (port is NodeMethodPort ) {
                if(port.io == NodePort.IO.Input) {
                    return event_color;
                }
                if(port.sign_ret == typeof(DeviceVector2)) {
                    return v2_color;
                }
                if(port.sign_ret == typeof(bool))
                    return bool_color;
                if (port.sign_ret == typeof(float)) {
                    return float_color;
                }
                return data_color;
            }
            if(port is NodePropertyPort_Delegate) {
                if (port.sign_ret == typeof(DeviceVector2)) {
                    return v2_color;
                }
                if (port.sign_ret == typeof(bool)) { 
                    return bool_color; 
                }
                if (port.sign_ret == typeof(float)) {
                    return float_color;
                }
                return data_color;
            }
            return event_color;
        }
    }
}
