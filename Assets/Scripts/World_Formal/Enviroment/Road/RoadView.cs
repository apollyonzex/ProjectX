using UnityEngine;

namespace World_Formal.Enviroment.Road
{
    public class RoadView : MonoBehaviour
    {
        public Curve curve;
        public void init(Curve curve) {
            this.curve = curve;

            GetComponent<SpriteRenderer>().sprite = curve.curve_sprite;
        }

    }
}
