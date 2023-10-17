namespace Camp.Level_Entrances
{
    public class Level_Entrance
    {
        public uint seq; //顺序
        public string name;
        public uint world_id;
        public uint level_id;

        //==================================================================================================

        public Level_Entrance(Level_Enterance_Serializable data)
        {
            seq = data.seq;
            name = data.name;
            world_id = data.world_id;
        }
    }
}

