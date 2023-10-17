using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace World_Formal.Card.Controls
{
    public class TimerControls : CardController
    {
        public int interval;
        private int currnet_time;

        [HideInInspector]
        public List<int> timer_action_indexs = new();
        [HideInInspector]
        public List<string> timer_action_names = new();


        public Action timer_action;

        public override void init()
        {
            Type controls_type = typeof(Controls_Utility);

            foreach (var timer_action_name in timer_action_names)
            {
                MethodInfo timer_method = controls_type.GetMethod(timer_action_name, new Type[] { });
                if (timer_method == null)
                {
                    MethodInfo[] methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction) != null)
                        {
                            if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == timer_action_name)
                                timer_action += (Action)Delegate.CreateDelegate(typeof(Action), method);
                        }
                    }
                }
                else
                {
                    timer_action += (Action)Delegate.CreateDelegate(typeof(Action), timer_method);
                }
            }

        }

        public override void tick()
        {
            currnet_time++;
            if(currnet_time >= interval)
            {
                currnet_time = 0;
                timer_action?.Invoke();
            }
        }
    }
}
