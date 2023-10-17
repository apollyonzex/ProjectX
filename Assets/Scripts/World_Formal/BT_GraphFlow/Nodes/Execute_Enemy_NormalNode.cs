using GraphNode;
using UnityEngine;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Execute_Enemy_NormalNode : StartNode
    {


        //================================================================================================

        #region Output
        [Display("target", seq = 1)]
        [Output]
        public void o_target(BT_Context ctx)
        {

        }


        [Display("move", seq = 2)]
        [Output]
        public void o_move(BT_Context ctx)
        {
        }


        [Display("action", seq = 3)]
        [Output]
        public void o_action(BT_Context ctx)
        {
        }


        [Display("face_direction", seq = 4)]
        [Output]
        public void o_face_dir(BT_Context ctx)
        {
        }


        [Display("end", seq = 99)]
        [Output]
        public void o_end(BT_Context ctx)
        {
        }
        #endregion
    }
}

