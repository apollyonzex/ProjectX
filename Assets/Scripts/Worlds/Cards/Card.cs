using CaravanEnhanced;

namespace Worlds.CardSpace {

    public class Card {

        #region 卡牌属性
        public uint id;
        public string name;
        public (string, string) image;
        public string description;
        public int rank;
        public int cost;
        public (IJudge, string) player_status;
        public IFunc draw_func;
        public IFunc use_func;
        public IFunc add_func;
        public IFunc use_success_func;
        public IFunc use_fail_func;
        #endregion

        public Item owner;
    }
}
