using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CaravanEnhanced {

    [CustomEditor(typeof(CaravanEditorRoot))]
    public class CaravabEditorRootEditor : Editor {

        public CaravanEditorRoot owner;
        private void OnEnable() {
            owner = (CaravanEditorRoot)target;
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if(GUILayout.Button("new body")) {
                owner.new_body();
            }
            if (GUILayout.Button("save")) {
                owner.save();
            }
        }
    }
}
