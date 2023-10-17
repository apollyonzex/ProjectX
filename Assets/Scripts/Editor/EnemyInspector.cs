using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worlds;
using Worlds.Missions.Battles.Enemies;


public class EnemyInspector : EditorWindow
{
    public int id = 0;

    //================================================================================================


    [MenuItem("EditorWindow/EnemyInspector _F6")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EnemyInspector));
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

        GUILayout.Label("敌人name");
        id = EditorGUILayout.IntField(id);

        var mgr = WorldState.instance.mission.battleMgr.enemyMgr;
        if (!mgr.try_get_cell_from_id(id, out var cell))
            return;

        var brain = cell.brain;

        GUILayout.BeginVertical();

        GUILayout.Label("");
        GUILayout.Label($"hp: {cell.current_hp}");
        GUILayout.Label($"is_dead: {cell.is_dead}");

        GUILayout.Label("");
        GUILayout.Label($"pos: {cell.position}");

        GUILayout.Label("");
        GUILayout.Label($"v: {cell.velocity}");
        GUILayout.Label($"active_v: {cell.concrete.active_v}");
        GUILayout.Label($"impact_v: {cell.concrete.impact_v}");
        GUILayout.Label($"environmet_v: {cell.concrete.environment_v}");

        GUILayout.Label("");
        GUILayout.Label($"main_state: {brain.main_state}");
        GUILayout.Label($"battle_state: {brain.battle_state}");
        GUILayout.Label($"moving_state: {brain.moving_state}");

        if (cell.concrete is Ground_E g)
        {
            GUILayout.Label("");
            GUILayout.Label($"is_liftoff: {g.is_liftoff}");
        }

        GUILayout.EndVertical();
    }


    void OnInspectorUpdate()
    {
        Repaint();
    }
}
