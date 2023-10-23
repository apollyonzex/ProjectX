using Common;
using Foundation;

namespace World.BattleFields
{
    public interface IBattleFieldView : IModelView<BattleFieldMgr>
    { 
        
    }


    public class BattleFieldMgr : Model<BattleFieldMgr, IBattleFieldView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        //==================================================================================================

        public BattleFieldMgr(string name, params object[] objs)
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
    }
}

