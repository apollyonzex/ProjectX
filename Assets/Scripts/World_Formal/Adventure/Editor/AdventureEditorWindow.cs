using UnityEngine;
using UnityEditor;
using Common_Formal;
using World_Formal.Adventure;

public class AdventureEditorWindow : EditorWindow
{

    public int exp;

    [MenuItem("EditorWindow/AdventureWindow")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(AdventureEditorWindow));
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("请先运行游戏");
            return;
        }
        if (Mission.instance == null)
            return;
        Mission.instance.try_get_mgr("adventure", out var mgr);
        if (mgr == null)
            return;

        exp = EditorGUILayout.IntField(exp);
        if(GUILayout.Button("add exp"))
        {
            if (mgr is AdventureMgr amgr)
            {
                amgr.AddPlayerExp(exp);
            }
        }
        if(GUILayout.Button("deliver bonus"))
        {
            if (mgr is AdventureMgr amgr)
            {
                amgr.DeliverRewards();
            }
        }
    }
}
