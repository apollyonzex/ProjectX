using UnityEngine;
using UnityEditor;
using Common_Formal;
using World_Formal.Adventure;

public class DeviceExpEditorWindow : EditorWindow
{
    public uint device_id;
    public int exp;

    [MenuItem("EditorWindow/DeviceExpWindow")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(DeviceExpEditorWindow));
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

        device_id = (uint)EditorGUILayout.IntField((int)device_id);
        exp = EditorGUILayout.IntField(exp);
        if (GUILayout.Button("add exp"))
        {
            if (mgr is AdventureMgr amgr)
            {
                amgr.AddDeviceExp(device_id, exp);
            }
        }
    }
}
