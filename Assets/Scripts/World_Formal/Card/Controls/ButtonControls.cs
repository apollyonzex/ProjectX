using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using World_Formal.Card;

namespace World_Formal.Card.Controls
{
    public class ButtonControls : CardController, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector]
        public List<int> press_action_indexs = new();
        [HideInInspector]
        public List<string> press_action_names = new();

        [HideInInspector]
        public List<int> release_action_indexs = new();
        [HideInInspector]
        public List<string> release_action_names = new();

        public Action press_button_action;

        public Action release_button_action;

        public override void init()
        {
            Type controls_type = typeof(Controls_Utility);
            foreach(var press_action_name in press_action_names)
            {
                //先直接找函数
                MethodInfo press_method = controls_type.GetMethod(press_action_name, new Type[] { });
                if (press_method == null)
                {
                    MethodInfo[] methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction)!=null)
                        {
                            if((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == press_action_name)
                                press_button_action += (Action)Delegate.CreateDelegate(typeof(Action), method);
                        }
                    }
                }
                else
                {
                    press_button_action += (Action)Delegate.CreateDelegate(typeof(Action), press_method);
                }
            }

            foreach(var release_action_name in release_action_names)
            {
                MethodInfo release_method = controls_type.GetMethod(release_action_name, new Type[] { });
                if (release_method == null)
                {
                    MethodInfo[] methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction) != null)
                        {
                            if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == release_action_name)
                                release_button_action += (Action)Delegate.CreateDelegate(typeof(Action), method);
                        }
                    }
                }
                else
                {
                    release_button_action += (Action)Delegate.CreateDelegate(typeof(Action), release_method);
                }
            }
        }


        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            press_button_action?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            release_button_action?.Invoke();
        }
    }
}
