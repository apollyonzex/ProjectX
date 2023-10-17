using UnityEngine;

namespace Camp.Level_Entrances
{
    [CreateAssetMenu(fileName = "Level_Entrance_Asset", menuName = "ScriptObejct/Level_Entrance_Asset", order = 2)]
    public class Level_Entrance_Asset : ScriptableObject
    {
        public Level_Enterance_Serializable[] cells;
    }
}

