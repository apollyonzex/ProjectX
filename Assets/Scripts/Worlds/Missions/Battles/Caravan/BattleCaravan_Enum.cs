using UnityEngine;

namespace Worlds.Missions.Battles.Caravan
{
    public class BattleCaravan_Enum : MonoBehaviour
    {
        /// <summary>
        /// 主状态机
        /// </summary>
        public enum Main_State
        {
            Idle = 0,
            Run = 1,
            Brake = 2,
            Jump = 3,
            Land = 4,
            Spurt = 5,
            Jumping = 6,
        }
    }
}

