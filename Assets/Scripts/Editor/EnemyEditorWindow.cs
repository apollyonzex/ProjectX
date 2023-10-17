
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worlds;
using Worlds.Missions.Battles;

public class EnemyEditorWindow : EditorWindow
{
    public int num = 1;
    public int interval = 60;
    private int current_interval;

    private bool action_dirty = true;

    public bool ret;
    private bool Ret {
        get {
            return ret;
        }
        set {
            if(value!= ret)
                action_dirty = true;
            ret = value;
            current_interval = interval;
        }
    }
    public int id = 1;
    public Vector2 pos;
    public int count = 1;
    public uint u_id => (uint)id;
    public int count_result
    {
        get 
        {
            if (count >= 1) return count;
            return 1;
        }
    }

    [MenuItem("EditorWindow/EnemyWindow _F5")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(EnemyEditorWindow));
    }

    public void _update() {
        if (!Application.isPlaying) {
            return;
        }

        if (SceneManager.GetActiveScene().name != "World") {
            return;
        }


        if (Worlds.WorldState.instance.subState != Worlds.WorldState.SubState.normal_battle) {
            return;
        }

        if (EditorApplication.isPaused == true) {
            return;
        }
        if (Ret) {
            if (current_interval > 0) {
                current_interval--;
                return;
            }
            current_interval = interval + Random.Range(0, interval);
            var mgr = WorldState.instance.mission.battleMgr.enemyMgr;
            var dic = mgr.raw_data;
            var max_num = dic.Count;
            int init_num = Random.Range(0, num + 1);
            int init_id = Random.Range(1, max_num + 1);
            var _id = (uint)init_id;
            if (dic.ContainsKey(_id)) {
                Vector2 pos;
                var pos_x = Random.Range(-10, 10);
                var pos_y = Random.Range(0, 10);
                if (pos_x < 0) {
                    pos = new Vector2(pos_x - 10, pos_y);
                } else {
                    pos = new Vector2(pos_x + 10, pos_y);
                }
                for (int i = 0; i < init_num; i++) {
                    pos = pos + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
                    var index = mgr.add_cell(_id, pos);
                    Debug.Log($"已添加 怪物 {index} , 相对坐标:{pos}");
                }
            }
        }
    }
    private void Update() {
        if (action_dirty) {
            if (Ret) {      //变成了true
                BattleSceneRoot.physics_tick += _update;
            } else {
                BattleSceneRoot.physics_tick -= _update;
            }
            action_dirty = false;
        }
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

        var mgr = WorldState.instance.mission.battleMgr.enemyMgr;

        GUILayout.BeginVertical();
        GUILayout.Label("可添加的敌人");
        var dic = mgr.raw_data;
        foreach (var info in dic)
        {
            var v = info.Value;
            GUILayout.Label($"id:{v.f_id}，类型:{v.f_move_type.value}，血量:{v.f_hp}，速度:{v.f_move_speed}");
        }
        GUILayout.Label("");

        GUILayout.Label("敌人id，代表放入的是哪一种敌人");
        id = EditorGUILayout.IntField(id);
        pos = EditorGUILayout.Vector2Field("相对坐标",pos);

        GUILayout.Label("数量");
        count = EditorGUILayout.IntField(count);
        GUILayout.Label("");

        if (GUILayout.Button("添加敌人"))
        {
            if (dic.ContainsKey(u_id))
            {
                for (int i = 0; i < count_result; i++)
                {
                    BattleSceneRoot.instance.add_enemy_per_random_time(() => {
                        var _id = mgr.add_cell(u_id, pos);
                        Debug.Log($"已添加 怪物 {_id} , 相对坐标:{pos}");
                    });
                } 
            }     
        }
        GUILayout.BeginHorizontal();
        Ret = GUILayout.Toggle(Ret, "自动生成怪物");
        interval = EditorGUILayout.IntField(interval);
        num = EditorGUILayout.IntField(num);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}
