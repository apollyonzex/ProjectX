using Common_Formal;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;


namespace World_Formal.Adventure
{
    public class PlayerExp : Exp
    {
        public int talent_point;
        private Stack<UpgradeBonus> bonus_queue = new Stack<UpgradeBonus>();
        public AutoCode.Tables.PlayerExp.Record current_record;
        public PlayerExp(uint init_level, int init_exp, AutoCode.Tables.PlayerExp.Record current_record) : base(init_level, init_exp)
        {
            this.current_record = current_record;
        }

        public override void AddExp(int delta)
        {
            exp += delta;
            while (exp > current_record.f_exp && level <  Common.Config.current.max_player_level)
            {
                LevelUp();
            }
        }

        protected override void LevelUp()
        {
            exp -= current_record.f_exp;
            ++ m_level;
            AutoCode.Tables.PlayerExp player_exp = new AutoCode.Tables.PlayerExp();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "player_exp", out var asset);
            player_exp.load_from(asset);
            player_exp.try_get(level, out var t);
            if (t == null)
            {
                UnityEngine.Debug.LogWarning($"PlayerExp表中没有找到能升的等级---{level}");
            }
            current_record = t;
            bonus_queue.Push(new UpgradeBonus()
            {
                talent_point = t.f_talent_point,
            });
        }

        public bool HasBonus()
        {
            return bonus_queue.Count != 0;
        }

        public UpgradeBonus DeliverBonus()
        {
            if (bonus_queue.Count != 0)
            {
                var bonus = bonus_queue.Pop();
                bonus.LevelUp();

                return bonus;
            }
            return null;
        }
    }
    public class UpgradeBonus
    {
        public int talent_point;

        public void LevelUp()
        {
            Mission.instance.try_get_mgr("adventure", out var mgr);
            if (mgr != null && mgr is AdventureMgr amgr)
            {
                amgr.player_exp.talent_point += talent_point;
            }
        }
    }
}
