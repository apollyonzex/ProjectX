using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using World_Formal.Card;

namespace World_Formal.Card.Controls
{
    public class LeverControls : CardController
    {
        #region 委托的可视化配置变量
        [HideInInspector]
        public List<int> change_action_indexs;
        [HideInInspector]
        public List<string> change_action_names;
        [HideInInspector]
        public List<int> zero_action_indexs;
        [HideInInspector]
        public List<string> zero_action_names;
        [HideInInspector]
        public List<int> one_action_indexs;
        [HideInInspector]
        public List<string> one_action_names;
        [HideInInspector]
        public List<int> value_action_indexs;
        [HideInInspector]
        public List<string> value_action_names;
        #endregion

        public Lever lever;
        [HideInInspector]
        public float lever_value;

        public RectTransform min_y_position;
        public RectTransform max_y_position;

        public Action lever_change_action;
        public Action lever_zero_action;
        public Action lever_one_action;
        public Action<float> lever_value_action;

        private float max_y, min_y;

        public override void init()
        {
            min_y = min_y_position.localPosition.y;
            max_y = max_y_position.localPosition.y;

            Type controls_type = typeof(Controls_Utility);
            MethodInfo[] methods;
            foreach(var value_action_name in value_action_names)
            {
                MethodInfo value_method = controls_type.GetMethod(value_action_name,new Type[] { typeof(float)});
                if(value_method == null)
                {
                    methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach(var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction) != null)
                        {
                            if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == value_action_name)
                                lever_value_action += (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), method);
                        }
                    }
                }
                else
                {
                    lever_value_action += (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), value_method);
                }
            }

            foreach(var zero_action_name in zero_action_names)
            {
                MethodInfo zero_method = controls_type.GetMethod(zero_action_name, new Type[] { });
                if(zero_method == null)
                {
                    methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction) != null)
                        {
                            if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == zero_action_name)
                                lever_zero_action += (Action)Delegate.CreateDelegate(typeof(Action), method);
                        }
                    }
                }
                else
                {
                    lever_zero_action += (Action)Delegate.CreateDelegate(typeof(Action), zero_method);
                }
            }

            foreach (var one_action_name in one_action_names)
            {
                MethodInfo one_method = controls_type.GetMethod(one_action_name, new Type[] { });
                if (one_method == null)
                {
                    methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction) != null)
                        {
                            if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == one_action_name)
                                lever_one_action += (Action)Delegate.CreateDelegate(typeof(Action), method);
                        }
                    }
                }
                else
                {
                    lever_one_action += (Action)Delegate.CreateDelegate(typeof(Action), one_method);
                }
            }


            foreach (var change_action_name in change_action_names)
            {
                MethodInfo change_method = controls_type.GetMethod(change_action_name, new Type[] { });
                if (change_method == null)
                {
                    methods = controls_type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction) != null)
                        {
                            if ((method.GetCustomAttribute(typeof(ControlsAction)) as ControlsAction).action_name == change_action_name)
                                lever_change_action += (Action)Delegate.CreateDelegate(typeof(Action), method);
                        }
                    }
                }
                else
                {
                    lever_change_action += (Action)Delegate.CreateDelegate(typeof(Action), change_method);
                }
            }

        }

        public override void tick()
        {
            if(lever_value == 0)
            {
                lever_zero_action?.Invoke();
            }
            else if(lever_value  == 1)
            {
                lever_one_action?.Invoke();
            }
            else
            {
                lever_value_action?.Invoke(lever_value);
            }
        }

        public  void update_lever_value(Vector3 target_pos)
        {

            lever_value = Mathf.Clamp((target_pos.y - min_y) / (max_y - min_y), 0, 1);
            var rct = lever.GetComponent<RectTransform>().localPosition;
            lever.GetComponent<RectTransform>().localPosition = new Vector3(rct.x, Mathf.Lerp(min_y, max_y, lever_value), rct.z);

            lever_change_action?.Invoke();
        }


        public virtual void rebound()
        {
            lever_value = 0;
            lever.GetComponent<RectTransform>().localPosition = new Vector3(lever.GetComponent<RectTransform>().localPosition.x, min_y, lever.GetComponent<RectTransform>().localPosition.z);
        }
    }
}
