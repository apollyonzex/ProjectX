using Common_Formal;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor;
using World_Formal.Helpers;
using Worlds;
using Worlds.Missions.Dialogs;
using static AutoCode.Tables.Sites;

namespace World_Formal.Map.Encounters
{
    public interface IEncounterView : IModelView<EncounterMgr>
    {

    }


    public class EncounterMgr : Model<EncounterMgr, IEncounterView>, IMissionDialogHandler
    {

        public uint world_id;
        public uint journey_difficult;

        private uint current_difficult;     //当前实际的难度区间,比如有2个难度0,20  此时journey_difficult =15 ,current_difficult = 0;

        public struct Encounter
        {
            public uint[] battle;
            public uint[] rndEvents;

            public Encounter(uint[] rndEvents, uint[] battle)
            {
                this.battle = battle;
                this.rndEvents = rndEvents;
            }
        }


        public struct Encounters
        {
            public Dictionary<(uint, uint), Encounter> encounters;//KV: (world_id,difficult),Encounter

            public Encounters(Dictionary<(uint, uint), Encounter> encounters)
            {
                this.encounters = encounters;
            }
        }
        private rndEvent.rndEventMgr m_rndEventMgr;

        public Encounters encounters => m_encounters;
        private Encounters m_encounters;
        public EncounterMgr(uint world_id, uint journey_difficult)
        {
            this.world_id = world_id;
            this.journey_difficult = journey_difficult;
            m_encounters = new Encounters(new Dictionary<(uint, uint), Encounter>());
            m_encounters = tryGetData_from_db();

            //加载关联类
            m_rndEventMgr = new();
        }


        /// <summary>
        /// 读表获取数据
        /// </summary>
        private Encounters tryGetData_from_db()
        {
            AutoCode.Tables.Contents table = new();

            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "encounter_site", out var asset);

            table.load_from(asset);

            foreach (var rec in table.records)
            {
                var encounter = new Encounter(rec.f_rndEvent, rec.f_battle);
                m_encounters.encounters.Add((rec.f_id, rec.f_journey_difficult), encounter);
            }

            return m_encounters;
        }


        /// <summary>
        /// 激活奇遇
        /// </summary>
        public void active(e_se_Type se_type)
        {
            uint[] ids = null;
            if (se_type == e_se_Type.i_RandomEvent) //随机事件奇遇 - 触发对话
            {
                uint diffcult = 0;
                foreach (var e in encounters.encounters)
                {
                    if (e.Key.Item1 ==  world_id && e.Key.Item2 <= journey_difficult)
                    {
                        diffcult = e.Key.Item2;
                        ids = e.Value.rndEvents;
                    }
                }
                if (diffcult != current_difficult)
                {
                    m_rndEventMgr.reset_rnd_event();
                }

                //进入dialog Flow
                bool bl = m_rndEventMgr.try_get_Dialog(ids, se_type == e_se_Type.i_Battle, out var rndEvent);
                if (!bl) return;

                AssetBundleManager.instance.load_asset<MissionDialogGraphAsset>(rndEvent.dialog.Item1, rndEvent.dialog.Item2, out var asset);
                AssetBundleManager.instance.load_asset<MissionDialogWindow>("dialog", "dialog_window", out var w);
                var win = UnityEngine.GameObject.Instantiate(w, UnityEngine.GameObject.Find("Canvas").transform);
                AssetBundleManager.instance.load_asset<UnityEngine.Sprite>(rndEvent.image.Item1, rndEvent.image.Item2, out var s);//临时
                win.init(this, rndEvent.title.ToString(), s);      //暂时
                asset.graph.entry.invoke(win);
            }

            else if (se_type == e_se_Type.i_Battle) // 规则：战斗奇遇 - 无对话
            {
                uint diffcult = 0;
                foreach (var e in encounters.encounters)
                {
                    if (e.Key.Item1 == world_id && e.Key.Item2 <= journey_difficult)
                    {
                        diffcult = e.Key.Item2;
                        ids = e.Value.battle;
                    }
                }
                if (diffcult != current_difficult)
                {
                    m_rndEventMgr.reset_battle_event();
                }

                Battle_In_Out_Helper.instance.start_battle(null, true);
            }
        }


        void IMissionDialogHandler.end()
        {
        }


        void IMissionDialogHandler.destroy()
        {
        }


        void IMissionDialogHandler.enter_normal_battle(System.Action<bool> callback)
        {
            Battle_In_Out_Helper.instance.start_battle(callback);
        }
    }
}

