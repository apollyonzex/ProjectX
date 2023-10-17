using System.Collections.Generic;
using World_Formal.BT_GraphFlow.Nodes.DS_Nodes;

namespace World_Formal.BT_GraphFlow.Expression_Externals
{
    public class BT_CPN_Info_Dic
    {
        public static Dictionary<string, (System.Type cpn_type, System.Type ee_type)> cpn_infos_dic = new()
        {
            { "_float", (typeof(BT_Float), typeof(BT_EE)) },
            { "_sfloat", (typeof(BT_ShareFloat), typeof(BT_EE)) },
            { "_rfloat", (typeof(BT_Random_Float), typeof(BT_EE)) },

            { "_int", (typeof(BT_Int), typeof(BT_EE)) },
            { "_sint", (typeof(BT_ShareInt), typeof(BT_EE)) },

            { "_bool", (typeof(BT_Bool), typeof(BT_EE)) },
            { "_sbool", (typeof(BT_ShareBool), typeof(BT_EE)) },

            { "self", (typeof(BT_Self), typeof(BT_EE)) },
            { "caravan", (typeof(BT_Caravan), typeof(BT_EE)) },

            { "_vector2", (typeof(BT_Vector2), typeof(BT_EE)) },
            
        };
    }
}

