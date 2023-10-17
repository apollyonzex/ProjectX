using Foundation;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace CaravanEnhanced {
    public class CargoMgrView : MonoBehaviour, ICargoMgrView {

        public CargoMgr owner;

        public Transform content;

        public CargoItemView prefab;

        public List<CargoItemView> cargo_items_view = new List<CargoItemView>();

        void IModelView<CargoMgr>.attach(CargoMgr owner) {
            this.owner = owner;
        }

        void IModelView<CargoMgr>.detach(CargoMgr owner) {
            if (owner != null) {
                owner = null;
            }
            Destroy(gameObject);
        }

        void ICargoMgrView.notified_item_add(CargoMgr owner, Item item) {
            var g = Instantiate(prefab, content, false);
            g.init(item, owner);
            g.gameObject.SetActive(true);
            cargo_items_view.Add(g);
        }

        void ICargoMgrView.notified_item_remove(CargoMgr owner, int index) {
            Destroy(cargo_items_view[index].gameObject);
            cargo_items_view.RemoveAt(index);
        }


        void IModelView<CargoMgr>.shift(CargoMgr old_owner, CargoMgr new_owner) {
            
        }
    }
}
