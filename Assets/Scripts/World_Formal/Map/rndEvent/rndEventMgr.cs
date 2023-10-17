using Common_Formal;
using Foundation;
using Foundation.Tables;
using System.Collections.Generic;

namespace World_Formal.Map.rndEvent
{
    public interface IrndEventMgrView : IModelView<rndEventMgr>
    { 
        
    }


    public class rndEventMgr : Model<rndEventMgr, IrndEventMgrView>
    {
        public List<uint> triggered_event = new();

        public List<uint> triggered_battle = new();
        public struct RndEvent {
            public uint id;
            public uint title;
            public (string, string) image;
            public (string,string) dialog;

            public RndEvent(uint id, uint title, (string,string)image, (string,string) dialog) {
                this.id = id;
                this.title = title;
                this.image = image;
                this.dialog = dialog;
            }
        }


        public struct RndEvents {
            public Dictionary<uint, RndEvent> rndEvents;//KV: index,RndEvent

            public RndEvents(Dictionary<uint, RndEvent> rndEvents) {
                this.rndEvents = rndEvents;
            }
        }

        public RndEvents rndEvents => m_rndEvents;
        private RndEvents m_rndEvents;
        public rndEventMgr() {
            m_rndEvents = new RndEvents(new Dictionary<uint, RndEvent>());
            m_rndEvents = tryGetData_from_db();
        }


        /// <summary>
        /// 读表获取数据
        /// </summary>
        private RndEvents tryGetData_from_db() {
            AutoCode.Tables.RndEvents table = new();

            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "rnd_events", out var asset);

            table.load_from(asset);

            foreach (var rec in table.records) {
                var encounter = new RndEvent(rec.f_id, rec.f_title,rec.f_image, rec.f_dialogue);
                m_rndEvents.rndEvents.Add(rec.f_id, encounter);
            }

            return m_rndEvents;
        }

        public void reset_rnd_event() {
            triggered_event.Clear();
        }


        public void reset_battle_event() {
            triggered_battle.Clear();
        }
        /// <summary>
        /// 获取奇遇对应事件
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dialog"></param>
        public bool try_get_Dialog(uint[] ids,bool isbattle, out RndEvent r) {
            r = new();
            if (ids ==null || ids.Length == 0) return false;
            int i = new System.Random().Next(0, ids.Length);//等概率随机
            if (isbattle) {
                while (triggered_battle.Contains(ids[i])) {
                    i++;
                    if (i >= ids.Length) {
                        i = 0;
                    }
                    if (triggered_battle.Count == ids.Length) {
                        UnityEngine.Debug.Log("这个难度的所有战斗都触发过了");
                        triggered_battle.Clear();
                        break;
                    }
                }
                triggered_battle.Add(ids[i]);
            } else {
                while (triggered_event.Contains(ids[i])) {
                    i++;
                    if (i >= ids.Length) {
                        i = 0;
                    }
                    if (triggered_event.Count == ids.Length) {
                        UnityEngine.Debug.Log("这个难度的所有事件都触发过了");
                        triggered_event.Clear();
                        break;
                    }
                }
                triggered_event.Add(ids[i]);
            }

            r = rndEvents.rndEvents[ids[i]];
            if (r.dialog.Item1 == null || r.dialog.Item2 == null) return false;
            return true;
        }

    }
}

