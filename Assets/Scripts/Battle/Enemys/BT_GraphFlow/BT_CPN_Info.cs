using Battle.Enemys.BT_GraphFlow.CPNs;
using System.Collections.Generic;

namespace Battle.Enemys.BT_GraphFlow
{
    public class BT_CPN_Info
    {
        public static Dictionary<string, System.Type> infos_dic = new()
        {
            { "_bool", typeof(BT_Bool) },
            { "_float", typeof(BT_Float) }
        };
    }
}

