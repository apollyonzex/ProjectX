using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardWindow : EditorWindow
{
    public int card_id;

    [MenuItem("EditorWindow/CardWindow/MapCardWinodw")]

    public static void ShowWindow() {
        var window = EditorWindow.GetWindow(typeof(CardWindow));
    }

    private void OnGUI() {
        if (!Application.isPlaying) {
            GUILayout.Label("请先运行游戏");
            return;
        }

        if (SceneManager.GetActiveScene().name != "World") {
            return;
        }
        GUILayout.BeginVertical();

        GUILayout.Label("Deck");
        card_id = EditorGUILayout.IntField(card_id);
        if (GUILayout.Button("添加卡片")) {
            if (Worlds.WorldState.instance.mission.cardMgr.TryGetCardData((uint)card_id)!=null) {
                Worlds.WorldState.instance.mission.cardMgr.AddCard((uint)card_id,null);
            } else {
                Debug.Log("无效的卡牌id输入");
            }
        }

        var count = Worlds.WorldState.instance.mission.cardMgr.cards.Count;
        for(int i = 0; i < count; i++) {
            var card = Worlds.WorldState.instance.mission.cardMgr.cards[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{card.name}");
            if (GUILayout.Button("remove")) {
                Worlds.WorldState.instance.mission.cardMgr.RemoveCard(card.id, null) ;
                count -= 1;
            }
            GUILayout.EndHorizontal();
        }


        GUILayout.EndVertical();
    }
}
