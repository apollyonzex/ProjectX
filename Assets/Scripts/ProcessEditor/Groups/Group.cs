using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcessEditor.Groups
{
    public class Group : MonoBehaviour
    {
        public float icon_pos;

        public ProcessAsset.Enemy[] enemies
        {
            get 
            {
                List<ProcessAsset.Enemy> list = new();

                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var pos = child.transform.localPosition;

                    ProcessAsset.Enemy e = new()
                    {
                        id = Convert.ToUInt16(child.name),
                        x = pos.x,
                        y = pos.y
                    };
                    list.Add(e);
                }

                return list.ToArray();
            }
        }
    }
}

