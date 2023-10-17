﻿using GraphNode;


namespace DeviceGraph {

    [System.Serializable]
    [Graph(typeof(DeviceGraph))]
    public class CustomVector2Node : DeviceNode{


        [ShowInBody(format = "[vector2.y] -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public DeviceExpression y;

        [ShowInBody(format = "[vector2.x] -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public DeviceExpression x;

        [Output(can_multi_connect = true)]
        public Vector2? output(DeviceContext ctx) {
            if(x!=null && x.calc(ctx,typeof(DeviceContext),out float n) && y!=null && y.calc(ctx,typeof(DeviceContext),out float m)) {
                return new Vector2 {
                    v = new UnityEngine.Vector2(n, m),
                    normalized  = false,
                };
            }
            return null;
        } 
    }
}
