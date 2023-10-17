

using DialogFlow;
using GraphNode;

namespace Assets.Scripts.World_Formal.Dialog_Formal
{
    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class illustrationPageNode : PageNode
    {
        [Display("illustration bundle")]
        [SortedOrder(4)]
        public string i_bundle;

        [Display("illustration path")]
        [SortedOrder(5)]
        public string i_path;

        public override void show(DialogFlow.IContext context)
        {
            if(context is IXContext ixcontext)
            {
                ixcontext.show_illustration(i_bundle, i_path);
            }
            base.show(context);
        }
    }
}
