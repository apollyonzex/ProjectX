using UnityEngine;
using Worlds.Maps;
using UnityEngine.UI;

namespace Worlds.Missions.Battles
{
    public class BattleMgrView : MonoBehaviour, IBattleMgrView
    {
        internal BattleMgr owner;
        public Slider process;
        public Text text;
        public Image enemy_birth_icon;

        public Transform process_start;
        public Transform process_end;

        public void attach(BattleMgr owner)
        {
            this.owner = owner;
        }

        public void detach(BattleMgr owner)
        {
            if (this.owner != owner)
            {
                this.owner = null;
            }

            Destroy(gameObject);
        }

        public void shift(BattleMgr old_owner, BattleMgr new_owner)
        {          
        }

        /// <summary>
        /// 退出战斗，测试用 
        /// </summary>
        public void btn_fail()
        {
            //owner.end_battle(false);
            owner.end_battle_expand(false);
        }

        public void btn_win()
        {
            //owner.end_battle(true);
            owner.end_battle_expand(true);
        }


        //-------
        public void Update() {
            if (owner == null || owner.caravan_mgr==null)
                return;
            owner.caravan_mgr.get_process(out double now,out double all,out string str);
            process.maxValue = (float)all;
            process.value = (float)now;
            text.text = str;
            if(process.value/process.maxValue == 1) {
                btn_win();
            }
        }

        void IBattleMgrView.notify_add_enemy_birth_icon(float ratio)
        {
            var pos = owner.calc_enemy_icon_pos(ratio, process_start.localPosition, process_end.localPosition);

            var go = Instantiate(enemy_birth_icon, process.transform);
            go.transform.localPosition = pos;
            go.gameObject.SetActive(true);
        }
    }

}

