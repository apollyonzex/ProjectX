using Common;

namespace World.Times
{
    public class TimeCreator : Creator
    {
        public TimeView model;

        TimeMgr mgr;

        //==================================================================================================

        public override void @do()
        {
            mgr = new TimeMgr(Config.TimeMgr_Name);

            var view = Instantiate(model, transform);
            mgr.add_cell(view);
        }
    }
}

