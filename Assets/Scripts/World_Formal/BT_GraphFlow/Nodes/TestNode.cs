using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class TestNode : BT_Node
    {
        [ShowInBody(format = "-> {0}")]
        [ExpressionType(CalcExpr.ValueType.Integer)]
        public Expression b;

        [ShowInBody]
        [ActionReturn(typeof(bool))]
        public Action<Test> method;

        //================================================================================================

        #region Input
        [Input]
        [Display("string", seq = 1)]
        public System.Func<BT_Context, string> _in1 { get; set; }

        [Input]
        [Display("bool", seq = 2)]
        public System.Func<BT_Context, bool> _in2 { get; set; }

        [Input]
        [Display("float", seq = 3)]
        public System.Func<BT_Context, float> _in3 { get; set; }
        #endregion


        #region Output
        [Output]
        [Display("out1", seq = 1)]
        public string _out(BT_Context ctx)
        {
            UnityEngine.Debug.Log($"out1_{comment}");
            return "";
        }


        [Output]
        [Display("out2", seq = 2)]
        public string _out2(BT_Context ctx)
        {
            UnityEngine.Debug.Log($"out2_{comment}");
            return "";
        }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            UnityEngine.Debug.Log($"do_{comment}");
        }
    }
}

