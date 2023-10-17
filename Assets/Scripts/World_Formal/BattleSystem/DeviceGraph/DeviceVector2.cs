
using CalcExpr;
using UnityEngine;



namespace World_Formal.BattleSystem.DeviceGraph
{
    public class DeviceVector2
    {
        [ExprConst]
        public float x => v.x;

        [ExprConst]
        public float y => v.y;

        public Vector2 v;
        public bool normalized;

        public DeviceVector2(Vector2 v, bool normalized)
        {
            this.v = v;
            this.normalized = normalized;
        }

        /// <returns>在vector为零向量的时候会返回右向量</returns>
        public Vector2 get_normalized()
        {
            if (normalized)
            {
                return v;
            }
            if(v == Vector2.zero)
            {
                return Vector2.right;
            }
            return v.normalized;
        }

        public static explicit operator DeviceVector2(Vector2 value){
            return new DeviceVector2(value, false);
        }
    }
}
