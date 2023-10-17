using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.DS
{
    public interface ITarget
    {
        Vector2 Position { get; }

        Collider2D collider { get; }

        /// <summary>
        /// 受伤
        /// </summary>
        void hurt(int dmg);
    }
}

