using Foundation;
using UnityEngine;
using UnityEngine.UI;

namespace World_Formal.Adventure
{
    public class AdventureView : MonoBehaviour, IAdventureMgr
    {
        AdventureMgr owner;

        public Slider exp_slider;

        public Image level_text_1,level_text_2;

        public BonusView bonus_view;

        public Sprite[] numbers = new Sprite[10];

        //==================================================================================================

        void IModelView<AdventureMgr>.attach(AdventureMgr owner)
        {
            this.owner = owner;
            update_info();
        }
        void IModelView<AdventureMgr>.detach(AdventureMgr owner)
        {
            this.owner = null;
        }


        void IModelView<AdventureMgr>.shift(AdventureMgr old_owner, AdventureMgr new_owner)
        {
        }


        private void update_info()
        {
            exp_slider.maxValue = owner.player_exp.current_record.f_exp;
            exp_slider.value = owner.player_exp.exp;
            SetLevelSprite();
        }

        void IAdventureMgr.AddPlayerExp()
        {
            update_info();
        }

        void IAdventureMgr.DeliverBonus(UpgradeBonus bonus)
        {
            var b = Instantiate(bonus_view, transform, false);
            b.init(bonus);
            b.gameObject.SetActive(true);
        }

        private void SetLevelSprite()
        {
            var level = owner.player_exp.current_record.f_level;
            level_text_1.sprite = numbers[level / 10];
            level_text_2.sprite = numbers[level % 10];
        }
    }
}

