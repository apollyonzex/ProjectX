using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes.OprNodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Vector2_SubNode : BT_OprNode
    {


        //================================================================================================

        #region Input
        [Input]
        [Display("a", seq = 1)]
        public System.Func<BT_Context, Vector2?> _i_a { get; set; }


        [Input]
        [Display("b", seq = 2)]
        public System.Func<BT_Context, Vector2?> _i_b { get; set; }
        #endregion


        #region OutPut
        [Output]
        [Display("out", seq = 1)]
        public Vector2? _o(BT_Context ctx)
        {
            var a = _i_a?.Invoke(ctx);
            var b = _i_b?.Invoke(ctx);

            if (a == null || b == null) return null;

            return a - b;
        }
        #endregion
    }
}

