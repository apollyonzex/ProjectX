using Devices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour,ITarget
{
    Vector2 ITarget.position =>transform.position;
}
