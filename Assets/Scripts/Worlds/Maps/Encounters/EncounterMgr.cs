using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using Worlds.Missions.Dialogs;

namespace Worlds.Maps
{
    public struct Encounter
    {
        public uint id;
        public uint[] rndEvents;

        public Encounter(uint id, uint[] rndEvents) {
            this.id = id;
            this.rndEvents = rndEvents;
        }
    }


    public struct Encounters
    {
        public Dictionary<uint, Encounter> encounters;//KV: index,Encounter

        public Encounters(Dictionary<uint, Encounter> encounters)
        {
            this.encounters = encounters;
        }
    }


    public class EncounterMgr: IMissionDialogHandler
    {
        private rndEventMgr m_rndEventMgr;

        public Encounters Encounters => m_encounters;
        private Encounters m_encounters;
        public EncounterMgr() {
            m_encounters = new Encounters(new Dictionary<uint, Encounter>());
            m_encounters = tryGetData_from_db();

            //加载关联类
            m_rndEventMgr = new();
        }


        /// <summary>
        /// 读表获取数据
        /// </summary>
        private Encounters tryGetData_from_db()
        {
            AutoCode.Tables.Encounter table = new();

            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "encounter", out var asset);

            table.load_from(asset);

            foreach (var rec in table.records)
            {
                var encounter = new Encounter(rec.f_id, rec.f_rndEvent);
                m_encounters.encounters.Add(rec.f_id ,encounter);
            }

            return m_encounters;
        }


        /// <summary>
        /// 激活奇遇
        /// </summary>
        public void active(uint[] ids)
        {
            if (ids.Length == 0) return;

            int i = new System.Random().Next(0, ids.Length);//等概率随机
            var id = ids[i];
            Encounter e = Encounters.encounters[id];
            ids = e.rndEvents;

            bool bl = m_rndEventMgr.try_get_Dialog(ids, out var rndEvent);
            if (!bl) return;

            //进入dialog Flow
            AssetBundleManager.instance.load_asset<MissionDialogGraphAsset>("dialog", rndEvent.dialog, out var asset);
            AssetBundleManager.instance.load_asset<MissionDialogWindow>("dialog", "dialog_window",out var w);
            var win = UnityEngine.GameObject.Instantiate(w,UnityEngine.GameObject.Find("Canvas").transform);        //临时
            win.init(this, rndEvent.title);
            asset.graph.entry.invoke(win);
        }



        void IMissionDialogHandler.end()
        {
        }

        void IMissionDialogHandler.destroy()
        {
        }

        void IMissionDialogHandler.enter_normal_battle(System.Action<bool> callback)
        {
            WorldState.instance.mission.start_battle("normal_battle", callback);
        }
    }

}

