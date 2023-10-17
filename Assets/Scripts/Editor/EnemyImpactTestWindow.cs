using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worlds;


public class EnemyImpactTestWindow : EditorWindow
{
    public float dis;
    public Vector2 dir;

    //================================================================================================


    [MenuItem("EditorWindow/EnemyImpactTestWindow _F7")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EnemyImpactTestWindow));
    }


    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("请先运行游戏");
            return;
        }

        if (SceneManager.GetActiveScene().name != "World")
        {
            return;
        }

        if (Worlds.WorldState.instance.subState != Worlds.WorldState.SubState.normal_battle)
        {
            GUILayout.Label("请先进入战斗");
            return;
        }

        GUILayout.BeginVertical();

        GUILayout.Label("对怪物施加击退");

        GUILayout.Label("击退力度");
        dis = EditorGUILayout.FloatField(dis);
        dir = EditorGUILayout.Vector2Field("击退方向", dir);
        GUILayout.Label("");

        GUILayout.Label("视野内的怪物");
        var mgr = WorldState.instance.mission.battleMgr.enemyMgr;
        var dic = mgr.enemies;
        foreach (var info in dic)
        {
            if (info.Value == null) continue;

            var e = info.Key;
            GUILayout.Label($"id: {e._id} , 类型: {e._type}");

            if (GUILayout.Button("击退"))
            {
                e.be_impacted(dis, dir);
                Debug.Log($"怪物 {e._id} , 被击退, 距离 {dis}, 方向 {dir}");
            }
        }
        GUILayout.Label("");

        GUILayout.EndVertical();
    }


    void OnInspectorUpdate()
    {
        Repaint();
    }

}




     

