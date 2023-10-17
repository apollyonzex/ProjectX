using Foundation;
using System.Collections.Generic;

namespace CaravanEnhanced {

    public interface ICargoMgrView :IModelView<CargoMgr>{
        void notified_item_add(CargoMgr owner, Item item);

        void notified_item_remove(CargoMgr owner, int index);
    }

    public class CargoMgr :Model<CargoMgr,ICargoMgrView>{
        public List<Item> cargo_items = new List<Item>();

        public void AddItem(int id) {

            var t = CaravanFunc.TryMakeItem((uint)id);

            cargo_items.Add(t);

            foreach(var view in views) {
                view.notified_item_add(this, t);
            }
        }

        public void AddItem(Item item) {
            cargo_items.Add(item);
            foreach (var view in views) {
                view.notified_item_add(this, item);
            }
        }



        public void RemoveItem(Item item) {
            var index = cargo_items.IndexOf(item);
            cargo_items.Remove(item);
            foreach(var view in views) {
                view.notified_item_remove(this, index);
            }
        }
    }
}
