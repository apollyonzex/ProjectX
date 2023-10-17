

using System.Collections.Generic;

namespace CaravanEnhanced{

    public enum itemType {
        wheel = 0,
        item = 1 ,
    }

    public class Item {

        public uint id;
        public string name;
        public string description;


        public string graph_path;

        public Dictionary<AutoCode.Tables.Item.e_slotType, (string,string)> item_paths = new Dictionary<AutoCode.Tables.Item.e_slotType, (string, string)>();
        public Dictionary<AutoCode.Tables.Item.e_slotType, (string, string)> item_battle_paths = new Dictionary<AutoCode.Tables.Item.e_slotType, (string, string)>();

        public int current_hp;
        public int hp;
        public int damage;

        public string icon_path;

        //
        public int driving_speed_limit;

        public int driving_acc;

        public int braking_acc;

        public int descend_speed_limit;
        // *1000


        public float height;

        public Slot owner = null;

        public HashSet<string> tags = new HashSet<string>();

        public (int, int) size;
    }
}
