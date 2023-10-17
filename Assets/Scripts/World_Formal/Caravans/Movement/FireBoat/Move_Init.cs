using Common_Formal;
using System.Collections.Generic;
using static World_Formal.Helpers.Caravan_Move_Helper;

namespace World_Formal.Caravans.Movement.FireBoat
{
    public class Move_Init
    {
        public Dictionary<Enum.EN_caravan_move_status, IMove_AC> get_dic()
        {
            Dictionary<Enum.EN_caravan_move_status, IMove_AC> dic = new()
            {
                { Enum.EN_caravan_move_status.idle, new Move_Idle() },
                { Enum.EN_caravan_move_status.run, new Move_Run_FireBoat() },
                { Enum.EN_caravan_move_status.jump, new Move_Jump() },
                { Enum.EN_caravan_move_status.jumping, new Move_Jumping() },
                { Enum.EN_caravan_move_status.land, new Move_Land() },
            };

            return dic;
        }
    }
}

