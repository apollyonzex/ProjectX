using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CaravanEnhanced {
    [CustomEditor(typeof(CaravanBody))]
    public class CaravanBodyEditor : Editor {

        public CaravanBody owner;

        public void OnEnable() {
            owner = (CaravanBody)target;
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if(GUILayout.Button("add slot")) {
                owner.AddSlot();
            }
        }
    }
}
