using System.Collections.Generic;

using GraphNode;
using InvokeFlow;


namespace StateFlow {
    [System.Serializable]
    [Graph(typeof(StateGraph))]
    public class StateEventNode : InvokeNode {

        [NonProperty]
        public string name;

        [NonProperty]
        public readonly List<Parameter> available_parameters = new List<Parameter>();

        [Display("Parameters")]
        public Parameters parameters;

    }
}