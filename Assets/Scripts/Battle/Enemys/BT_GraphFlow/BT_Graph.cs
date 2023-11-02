using GraphNode;
using System;

namespace Battle.Enemys.BT_GraphFlow
{
    [System.Serializable]
    public class BT_Graph : Graph
    {
        public override Type context_type => typeof(BT_Context);

        public System.Action<BT_Context> start_ac;

        //==================================================================================================

        public override void after_deserialize(GraphAsset asset, UnityEngine.Object[] referenced_objects)
        {
            foreach (var node in nodes)
            {
                if (node is Nodes.StartNode _s)
                    start_ac = _s.do_start;

                node.on_deserialized(referenced_objects);
            }
        }

    }
}
