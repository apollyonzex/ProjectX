using UnityEngine;
using UnityEngine.UI;

namespace CaravanEnhanced {
    public class CargoItemInfo :MonoBehaviour{

        public Text _name;
        public Text description;
        public Text health_info;

        public void set_info(string _name,string desc,int current_hp,int hp) {
            this._name.text = $"名称: {_name}";
            description.text = $"信息: {desc}";
            health_info.text = $"耐久: {current_hp}/{hp}";
        }
    }
}
