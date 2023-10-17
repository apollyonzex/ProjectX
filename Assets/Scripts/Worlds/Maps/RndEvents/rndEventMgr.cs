using Foundation;
using Foundation.Tables;
using System.Collections.Generic;

namespace Worlds.Maps
{
    public struct RndEvent
    {
        public uint id;
        public string title;
        public string dialog;

        public RndEvent(uint id, string title, string dialog)
        {
            this.id = id;
            this.title = title;
            this.dialog = dialog;
        }
    }


    public struct RndEvents
    {
        public Dictionary<uint, RndEvent> rndEvents;//KV: index,RndEvent

        public RndEvents(Dictionary<uint, RndEvent> rndEvents)
        {
            this.rndEvents = rndEvents;
        }
    }


    public class rndEventMgr
    {
        public RndEvents RndEvents => m_rndEvents;
        private RndEvents m_rndEvents;
        public rndEventMgr()
        {
            m_rndEvents = new RndEvents(new Dictionary<uint, RndEvent>());
            m_rndEvents = tryGetData_from_db();
        }


        /// <summary>
        /// 读表获取数据
        /// </summary>
        private RndEvents tryGetData_from_db()
        {
            AutoCode.Tables.RndEvent table = new();

            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "rnd_event", out var asset);

            table.load_from(asset);

            foreach (var rec in table.records)
            {
                var encounter = new RndEvent(rec.f_id, rec.f_title, rec.f_dialog);
                m_rndEvents.rndEvents.Add(rec.f_id, encounter);
            }

            return m_rndEvents;
        }


        /// <summary>
        /// 获取奇遇对应事件
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dialog"></param>
        public bool try_get_Dialog(uint[] ids, out RndEvent r)
        {
            r = new();
            if (ids.Length == 0) return false;
            int i = new System.Random().Next(0, ids.Length);//等概率随机
            var id = ids[i];

            r = RndEvents.rndEvents[id];
            if (r.dialog.Length == 0) return false;

            return true;
        }

    }
}


