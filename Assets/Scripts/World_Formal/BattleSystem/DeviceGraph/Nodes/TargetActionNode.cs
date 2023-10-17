using GraphNode;
using CalcExpr;
using World_Formal.BattleSystem.Device;
using Common_Formal;
using World_Formal.DS;

namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class TargetActionNode  :DeviceNode
    {
        [ActionReturn(typeof(bool))]
        public DeviceAction<ITarget> target_method;

        [Input]
        public void invoke(DeviceContext ctx,ProjectileEvent pe)
        {
            if (target_method != null)
            {
                target_method.invoke(typeof(DeviceContext), ctx, (pe.target as  ITarget), out var ret);
            }
        }
    }
}
