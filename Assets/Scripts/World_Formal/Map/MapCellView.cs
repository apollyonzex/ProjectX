using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Map {
    public class MapCellView : MonoBehaviour {

        public MapView mgrView;
        public SpriteRenderer indicator_Pic;
        public LineRenderer road_Model;

        internal int index;
        internal Site site;
        internal MapMgr owner;
        private bool can_Opr = false;//是否可操作

        public  void init(MapMgr owner) {
            this.owner = owner;

            active_site(index == owner.Sites.start_index, Color.green, E_Site_State.Self);
            gameObject.SetActive(true);
        }

        public void active_site(bool b, Color cl, E_Site_State state) {
            indicator_Pic.color = cl;
            indicator_Pic.gameObject.SetActive(b);
            can_Opr = b;

            if (state == E_Site_State.Attachable) can_Opr = true;

            if (state == E_Site_State.Self) can_Opr = false;
        }
    }
}
