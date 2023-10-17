using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace World_Formal.Card.Controls
{
    [CustomEditor(typeof(LeverControls))]
    public class LeverControlsEditor  : Editor
    {
        LeverControls self;

        bool changeb, zerob, oneb, valueb;

        List<int> change_action_indexs;

        List<int> value_action_indexs;

        List<int> zero_action_indexs;

        List<int> one_action_indexs;

        private string[] FloatAttributeNames;

        private string[] NullAttributeNames;

        public void OnEnable()
        {
            self = (LeverControls)target;
            GetMethod();
            change_action_indexs = self.change_action_indexs;
            value_action_indexs = self.value_action_indexs;
            zero_action_indexs = self.zero_action_indexs;
            one_action_indexs = self.one_action_indexs;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            #region change_action
            changeb = EditorGUILayout.BeginFoldoutHeaderGroup(changeb, "changeAction");
            if (changeb)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < change_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    change_action_indexs[i] = EditorGUILayout.Popup("  change", change_action_indexs[i], NullAttributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        change_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    change_action_indexs.Add(0);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var change_index_list = serializedObject.FindProperty("change_action_indexs");
            change_index_list.ClearArray();
            for (int i = 0; i < change_action_indexs.Count; i++)
            {
                change_index_list.InsertArrayElementAtIndex(i);
                change_index_list.GetArrayElementAtIndex(i).intValue = change_action_indexs[i];
            }

            var change_names_list = serializedObject.FindProperty("change_action_names");
            change_names_list.ClearArray();
            for (int i = 0; i < change_action_indexs.Count; i++)
            {
                change_names_list.InsertArrayElementAtIndex(i);
                change_names_list.GetArrayElementAtIndex(i).stringValue = FloatAttributeNames[change_action_indexs[i]];
            }
            #endregion

            #region value_action
            valueb = EditorGUILayout.BeginFoldoutHeaderGroup(valueb, "valueAction");
            if (valueb)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < value_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    value_action_indexs[i] = EditorGUILayout.Popup("  value", value_action_indexs[i], FloatAttributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        value_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    value_action_indexs.Add(0);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var value_index_list = serializedObject.FindProperty("value_action_indexs");
            value_index_list.ClearArray();
            for (int i = 0; i < value_action_indexs.Count; i++)
            {
                value_index_list.InsertArrayElementAtIndex(i);
                value_index_list.GetArrayElementAtIndex(i).intValue = value_action_indexs[i];
            }

            var value_names_list = serializedObject.FindProperty("value_action_names");
            value_names_list.ClearArray();
            for (int i = 0; i < value_action_indexs.Count; i++)
            {
                value_names_list.InsertArrayElementAtIndex(i);
                value_names_list.GetArrayElementAtIndex(i).stringValue = FloatAttributeNames[value_action_indexs[i]];
            }
            #endregion

            #region one_action
            oneb = EditorGUILayout.BeginFoldoutHeaderGroup(oneb, "oneAction");
            if (oneb)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < one_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    one_action_indexs[i] = EditorGUILayout.Popup("  one", one_action_indexs[i], NullAttributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        one_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    one_action_indexs.Add(0);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var one_index_list = serializedObject.FindProperty("one_action_indexs");
            one_index_list.ClearArray();
            for (int i = 0; i < one_action_indexs.Count; i++)
            {
                one_index_list.InsertArrayElementAtIndex(i);
                one_index_list.GetArrayElementAtIndex(i).intValue = one_action_indexs[i];
            }

            var one_names_list = serializedObject.FindProperty("one_action_names");
            one_names_list.ClearArray();
            for (int i = 0; i < one_action_indexs.Count; i++)
            {
                one_names_list.InsertArrayElementAtIndex(i);
                one_names_list.GetArrayElementAtIndex(i).stringValue = NullAttributeNames[one_action_indexs[i]];
            }
            #endregion

            #region zero_action
            zerob = EditorGUILayout.BeginFoldoutHeaderGroup(zerob, "zeroAction");
            if (zerob)
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < zero_action_indexs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    zero_action_indexs[i] = EditorGUILayout.Popup("  zero", zero_action_indexs[i], NullAttributeNames);
                    if (GUILayout.Button("remove"))
                    {
                        zero_action_indexs.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("add action"))
                {
                    zero_action_indexs.Add(0);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            var zero_index_list = serializedObject.FindProperty("zero_action_indexs");
            zero_index_list.ClearArray();
            for (int i = 0; i < zero_action_indexs.Count; i++)
            {
                zero_index_list.InsertArrayElementAtIndex(i);
                zero_index_list.GetArrayElementAtIndex(i).intValue = zero_action_indexs[i];
            }

            var zero_names_list = serializedObject.FindProperty("zero_action_names");
            zero_names_list.ClearArray();
            for (int i = 0; i < zero_action_indexs.Count; i++)
            {
                zero_names_list.InsertArrayElementAtIndex(i);
                zero_names_list.GetArrayElementAtIndex(i).stringValue = NullAttributeNames[zero_action_indexs[i]];
            }
            #endregion

            serializedObject.ApplyModifiedProperties();

        }

        private void GetMethod()
        {
            Type controls_type = typeof(Controls_Utility);
            MethodInfo[] methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);

            List<string> FloatMethodlist = new List<string>();
            List<string> NullMethodlist = new List<string>();

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 1 && returnType.Equals(typeof(void)) && parameters[0].ParameterType.Equals(typeof(float)))
                {
                    var method_name = method.GetCustomAttribute(typeof(ControlsAction));
                    if (method_name != null)
                    {
                        FloatMethodlist.Add((method_name as ControlsAction).action_name);
                    }
                    else
                    {
                        FloatMethodlist.Add(method.Name);
                    }
                }
                if(parameters.Length == 0&& returnType.Equals(typeof(void)))
                {
                    var method_name = method.GetCustomAttribute(typeof(ControlsAction));
                    if (method_name != null)
                    {
                        NullMethodlist.Add((method_name as ControlsAction).action_name);
                    }
                    else
                    {
                        NullMethodlist.Add(method.Name);
                    }
                }
            }
            NullAttributeNames = NullMethodlist.ToArray();
            FloatAttributeNames = FloatMethodlist.ToArray();
        }
    }
}
