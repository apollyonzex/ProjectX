using System;
using System.Collections.Generic;
using Common_Formal;
using Foundation;


namespace World_Formal.BattleReward
{
    public interface IBattleRewardView :IModelView<BattleRewardMgr>
    {

    }
    public class BattleRewardMgr : Model<BattleRewardMgr, IBattleRewardView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;
        public BattleRewardMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }

        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }

        void IMgr.init(params object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);
        }

        //=======================================================================================================================

        public BattleRewardCell[,,] BattleRewards = new BattleRewardCell[99,12,6];
    }

    public class BattleRewardCell
    {

    }
}
