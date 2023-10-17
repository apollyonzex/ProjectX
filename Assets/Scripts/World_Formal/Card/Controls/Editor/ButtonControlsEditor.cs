using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace World_Formal.Card.Controls
{

    [CustomEditor(typeof(ButtonControls))]
    public class ButtonControlsEditor  :  Editor
    {
        ButtonControls self;

        bool pressb,releaseb;

        List<int> press_action_indexs = new List<int>();

        List<int> release_action_indexs = new List<int>();

        private string[] attributeNames;


        public void OnEnable()
        {
            self = (ButtonControls)target;
            GetMethod();
            press_action_indexs = self.press_action_indexs;
            release_action_indexs = self.release_action_indexs;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            #region presss  action
            pressb = EditorGUILayout.BeginFoldoutHeaderGroup(pressb,"PressAction");
            if (pressb)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < press_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    press_action_indexs[i] = EditorGUILayout.Popup("  press", press_action_indexs[i], attributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        press_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    press_action_indexs.Add(0);
                }
                
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var press_index_list = serializedObject.FindProperty("press_action_indexs");
            press_index_list.ClearArray();
            for(int i= 0;i < press_action_indexs.Count; i++){
                press_index_list.InsertArrayElementAtIndex(i);
                press_index_list.GetArrayElementAtIndex(i).intValue = press_action_indexs[i];
            }

            var press_names_list = serializedObject.FindProperty("press_action_names");
            press_names_list.ClearArray();
            for (int i = 0; i < press_action_indexs.Count; i++)
            {
                press_names_list.InsertArrayElementAtIndex(i);
                press_names_list.GetArrayElementAtIndex(i).stringValue = attributeNames[press_action_indexs[i]];
            }
            #endregion

            #region release action

            releaseb = EditorGUILayout.BeginFoldoutHeaderGroup(releaseb, "ReleaseAction");
            if (releaseb)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < release_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    release_action_indexs[i] = EditorGUILayout.Popup("  release", release_action_indexs[i], attributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        release_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    release_action_indexs.Add(0);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var release_index_list = serializedObject.FindProperty("release_action_indexs");
            release_index_list.ClearArray();
            for (int i = 0; i < release_action_indexs.Count; i++)
            {
                release_index_list.InsertArrayElementAtIndex(i);
                release_index_list.GetArrayElementAtIndex(i).intValue = release_action_indexs[i];
            }

            var release_names_list = serializedObject.FindProperty("release_action_names");
            release_names_list.ClearArray();
            for (int i = 0; i < release_action_indexs.Count; i++)
            {
                release_names_list.InsertArrayElementAtIndex(i);
                release_names_list.GetArrayElementAtIndex(i).stringValue = attributeNames[release_action_indexs[i]];
            }
            #endregion

            serializedObject.ApplyModifiedProperties();

        }
        private void GetMethod()
        {
            Type controls_type = typeof(Controls_Utility);
            MethodInfo[] methods = controls_type.GetMethods(BindingFlags.Public|BindingFlags.Static);

            List<string> methodlist = new List<string>();

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                ParameterInfo[] parameters = method.GetParameters();
                if(parameters.Length == 0 && returnType.Equals(typeof(void)))
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
