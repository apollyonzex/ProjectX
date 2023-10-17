using System;
using System.Collections.Generic;
using Common_Formal;
using Foundation;
using Foundation.Tables;

namespace World_Formal.Adventure
{
    public class DeviceExp : Exp
    {
        public AutoCode.Tables.DeviceExp.Record current_record;
        public DeviceExp(uint init_level, int init_exp, AutoCode.Tables.DeviceExp.Record current_record) : base(init_level, init_exp)
        {
            this.current_record = current_record;
        }

        public override void AddExp(int delta)
        {
            exp += delta;
            while (exp > current_record.f_exp && level < Common.Config.current.max_device_level)
            {
                LevelUp();
            }
        }

        protected override void LevelUp()
        {
            exp -= current_record.f_exp;
            ++m_level;
            AutoCode.Tables.DeviceExp device_exp = new AutoCode.Tables.DeviceExp();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "device_exp", out var asset);
            device_exp.load_from(asset);
            device_exp.try_get(level, out var t);
            if (t == null)
            {
                UnityEngine.Debug.LogWarning($"DeviceExp表中没有找到能升的等级---{level}");
            }
            current_record = t;

            Mission.instance.try_get_mgr("adventure", out var mgr);
            if(mgr is AdventureMgr amgr)
            {
                amgr.AddPlayerExp(t.f_reward_player_exp);
            }
        }
    }
}
