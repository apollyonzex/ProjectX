

using InvokeFlow;

namespace StateFlow {

    [System.Serializable]
    public class StateGraph : InvokeGraph {
        public override System.Type context_type => typeof(IContext);

        public EntryStateNode entry { get; set; }


    }
}