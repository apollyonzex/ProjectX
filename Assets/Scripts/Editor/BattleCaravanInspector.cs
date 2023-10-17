using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worlds;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Enemies;

public class BattleCaravanInspector : EditorWindow
{
    public int id = 0;
    public BattleCaravan caravan;
    public EnemyMgr enemyMgr;

    //================================================================================================


    [MenuItem("EditorWindow/BattleCaravanInspector")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BattleCaravanInspector));
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

        if (caravan == null)
        {
            var mgr = WorldState.instance.mission.battleMgr.caravan_mgr;
            if (mgr == null) return;
            caravan = mgr.caravan;
        }

        if (enemyMgr == null)
            enemyMgr = WorldState.instance.mission.battleMgr.enemyMgr;

        GUILayout.BeginVertical();

        GUILayout.Label($"hp: {caravan.current_hp}");
        GUILayout.Label($"max_hp_limit: {caravan.max_hp_limit}");

        GUILayout.Label("");
        GUILayout.Label($"pos: {caravan.position}");
        GUILayout.Label($"logic_pos: {caravan.logic_position}");
        GUILayout.Label($"logic_dir: {caravan.logic_direction}");

        GUILayout.Label("");
        GUILayout.Label($"v: {caravan.velocity}");
        GUILayout.Label($"driving_acc: {caravan.driving_acc_readonly}");
        GUILayout.Label($"braking_acc: {caravan.braking_acc_readonly}");
        GUILayout.Label($"driving_speed_limit: {caravan.driving_speed_limit_readonly}");

        if (caravan.glide_status == Glidestatus.ready)
            GUILayout.Label($"descend_speed_limit: {caravan.descend_speed_limit_glide_readonly}");
        else
            GUILayout.Label($"descend_speed_limit: {caravan.descend_speed_limit_readonly}");

        GUILayout.Label("");
        GUILayout.Label($"acc_status: {caravan.acc_status}");
        GUILayout.Label($"liftoff_status: {caravan.liftoff_status}");
        GUILayout.Label($"glide_status: {caravan.glide_status}");

        GUILayout.Label("");
        GUILayout.Label($"holding_enemies_count: {enemyMgr.holding_cells.Count}");
        GUILayout.Label($"boarding_enemies_count: {enemyMgr.boarding_cells.Count}");

        GUILayout.EndVertical();
    }


    void OnInspectorUpdate()
    {
        Repaint();
    }
}
