using UnityEngine;

namespace Camp.Level_Entrances
{
    [System.Serializable]
    public class Level_Enterance_Serializable
    {
        public uint seq; //顺序
        public string name;

        public Vector2 pos;
        public Vector2 size;

        public uint world_id;
    }
}

