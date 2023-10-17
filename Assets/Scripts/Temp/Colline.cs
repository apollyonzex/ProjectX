using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Colline : MonoBehaviour
{

    public Transform p1,p2,p3;

    [Range(-1,1)]
    public float k;

    // Update is called once per frame
    void Update()
    {
        if (p1 != null && p2 != null &&p3!=null)
        {
            p3.position = k * (p2.position - p1.position) + p1.position;
        }
    }
}
