
using GraphNode;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Input(can_multi_connect = true)]
    public class PageOptionsBase : DialogNode {

        public virtual int show_options(IContext context, ref int index) { return 0; }
        public virtual void skip_options(IContext context, ref int index) { }
        public virtual bool do_option(IContext context, ref int index) { context.end(); return true; }
    }
}