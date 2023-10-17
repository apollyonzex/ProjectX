using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace World_Formal.Card.Controls
{
    [CustomEditor(typeof(TimerControls))]
    public class TimerControlsEditor :Editor
    {
        TimerControls self;

        bool timerb;

        public  List<int> timer_action_indexs;

        private string[] attributeNames;

        private void OnEnable()
        {
            self = (TimerControls)target;
            GetMethod();
            timer_action_indexs = self.timer_action_indexs;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            timerb = EditorGUILayout.BeginFoldoutHeaderGroup(timerb,"TimerAction");
            if (timerb)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < timer_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    timer_action_indexs[i] = EditorGUILayout.Popup("  timer", timer_action_indexs[i], attributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        timer_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    timer_action_indexs.Add(0);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var timer_index_list = serializedObject.FindProperty("timer_action_indexs");
            timer_index_list.ClearArray();
            for (int i = 0; i < timer_action_indexs.Count; i++)
            {
                timer_index_list.InsertArrayElementAtIndex(i);
                timer_index_list.GetArrayElementAtIndex(i).intValue = timer_action_indexs[i];
            }

            var timer_names_list = serializedObject.FindProperty("timer_action_names");
            timer_names_list.ClearArray();
            for (int i = 0; i < timer_action_indexs.Count; i++)
            {
                timer_names_list.InsertArrayElementAtIndex(i);
                timer_names_list.GetArrayElementAtIndex(i).stringValue = attributeNames[timer_action_indexs[i]];
            }



            serializedObject.ApplyModifiedProperties();
        }

        private void GetMethod()
        {
            Type controls_type = typeof(Controls_Utility);
            MethodInfo[] methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            List<string> methodlist = new List<string>();

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0 && returnType.Equals(typeof(void)))
                {
                    var method_name = method.GetCustomAttribute(typeof(ControlsAction));
                    if (method_name != null)
                    {
                        methodlist.Add((method_name as ControlsAction).action_name);
                    }
                    else
                    {
                        methodlist.Add(method.Name);
                    }
                }
            }

            attributeNames = methodlist.ToArray();
        }
    }
}
