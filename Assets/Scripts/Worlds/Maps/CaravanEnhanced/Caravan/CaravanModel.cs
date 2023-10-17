using Foundation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaravanEnhanced {
    public class CaravanModel : MonoBehaviour{

        public Camera modelCamera;

        public GameObject body;

        public CaravanModelSlot slot_prefab;

        public List<CaravanModelSlot> slots = new List<CaravanModelSlot>();

        public GameObject hovered_item ;

        public void init(CaravanMgr owner) {
            AssetBundleManager.instance.load_asset<Sprite>("caravan", owner.body_path, out var asset);
            body.GetComponent<SpriteRenderer>().sprite = asset;
            body.GetComponent<BoxCollider2D>().size = owner.colider_size;
            foreach (var slot in owner.slots) {
                var g = Instantiate(slot_prefab, body.transform, false);
                g.init(owner, slot);
                g.gameObject.SetActive(true);
                slots.Add(g);
            }
        }


        public void InstallItem(Slot slot,Item item) {
            foreach(var model_slot in slots) {
                if (model_slot.data == slot) {
                    model_slot.SetItem(item);
                    return;
                }
            }
        }

        public void RemoveItem(Slot slot) {
            foreach (var model_slot in slots) {
                if (model_slot.data == slot) {
                    model_slot.SetItem(null);
                    return;
                }
            }
        }

        public void highlight(Item item) {
            foreach(var model_slot in slots) {
                if (item.item_paths.ContainsKey(model_slot.data.type)) {
                    model_slot.highlight();
                }
            }
        }

        public void cancelhighlight() {
            foreach (var model_slot in slots) {
                model_slot.cancel_highlight();
            }
        }


        //拓展：
        public void add_cell(CaravanMgr owner, Slot slot)
        {
            AssetBundleManager.instance.load_asset<Sprite>("caravan", owner.body_path, out var asset);
            body.GetComponent<SpriteRenderer>().sprite = asset;
            body.GetComponent<BoxCollider2D>().size = owner.colider_size;

            var g = Instantiate(slot_prefab, body.transform, false);
            g.init(owner, slot);
            g.gameObject.SetActive(true);
            slots.Add(g);
        }
    }
}
