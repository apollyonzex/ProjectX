using Common_Formal;
using Foundation;

namespace World_Formal.Map.Journey
{
    public interface IJounrenyView : IModelView<JourneyMgr>
    { 
        
    }


    public class JourneyMgr : Model<JourneyMgr, IJounrenyView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        //==================================================================================================

        public JourneyMgr(string name, params object[] objs)
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
            world_id = (uint)objs[0] ;
            level_id = (uint)objs[1] ;
        }

        //================================================================================================

        public uint world_id;
        public uint level_id;
    }
}

