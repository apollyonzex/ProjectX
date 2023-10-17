using UnityEngine;
using System.Collections.Generic;

namespace CaravanEnhanced{
    public class CaravanBody :MonoBehaviour{
        public List<CaravanMakerSlot> slots = new List<CaravanMakerSlot>();

        public void AddSlot() {
            var slot = Instantiate(prefab, transform, false);
            slot.index = slots.Count;
            slot.owner = this;
            slot.gameObject.SetActive(true);
            slots.Add(slot);
        }


        public void RemoveSlot(CaravanMakerSlot slot) {
            slots.Remove(slot);
            for (int i = 0; i < slots.Count; i++) {
                slots[i].index = i;     //重置每个槽的index
            }
        }


        [Header("插槽模板")]
        public CaravanMakerSlot prefab;
    }
}
