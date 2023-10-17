using Foundation;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.Caravans;

namespace World_Formal.Enviroment
{
    public class EnviromentCaravanView : MonoBehaviour{

        CaravanMgr_Formal owner;

        public EnviromentSlotView prefab_slot;

        public List<EnviromentSlotView> slots = new();

        public Transform anm;

        //void ICaravanView.notified_cancel_highlight(CaravanMgr_Formal owner) {

        //}

        //void ICaravanView.notified_highlight_slot(CaravanMgr_Formal owner, Item item) {

        //}

        //void ICaravanView.notified_item_install(CaravanMgr_Formal owner, Slot slot, Item item) {
        //    foreach(var _slot in slots) {
        //        if(_slot.data == slot) {
        //            _slot.SetItem(item);
        //            return;
        //        }
        //    }
        //}

        //void ICaravanView.notified_item_remove(CaravanMgr_Formal owner, Slot slot) {
        //    foreach (var _slot in slots) {
        //        if (_slot.data == slot) {
        //            _slot.SetItem(null);
        //            return;
        //        }
        //    }
        //}

        
        private void init() {
            //foreach (var slot in owner.slots) {
            //    var g = Instantiate(prefab_slot, transform, false);
            //    g.init(owner, slot);
            //    g.gameObject.SetActive(true);
            //    slots.Add(g);
            //    owner.slots_dic.Add(slot.bone_name, slot);
            //}

            //var bfs = anm.transform.GetComponentsInChildren<BoneFollower>();
            //foreach (var bf in bfs)
            //{
            //    owner.bones_dic.Add(bf.boneName, bf.bone);
            //}

            //var bone_pos_dic = owner.bone_pos_dic;
            //bone_pos_dic.Clear();
            //foreach (var e in bfs)
            //{
            //    bone_pos_dic.Add(e.boneName, e.transform.localPosition);
            //}
        }
    }
}
