using CaravanEnhanced;
using Foundation;
using UnityEngine;
using World_Formal.Caravans;

namespace World_Formal.Enviroment
{
    public class EnviromentSlotView : MonoBehaviour {

        public Transform node;

        public Slot data;

        public CaravanMgr_Formal owner;

        public GameObject item_gameObject;
        public void init(CaravanMgr_Formal owner, Slot slot) {
            this.owner = owner;
            this.data = slot;

            transform.localPosition = slot.position;
            transform.localRotation = slot.rotation;
            if (data.item != null) {
                AssetBundleManager.instance.load_asset<GameObject>(slot.item.item_paths[slot.type].Item1, slot.item.item_paths[slot.type].Item2, out var prefab_asset);

                if (prefab_asset == null) {
                    Debug.LogWarning($"{slot.item.name} 的 prefab_path配置有问题");
                    return;
                }

                item_gameObject = Instantiate(prefab_asset, node, false);
            }
        }

        public void SetItem(Item item) {
            if (item == null) {
                Destroy(item_gameObject);
                return;
            }
            AssetBundleManager.instance.load_asset<GameObject>(data.item.item_paths[data.type].Item1, data.item.item_paths[data.type].Item2, out var prefab_asset);
            item_gameObject = Instantiate(prefab_asset, node, false);
        }
    }
}
