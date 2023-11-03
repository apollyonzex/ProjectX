using UnityEngine;

namespace Common
{
    [CreateAssetMenu(menuName = "GameConfig", fileName = "GameConfig")]
    public class Config : ScriptableObject
    {
        #region codes
        public static Config current
        {
            get
            {
                if (s_current == null)
                {
                    s_current = CreateInstance<Config>();
                }
                return s_current;
            }
        }
        private static Config s_current;
        private void OnEnable()
        {
            s_current = this;
        }
        private void OnDisable()
        {
            if (ReferenceEquals(s_current, this))
            {
                s_current = null;
            }
        }
        #endregion


        #region const
        //帧率
        public const int PHYSICS_TICKS_PER_SECOND = 120;
        public const float PHYSICS_TICK_DELTA_TIME = 1f / PHYSICS_TICKS_PER_SECOND;
        #endregion


        #region internal_setting
        //tick优先级
        public const int EnemyMgr_Priority = 0;
        public const int HandCardMgr_Priority = 1;

        //tick管理类
        public const string EnemyMgr_Name = "EnemyMgr_Name";
        public const string HandCardMgr_Name = "HandCardMgr_Name";

        //普通管理类
        public const string SlotMgr_Player_Front_Name = "p_front_slot";
        public const string SlotMgr_Player_Back_Name = "p_back_slot";
        public const string SlotMgr_Enemy_Front_Name = "e_front_slot";
        public const string SlotMgr_Enemy_Back_Name = "e_back_slot";

        #endregion
    }
}

