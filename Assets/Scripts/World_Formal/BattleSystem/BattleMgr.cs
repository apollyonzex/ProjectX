using Common_Formal;
using Foundation;

namespace World_Formal.BattleSystem
{
    public interface IBattleView : IModelView<BattleMgr>
    { 
        
    }


    public class BattleMgr : Model<BattleMgr, IBattleView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        //==================================================================================================

        public BattleMgr(string name, params object[] objs)
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
        }

        //==================================================================================================

    }
}

