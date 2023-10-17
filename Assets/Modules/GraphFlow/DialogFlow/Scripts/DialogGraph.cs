
using GraphNode;
using CalcExpr;
using System.Collections.Generic;

namespace DialogFlow {

    [System.Serializable]
    public class DialogGraph : Graph {

        public override System.Type context_type => typeof(IContext);

        public virtual bool localizable => false;
        public virtual bool try_localize(string key, out string content) {
            content = null;
            return false;
        }

        [Display("Constants")]
        public List<Constant> constants;

        public EntryNode entry { get; set; }

        public Dictionary<string, Nodes.External> externals = new Dictionary<string, Nodes.External>();
        public Dictionary<string, Nodes.OptionState_External> option_state_externals = new Dictionary<string, Nodes.OptionState_External>();
        public Dictionary<string, Nodes.PageOptions_External> options_externals = new Dictionary<string, Nodes.PageOptions_External>();
        
    }

}