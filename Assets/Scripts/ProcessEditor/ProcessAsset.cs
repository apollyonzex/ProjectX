using UnityEngine;

namespace ProcessEditor
{
    [CreateAssetMenu(fileName = "process_data", menuName = "Process_Data")]
    public class ProcessAsset : ScriptableObject
    {
        [System.Serializable]
        public struct Enemy
        {
            public uint id;
            public float x;
            public float y;
        }

        [System.Serializable]
        public struct Group
        {
            public float icon_pos;
            public float trigger_pos;
            public Enemy[] enemies;
        }

        public Group[] groups;
        public float length;
    }
}

