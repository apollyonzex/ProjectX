using UnityEngine;

namespace World_Formal.Caravans
{
    public class DashBoard  : MonoBehaviour
    {
        public float maxSpeed;

        public float default_angle = 135;

        WorldContext ctx;
        private void Start()
        {
            ctx = WorldContext.instance;
        }
        private void Update()
        {
            var angle = ctx.caravan_velocity.magnitude / maxSpeed * 270f;
            transform.rotation = Quaternion.AngleAxis(default_angle + angle, Vector3.forward);
        }
    }
}
