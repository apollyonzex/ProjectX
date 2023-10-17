using Foundation;
using System.Collections.Generic;
using UnityEngine;

namespace CaravanEnhanced {

    public interface ICaravanMgrView : IModelView<CaravanMgr>{
        void initialization(CaravanMgr owner);

        void notified_item_install(CaravanMgr owner, Slot slot, Item item);

        void notified_item_remove(CaravanMgr owner, Slot slot);

        void notified_highlight_slot(CaravanMgr owner, Item item);

        void notified_cancel_highlight(CaravanMgr owner);
    }
    
    public class CaravanMgr :Model<CaravanMgr,ICaravanMgrView>{

        public Slot hovered_slot ;

        public struct BodyData {
            public int hp;
            public int driving_speed_limit;
            public int driving_acc;
            public int braking_acc;
            public int descend_speed_limit;
        }


        public Vector2 colider_size;
        public int current_hp;
        public int hp;
        public int driving_speed_limit;
        public int driving_acc;
        public int braking_acc;
        public int descend_speed_limit;
        public string body_path;
        public string body_prefab_path;

        public BodyData body_data;
        public List<Slot> slots = new List<Slot>();

        public CaravanModel model;

        private void add_item_property(Item item) {
            current_hp += item.current_hp;
            hp += item.hp;
            driving_speed_limit += item.driving_speed_limit;
            driving_acc += item.driving_acc;
            braking_acc += item.braking_acc;
            descend_speed_limit += item.descend_speed_limit;
        }

        private void remove_item_property(Item item) {
            current_hp -= item.current_hp;
            hp -= item.hp;
            driving_speed_limit -= item.driving_speed_limit;
            driving_acc -= item.driving_acc;
            braking_acc -= item.braking_acc;
            descend_speed_limit -= item.descend_speed_limit;
        }
        public void init(int id) {
            var record = CaravanFunc.TryGetBodyData((uint)id);
            current_hp = record.f_hp;
            hp = record.f_hp;
            driving_speed_limit = record.f_driving_speed_limit;
            driving_acc = record.f_driving_acc;
            braking_acc = record.f_braking_acc;
            descend_speed_limit = record.f_descend_speed_limit;
            body_data = new BodyData {
                hp = record.f_hp,
                driving_speed_limit = record.f_driving_speed_limit,
                driving_acc = record.f_driving_acc,
                braking_acc = record.f_braking_acc,
                descend_speed_limit = record.f_descend_speed_limit,
            };
            AssetBundleManager.instance.load_asset<CaravanData>("caravan", record.f_prefab, out var asset);
            body_path = asset.body_path;
            body_prefab_path = asset.body_prefab_path;
            colider_size = asset.size;
            foreach(var body_card_id in record.f_cards) {
                Worlds.WorldState.instance.mission.cardMgr.AddCard(body_card_id,null);
            }

            foreach (var slotdata in asset.slots) {
                Slot slot = new Slot {
                    position = slotdata.position,
                    rotation = slotdata.rotation,
        //            type = slotdata.type,
                    bone_name = slotdata.bone_name
                };
                var item = CaravanFunc.TryMakeItem(slotdata.item_id);
                slot.item = item;
                if (slot.item != null) {
                    slot.item.owner = slot; 
                    add_item_property(item);
                }
                var rec = CaravanFunc.TryGetItemData(slotdata.item_id);
                if (rec != null) {
                    foreach (var card_id in rec.f_cards) {
                        Worlds.WorldState.instance.mission.cardMgr.AddCard(card_id,slot.item);
                    }
                }
                slots.Add(slot);
            }

            foreach (var view in views) {
                view.initialization(this);
            }
        }



        public bool InstallItem(Slot slot,Item item,out Item origin_item) {

            if (!item.item_paths.ContainsKey(slot.type)) {
                origin_item = null;
                return false;
            }
            var index = slots.IndexOf(slot);

            origin_item = slots[index].item;        //获取原本装在槽位的物体

            slots[index].item = item;
            slots[index].item.owner = slots[index];     //修改新装物体的槽位信息

            add_item_property(item);
            var rec = CaravanFunc.TryGetItemData(item.id);
            foreach (var card_id in rec.f_cards) {
                Worlds.WorldState.instance.mission.cardMgr.AddCard(card_id, slots[index].item);         //need update
            }
            if (origin_item != null) {
                remove_item_property(origin_item);
                var r = CaravanFunc.TryGetItemData(origin_item.id);
                foreach (var card_id in r.f_cards) {
                    Worlds.WorldState.instance.mission.cardMgr.RemoveCard(card_id,origin_item);                     //need update
                }

                origin_item.owner = null;       //原有item的槽位设置为null

                foreach(var view in views) {
                    view.notified_item_remove(this,slot);
                }
            }

            foreach (var view in views) {
                view.notified_item_install(this,slot,item);
            }

            return true;
        }



        public void RemoveItem(Slot slot,out Item origin_item) {

            if(slot.type == AutoCode.Tables.Item.e_slotType.i_车轮) {
                Debug.Log("轮胎不允许移除");
                origin_item =null;
                return;
            }


            var index = slots.IndexOf(slot);

            origin_item = slots[index].item;        //获取原本装在槽位的物体
            


            slots[index].item = null;
            var rec = CaravanFunc.TryGetItemData(origin_item.id);
            foreach (var card_id in rec.f_cards) {
                Worlds.WorldState.instance.mission.cardMgr.RemoveCard(card_id,origin_item);
            }
            if (origin_item != null) {
                remove_item_property(origin_item);
            }
            foreach (var view in views) {
                view.notified_item_remove(this, slot);
            }
        }

        public void HighLightSlot(Item item) {
            foreach (var view in views) {
                view.notified_highlight_slot(this, item);
            }
        }

        public void CancelHighLight() {
            foreach (var view in views) {
                view.notified_cancel_highlight(this);
            }
        }


        //拓展：
        public void add_slot(Vector3 pos, Quaternion rotation, AutoCode.Tables.Item.e_slotType type, Item item)
        {
            Slot slot = new()
            {
                position = pos,
                rotation = rotation,
                type = type,
            };

            slots.Add(slot);
            model.add_cell(this, slot);
            
            InstallItem(slot, item, out var ori);
        }


    }
}
