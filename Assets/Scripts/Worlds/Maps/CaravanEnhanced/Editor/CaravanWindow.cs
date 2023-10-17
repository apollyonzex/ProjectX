using CaravanEnhanced;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaravanWindow : EditorWindow
{
    public int item_id;

    [MenuItem("EditorWindow/CargoWindow _b")]
    public static void ShowWindow() {
        var window = EditorWindow.GetWindow(typeof(CaravanWindow));
    }

    private void OnGUI() {
        if (!Application.isPlaying) {
            GUILayout.Label("请先运行游戏");
            return;
        }
        
        if(SceneManager.GetActiveScene().name != "World") {
            return;
        }
        GUILayout.BeginVertical();
        GUILayout.Label("Cargo:");
        item_id = EditorGUILayout.IntField(item_id);
        if (GUILayout.Button("添加物体")) {
            if (CaravanFunc.TryMakeItem((uint)item_id) == null) {
                Debug.Log("无效的id输入");
            } else {
                Worlds.WorldState.instance.mission.cargoMgr.AddItem(item_id);
            }
            
        }

        if (GUILayout.Button("添加所有设备"))
        {
            var items = CaravanEnhanced.CaravanFunc.TryGetItemData();
            foreach (var item in items.records)
            {
                Worlds.WorldState.instance.mission.cargoMgr.AddItem((int)item.f_id);
            }
        }

        var count = Worlds.WorldState.instance.mission.cargoMgr.cargo_items.Count;
        for (int i = 0; i < count; i++) {
            var item = Worlds.WorldState.instance.mission.cargoMgr.cargo_items[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{item.name}");
            if (GUILayout.Button("remove")) {
                Worlds.WorldState.instance.mission.cargoMgr.RemoveItem(item);
                count -= 1;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        GUILayout.Label("Caraven:");
        GUILayout.Label($"hp: {Worlds.WorldState.instance.mission.caravanMgr.hp}");
        GUILayout.Label($"{Worlds.WorldState.instance.mission.caravanMgr.body_path}");
        GUILayout.Label($"有{Worlds.WorldState.instance.mission.caravanMgr.slots.Count}个槽位");
        foreach(var slot in Worlds.WorldState.instance.mission.caravanMgr.slots) {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"槽位:{slot.position}");
            if (slot.item != null) {
                GUILayout.Label($"装载物体为:{slot.item.name}");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
