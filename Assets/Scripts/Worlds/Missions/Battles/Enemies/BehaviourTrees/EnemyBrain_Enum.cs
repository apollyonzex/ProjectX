

namespace Worlds.Missions.Battles.Enemies.BehaviourTrees
{
    public class EnemyBrain_Enum
    {
        /// <summary>
        /// 用于控制移动
        /// </summary>
        public enum Moving_State
        {
            None,
            Stop,
            ToTarget,
            Free,
        }

        /// <summary>
        /// 该敌人相对于小车的side
        /// </summary>
        public enum Side_State
        {
            Behind,
            Front,
            None
        }


        /// <summary>
        /// 战斗姿态
        /// </summary>
        public enum Battle_State
        { 
            Default,
            Withstand,
            Melee,
            Board,
        }


        /// <summary>
        /// 主状态机 - Enemy状态
        /// </summary>
        public enum Main_State 
        {
            Idle = 0,      //空闲
            Moving = 1,    //移动
            Holding = 2,   //在前方顶住大篷车
            Boarding = 3,  //扒在大篷车上
            Jumping = 4,   //跳起（在空中）
            Landing = 5,   //着陆
            Missile = 6,   //远程攻击
            Dead = 7,      //死亡
            Lie = 8,       //空中死亡，向下掉落
            Jump = 9       //跳起（的瞬间）
        }
    }

}

