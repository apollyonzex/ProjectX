namespace Common_Formal
{
    public class Enum
    {
        public enum EN_faction
        {
            enemy,
            player,
            neutral,
        }

        public enum EN_caravan_move_status
        {
            idle,
            run,
            jump,
            jumping,
            land,
        }


        public enum EN_caravan_acc_status
        {
            driving,
            braking,
        }


        public enum EN_caravan_liftoff_status
        {
            ground,
            sky,
        }


        public enum EN_caravan_glide_status
        {
            not_ready,
            ready,
        }


        public enum EN_caravan_anim_status
        {
            idle = 0,
            run = 1,
            brake = 2,
            jump = 3,
            land = 4,
            spurt = 5,
            jumping = 6,
        }


        public enum EN_caravan_anim_trigger_status
        {
            none = 0,
            jump = 1,
            land = 2,
        }
    }




}

