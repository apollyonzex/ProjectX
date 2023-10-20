using Common;
using Foundation;
using UnityEngine;

namespace World.Times
{
    public interface ITimeView : IModelView<TimeMgr>
    {
        void notify_on_tick1();
    }


    public class TimeMgr : Model<TimeMgr, ITimeView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        WorldContext ctx;

        int m_delta;
        public string view_time => ctx.world_time.ToString();

        //==================================================================================================

        public TimeMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }


        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }


        void IMgr.init(object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            ctx = WorldContext.instance;
            ctx.add_tick(Config.TimeMgr_Priority, Config.TimeMgr_Name, tick);
            ctx.add_tick1(Config.TimeMgr_Priority, Config.TimeMgr_Name, tick1);
        }


        void tick()
        {
            m_delta++;
            if (m_delta == 120)
            {
                m_delta = 0;
                ctx.world_time++;
            }
        }


        void tick1()
        {
            if (m_delta != 0) return;

            foreach (var view in views)
            {
                view.notify_on_tick1();
            }
        }


        public void load_view(Transform parent)
        {
            EX_Utility.quick_add_view<TimeMgr, ITimeView, TimeView>(("ui", "parts/Time"), this, parent, out var _);
        }
    }
}

