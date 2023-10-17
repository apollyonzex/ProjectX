namespace World_Formal.Enemys
{
    public class Enum
    {
        /// <summary>
        /// 怪物 - 主状态
        /// </summary>
        public enum EN_Main_State
        {
            Idle,
            Move,
            Hold, //顶住车
            Catch,
            Jump,
            Drop, //死亡 - 坠落
            FallDown //死亡 - 倒地
        }


        /// <summary>
        /// 怪物 - 攻击状态
        /// </summary>
        public enum EN_Attack_State
        { 
            Default,
            Defense,
            Catch, //扒车
        }


        public enum EN_Flip
        { 
            Right,
            Left
        }
    }
}

