using GraphNode;
using CalcExpr;
using World_Formal.BattleSystem.Device;

namespace World_Formal.BattleSystem.DeviceGraph.Nodes
{
    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class ProjectileActionNode :   DeviceNode
    {
        [ActionReturn(typeof(bool))]
        public DeviceAction<ProjectileEvent> projectile_method;

        [Input]
        public  void invoke(DeviceContext  ctx,ProjectileEvent pe)
        {
            if (projectile_method != null)
            {
                projectile_method.invoke(typeof(DeviceContext), ctx,pe , out var ret);
            }
        }
    }
}
