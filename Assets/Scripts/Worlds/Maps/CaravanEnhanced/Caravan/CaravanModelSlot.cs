

using Foundation;
using System;
using UnityEngine;

namespace CaravanEnhanced{
    public class CaravanModelSlot : MonoBehaviour {

        public Transform node;

        public Slot data;

        public CaravanMgr owner;

        public GameObject item_gameObject;

        public void init(CaravanMgr owner, Slot data) {
            this.owner = owner;
            this.data = data;

            transform.localPosition = data.position;
            transform.localRotation = data.rotation;
            if (data.item != null) {
                AssetBundleManager.instance.load_asset<GameObject>(data.item.item_paths[data.type].Item1, data.item.item_paths[data.type].Item2, out var prefab_asset);

                if(prefab_asset == null) {
                    Debug.LogWarning($"{data.item.name} 的 prefab_path配置有问题");
                    return;
                }

                item_gameObject = Instantiate(prefab_asset, node, false);
            }
        }

        public void SetItem(Item item) {
            if(item == null) {
                Destroy(item_gameObject);
                return;
            }
            AssetBundleManager.instance.load_asset<GameObject>(data.item.item_paths[data.type].Item1, data.item.item_paths[data.type].Item2, out var prefab_asset);
            item_gameObject = Instantiate(prefab_asset, node, false);
        }

        public void highlight() {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }

        public void cancel_highlight() {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
