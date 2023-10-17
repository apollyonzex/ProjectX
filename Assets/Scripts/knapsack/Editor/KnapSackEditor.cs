using AutoCode.Tables;
using Common_Formal;
using UnityEditor;
using UnityEngine;

namespace BackPack {
    [CustomEditor(typeof(KnapSackSystem))]
    public class KnapSackEditor : Editor {
        KnapSackSystem system;

        public int index;

        public int row,col;

        private void OnEnable() {
            system = (KnapSackSystem)target;
        }
        private void OnDisable() {
            
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("init")) {
                system.start();
            }

            GUILayout.Space(10f);

            index =  (int)EditorGUILayout.IntField("Area索引",index);
            if(GUILayout.Button("Add Area")) {
                var gameObjects = Selection.gameObjects;
                foreach(var g in gameObjects) {
                    if (g.TryGetComponent<KnapSackCell>(out var cell)) {
                        system.areaList[index].cells.Add(cell);
                    }
                }
            }

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            
            row = (int)EditorGUILayout.IntField("row", row);
            col = (int)EditorGUILayout.IntField("col", col);
            
            EditorGUILayout.EndHorizontal();
            if(GUILayout.Button("Put Item")) {
                TempDevice t = new TempDevice();
                EX_Utility.try_load_table_without_running<Item>("item", "item",out var item);
                if(!item.try_get((uint)system.item_id, out var record)) {
                    Debug.Log("无效的item_id");
                    return;
                }
                t.size = record.f_size;
                t.index =(int)record.f_id;
                bool b = system.try_input_device(row, col, t) ;
                Debug.Log($"put item {b}");
            }
            if(GUILayout.Button("Remove Item")) {
                system.try_remove_device(row, col);
            }

            GUILayout.Space(10f);

            if(GUILayout.Button("Save")) {
                system.save();
            }
        }

    }
}
