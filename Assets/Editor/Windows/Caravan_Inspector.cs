using UnityEditor;
using UnityEngine;
using World_Formal;
using World_Formal.Helpers;

namespace Editor.Windows
{
    public class Caravan_Inspector : EditorWindow
    {
        public WorldContext ctx;
        public float speed_max_mod = 0;
        public float acc_driving_mod = 0;

        //================================================================================================

        [MenuItem("EditorWindow/Caravan_Inspector _F4")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Caravan_Inspector));
        }


        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            if (WorldSceneRoot.instance == null)
            {
                GUILayout.Label("请先进入world");
                return;
            }

            ctx ??= WorldContext.instance;

            GUILayout.BeginVertical();
            GUILayout.Label("【可调整】");
            GUILayout.Label($"速度上限修正");
            {
                speed_max_mod = EditorGUILayout.FloatField(speed_max_mod);
                ctx.speed_max_mod = this.speed_max_mod;
            }
            GUILayout.Label($"加速度修正");
            {
                acc_driving_mod = EditorGUILayout.FloatField(acc_driving_mod);
                ctx.acc_driving_mod = this.acc_driving_mod;
            }

            GUILayout.Label("");
            if (GUILayout.Button("执行跳跃"))
            {
                Caravan_Move_Helper.instance.jump(ctx, 10, false);
            }

            GUILayout.Label("");
            GUILayout.Label("【基础参数】");
            GUILayout.Label($"位置: {ctx.caravan_pos}");
            GUILayout.Label($"速度: {ctx.caravan_velocity}");

            GUILayout.Label("");
            GUILayout.Label("【状态】");
            GUILayout.Label($"运动状态: {ctx.caravan_move_status}");
            GUILayout.Label($"加速状态: {ctx.caravan_acc_status}");
            GUILayout.Label($"滞空状态: {ctx.caravan_liftoff_status}");
            GUILayout.Label($"动画状态: {ctx.caravan_anim_status}");
            GUILayout.EndVertical();
        }


        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}


