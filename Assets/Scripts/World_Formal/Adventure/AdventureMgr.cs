using Common_Formal;
using Foundation;
using Foundation.Tables;
using System;
using System.Collections.Generic;

namespace World_Formal.Adventure
{

    public interface IAdventureMgr : IModelView<AdventureMgr>
    {
        void AddPlayerExp();
        void DeliverBonus(UpgradeBonus bonus);
    }
    public class AdventureMgr :  Model<AdventureMgr, IAdventureMgr>,IMgr            //冒险等级,设备练度
    {

        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        //==========================================================================

        public AdventureMgr(string name,params object[] objs)
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

            AutoCode.Tables.PlayerExp playerExp = new AutoCode.Tables.PlayerExp();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "player_exp", out var asset);
            playerExp.load_from(asset);
            playerExp.try_get(0, out var t);
            player_exp = new PlayerExp(0,0,t);
        }

        //===========================================================================

        public PlayerExp player_exp;

        public Dictionary<uint, DeviceExp> device_exp = new Dictionary<uint, DeviceExp>();
        /// <summary>
        /// 增加玩家经验
        /// </summary>
        /// <param name="delta"></param>
        public void AddPlayerExp(int delta)
        {
            player_exp.AddExp(delta);

            foreach (var view in views)
            {
                view.AddPlayerExp();        //增加经验的时候 也会更新等级视图
            }
        }
        /// <summary>
        /// 发放玩家升级奖励
        /// </summary>
        public void DeliverRewards()
        {
            while (player_exp.HasBonus())
            {
                var bonus = player_exp.DeliverBonus();

                foreach(var view in views)
                {
                    view.DeliverBonus(bonus);
                }
            }
        }
        /// <summary>
        /// 增加指定id的种类设备经验
        /// </summary>
        public void AddDeviceExp(uint id,int delta)
        {
            device_exp.TryGetValue(id, out var exp);
            if (exp != null)
            {
                exp.AddExp(delta);
            }
            else
            {
                AutoCode.Tables.DeviceExp deviceExp = new AutoCode.Tables.DeviceExp();
                AssetBundleManager.instance.load_asset<BinaryAsset>("db", "device_exp", out var asset);
                deviceExp.load_from(asset);
                deviceExp.try_get(0, out var t);
                device_exp.Add(id, new DeviceExp(0, 0, t));
                AddDeviceExp(id, delta);
            }
        }
        /// <summary>
        ///  查找指定id的种类设备经验
        /// </summary>
        public DeviceExp GetDevice(uint  id)
        {
            device_exp.TryGetValue(id, out var exp);
            if (exp != null)
            {
                return exp;
            }
            else
            {
                AutoCode.Tables.DeviceExp deviceExp = new AutoCode.Tables.DeviceExp();
                AssetBundleManager.instance.load_asset<BinaryAsset>("db", "device_exp", out var asset);
                deviceExp.load_from(asset);
                deviceExp.try_get(0, out var t);
                device_exp.Add(id, new DeviceExp(0, 0, t));
                return GetDevice(id);
            }
        }
    }
}
