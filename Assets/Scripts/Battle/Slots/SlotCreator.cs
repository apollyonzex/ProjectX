using Common;
using System.Collections.Generic;

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
                mgr.load_view();
            }
        }


        IEnumerable<Slot> cells()
        {
            for (int i = 0; i < count; i++)
            {
                Slot cell = new()
                {
                    id = i,
                    pos = new(i * 1.5f - 3, y),
                };

                yield return cell;
            }
        }
    }
}

