
using CalcExpr;

namespace DeviceGraph {
    public struct Vector2 {
        [ExprConst]
        public float x => v.x;

        [ExprConst]
        public float y => v.y;

        public UnityEngine.Vector2 v;
        public bool normalized;

        public Vector2(UnityEngine.Vector2 ve,bool n) {
            this.v = ve;
            normalized = n;
        }


        public UnityEngine.Vector2 get_normalized() {
            if (normalized) {
                return v;
            }
            if (v == UnityEngine.Vector2.zero) {
                return UnityEngine.Vector2.right;
            }
            return v.normalized;
        }

        public static explicit operator Vector2(UnityEngine.Vector2 value) {
            return new Vector2 {
                v = value,
                normalized = false,
            };
        }
    }
}
