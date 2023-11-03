using GraphNode;
using System;
using System.Collections.Generic;

namespace Battle.Enemys.BT_GraphFlow
{
    [System.Serializable]
    public class BT_Graph : Graph
    {
        public override Type context_type => typeof(BT_Context);

        public System.Action<BT_Context> start_ac;
        public Dictionary<string, BT_CPN> cpns_dic;

        //==================================================================================================

        public override void after_deserialize(GraphAsset asset, UnityEngine.Object[] referenced_objects)
        {
            cpns_dic = new();

            foreach (var node in nodes)
            {
                if (node is Nodes.StartNode _s)
                    start_ac = _s.do_start;

                if (node is BT_DSNode _ds)
                {
                    _ds.init();
                    cpns_dic.Add(_ds.module_name, _ds.cpn);
                }

                node.on_deserialized(referenced_objects);
            }
        }

    }
}
