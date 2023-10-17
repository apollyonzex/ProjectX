
using GraphNode;
using CalcExpr;
using System.Collections.Generic;

namespace BehaviourFlow {

    [System.Serializable]
    public class BehaviourTree : Graph {

        public override System.Type context_type => typeof(IContext);

        public Nodes.EntryNode entry { get; set; }

        [Display("Constants")]
        public List<Constant> constants;

        public Dictionary<string, Nodes.ExternalNode> externals = new Dictionary<string, Nodes.ExternalNode>();
    }

}
