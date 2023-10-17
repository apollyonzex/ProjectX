using UnityEngine;
using UnityEngine.EventSystems;
using World_Formal.Caravans.Devices;

namespace World_Formal.CaravanBackPack
{
    public class BackPackCellView : MonoBehaviour, IPointerClickHandler
    {

        public BackPackMgr owner;
        public GameObject lock_sprite;

        public GameObject top, bottom, left, right;

        public int row, col;
        public bool occupied, locked;

        public void init(BackPackMgr owner, int row, int col, bool occupied, bool locked)
        {
            this.owner = owner;
            this.row = row;
            this.col = col;
            this.occupied = occupied;
            this.locked = locked;

            if (locked)
            {
                lock_sprite.SetActive(true);
            }
            else
            {
                lock_sprite.SetActive(false);
            }
        }

        public bool SetItemHere(Device device)
        {
            if (device == null)
                return false;
            return owner.put_device(row, col, device);
        }

        public bool SetUnlocked()
        {
            locked = false;
            lock_sprite.SetActive(false);

            return true;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var helper = owner.helper;
            var holding_device = helper.holding.Item2;
            if (holding_device != null)
            {
                if (owner.put_device(row, col, holding_device))
                {
                    helper.mgr.remove_device(helper.holding.Item1, helper.holding.Item2);
                    helper.no_holding();
                }
            }
        }
    }
}
