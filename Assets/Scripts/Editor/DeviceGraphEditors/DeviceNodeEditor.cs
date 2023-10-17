
using UnityEngine;
using GraphNode.Editor;
using GraphNode;
using Worlds.CardSpace;

namespace World_Formal.BattleSystem.DeviceGraph             //因为demo版本把类名占用了,这个就先放在图流命名空间下
{

    [NodeEditor(typeof(DeviceNode))]
    public class DeviceNodeEditor : GenericNodeEditor {

        public override void on_port_desc_gui(PortView port) {
            if (port.port is DeviceNodePort_Event) {
                var style = GraphResources.styles.node_output_desc;
                GUILayout.Label("_", style);
                return;
            }

            if (port.port is DeviceNodePort_EventWith) {
                var style = GraphResources.styles.node_output_desc;
                var ty = port.port.sign_params[1];
                GUILayout.Label(ty.Name, style);
            }

            if (port.port is NodeMethodPort) {
                if (port.port.io == NodePort.IO.Output) {
                    var style = GraphResources.styles.node_output_desc;
                    if (port.port.sign_ret == typeof(DeviceVector2)) {
                        GUILayout.Label("v2", style);
                    }
                    if (port.port.sign_ret == typeof(bool)) {
                        GUILayout.Label("bool", style);
                    }
                    if (port.port.sign_ret == typeof(float)) {
                        GUILayout.Label("float", style);
                    }
                } else {
                    var style = GraphResources.styles.node_input_desc;
                    var ps = port.port.sign_params;
                    if (ps.Count > 1) {
                        var ty = ps[1];
                        GUILayout.Label(ty.Name, style);
                    } else {
                        GUILayout.Label("_", style);
                    }
                    
                }
                return;
            }

            if (port.port is NodePropertyPort_Delegate) {
                var style = GraphResources.styles.node_input_desc;
                if (port.port.sign_ret == typeof(DeviceVector2)) {
                    GUILayout.Label("v2", style);
                }
                if (port.port.sign_ret == typeof(bool)) {
                    GUILayout.Label("bool", style);
                }
                if (port.port.sign_ret == typeof(float)) {
                    GUILayout.Label("float", style);
                }
            }
        }
    }
}
