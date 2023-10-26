using UnityEngine;

namespace Battle.Slots
{
    public class Slot
    {
        public int id;
        public Vector2 pos;

        public Vector2 view_pos => pos;

        //==================================================================================================

        public Slot(int id, Vector2 pos)
        {
            this.id = id;
            this.pos = pos;
        }
    }
}

