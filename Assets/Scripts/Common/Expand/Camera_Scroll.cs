using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Scroll : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var scroll_dir = Mouse.current.scroll.ReadValue().normalized;
        if (scroll_dir == Vector2.zero) return;
        Vector3 dir = scroll_dir;

        //transform.localPosition += dir * Config.current.scene_mouse_scroll;
    }
}
