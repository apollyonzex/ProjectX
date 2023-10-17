using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BackPack {
    [ExecuteInEditMode]
    public class KnapSackCell : MonoBehaviour{
        public int row, col;
        public bool occupied = false;
        public bool locked = true;

        public Color item_color = Color.white;
        public bool CanUse() {
            return !occupied && !locked; //没被占用也没被锁
        }


        public void Update() {
            if (locked) {
                GetComponent<Image>().color = Color.black;
            } else if(occupied){
                GetComponent<Image>().color = item_color;
            } else {
                GetComponent<Image>().color = Color.white;
            }
        }
    }
}
