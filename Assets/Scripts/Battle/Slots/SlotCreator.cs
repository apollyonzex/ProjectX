using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Slots
{
    public class SlotCreator : Creator
    {
        public int count;
        public float y;

        public string mgr_name;
        public SlotView model; 

        SlotMgr mgr;

        //==================================================================================================

        public override void @do()
        {
            mgr = new SlotMgr(mgr_name);

            foreach (var cell in cells())
            {
                var view = Instantiate(model, transform);
                mgr.add_cell(cell, view);
            }
        }


        IEnumerable<Slot> cells()
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new(i * 1.5f - 3, y);
                Slot cell = new(i, pos);

                yield return cell;
            }
        }
    }
}

