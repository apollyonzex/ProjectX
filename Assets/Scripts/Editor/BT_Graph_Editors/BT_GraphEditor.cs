using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.BT_GraphFlow;
using World_Formal.Enemys.Projectiles;

namespace Scripts.Editor.BT_Graph_Editors
{
    [GraphEditor(typeof(BT_Graph))]
    public class BT_GraphEditor : GenericGraphEditor
    {
        Dictionary<System.Type, Color> m_color_from_ret_dic = new()
        {
            { typeof(bool) , new Color32(120, 200, 100, 255) },
            { typeof(float) , new Color32(130, 160, 0, 255) },
            { typeof(string) , new Color32(0, 240, 120, 255) },
            { typeof(Vector2?) , new Color32(0, 240, 120, 255) },
        };

        Dictionary<System.Type, Color> m_color_from_prms_dic = new()
        {
            { typeof(Projectile) , new Color32(120, 200, 100, 255) },
        };

    //==================================================================================================

    public override Color query_node_port_color(NodePort port)
        {
            if (m_color_from_ret_dic.TryGetValue(port.sign_ret, out var color0))
                return color0;

            var prms = port.sign_params;
            if (prms.Count > 1)
            {
                if (m_color_from_prms_dic.TryGetValue(prms[1], out var color1))
                    return color1;
            }

            return new Color32(0, 150, 200, 255);
        }
    }
}

