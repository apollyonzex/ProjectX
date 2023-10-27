using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.Helpers
{
    public class Mouse_Move_Helper : Singleton<Mouse_Move_Helper>
    {
        public Vector2 offset;
        public Vector2 mouse_pos => calc_mouse_pos();
        public Vector2 drag_pos => mouse_pos + offset;

        Camera camera;

        //==================================================================================================

        public void init(Camera camera)
        {
            this.camera = camera;
        }


        Vector2 calc_mouse_pos()
        {
            var raw = Mouse.current.position.ReadValue();
            var ret = camera.ScreenToWorldPoint(raw);
            return ret;
        }


        public void calc_offset(Vector2 pos)
        {
            offset = pos - mouse_pos;
        }


        public void clear()
        {
            offset = Vector2.zero;
        }
    }
}

