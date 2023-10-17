using Devices;
using Foundation.Tables;
using UnityEngine;
using Worlds.Missions.Battles;
using Worlds.Missions.Battles.Caravan;
using static Worlds.Missions.Battles.Caravan.BattleCaravan_Enum;

namespace Worlds.CardSpace
{
    public static class CardUtility
    {
        public static ExprTreeConverter converter = new ExprTreeConverter("Worlds.CardSpace.", ", Game", "Worlds.CardSpace.", ", Game");
    }

    public interface IFunc
    {
        bool exec(BattleCard card);
    }

    public interface IJudge
    {
        bool result();
    }

    public interface IData {
        float getData();
    }
    #region 运算符

    public class Not : IJudge {

        IJudge j;

        public Not(IJudge j) {
            this.j = j;
        }
        bool IJudge.result() {
            return (!j.result());
        }
    }

    public class Or: IJudge {
        IJudge J1;
        IJudge J2;


        public Or(IJudge j1, IJudge j2) {
            J1 = j1;
            J2 = j2;
        }

        bool IJudge.result() {
            return (J1.result()||J2.result());
        }
    }
    public class And : IFunc,IJudge {

        IFunc f1;
        IFunc f2;
        public And(IFunc f1, IFunc f2)
        {
            this.f1 = f1;
            this.f2 = f2;
        }
        bool IFunc.exec(BattleCard card)
        {
            return f1.exec(card) && f2.exec(card);
        }


        IJudge J1;
        IJudge J2;

        public And(IJudge j1, IJudge j2)
        {
            this.J1 = j1;
            this.J2 = j2;
        }

        bool IJudge.result()
        {
            return (J1.result() && J2.result());
        }
    }

    public class Greater : IJudge {

        IData d1;
        IData d2;
        bool hasData2 = false;
        float num;

        public Greater(IData d, ExprTreeInt _int) {
            d1 = d;
            num = _int.as_int;
        }
        public Greater(IData d, ExprTreeFloat _float) {
            d1 = d;
            num = _float.as_float;
        }

        public Greater(IData d1,IData d2) {
            this.d1 = d1;
            this.d2 = d2;
            hasData2 = true;
        }
        bool IJudge.result() {
            if(hasData2) {
                return d1.getData() > d2.getData();
            }
            return d1.getData() >num;
        }
    }


    public class Eq : IJudge
    {
        IData d1;
        IData d2;
        bool hasData2 = false;
        float num;

        public Eq(IData d, ExprTreeInt _int) {
            d1 = d;
            num = _int.as_int;
        }

        public Eq(IData d,ExprTreeFloat _float) {
            d1 = d;
            num = _float.as_float;
        }

        public Eq(IData d1,IData d2) {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result()
        {
            if (hasData2) {
                return d1.getData() == d2.getData();
            }
            return d1.getData() == num;
        }
    }

    public class GreaterEq : IJudge {
        IData d1;
        IData d2;
        bool hasData2 = false;
        float num;

        public GreaterEq(IData d, ExprTreeInt _int) {
            d1 = d;
            num = _int.as_int;
        }

        public GreaterEq(IData d, ExprTreeFloat _float) {
            d1 = d;
            num = _float.as_float;
        }

        public GreaterEq(IData d1, IData d2) {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result() {
            if (hasData2) {
                return d1.getData() >= d2.getData();
            }
            return d1.getData() >= num;
        }
    }
    public class LessEq : IJudge
    {
        IData d1;
        IData d2;
        bool hasData2 = false;
        float num;

        public LessEq(IData d, ExprTreeInt _int) {
            d1 = d;
            num = _int.as_int;
        }

        public LessEq(IData d, ExprTreeFloat _float) {
            d1 = d;
            num = _float.as_float;
        }

        public LessEq(IData d1, IData d2) {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result() {
            if (hasData2) {
                return d1.getData() <= d2.getData();
            }
            return d1.getData() <= num;
        }
    }

    public class Less: IJudge {
        IData d1;
        IData d2;
        bool hasData2 = false;
        float num;

        public Less(IData d, ExprTreeInt _int) {
            d1 = d;
            num = _int.as_int;
        }

        public Less(IData d, ExprTreeFloat _float) {
            d1 = d;
            num = _float.as_float;
        }

        public Less(IData d1, IData d2) {
            this.d1 = d1;
            this.d2 = d2;
        }
        bool IJudge.result() {
            if (hasData2) {
                return d1.getData() < d2.getData();
            }
            return d1.getData() <    num;
        }
    }

    #endregion
    public class TestFunc : IFunc {
        public string str;
        public TestFunc(string s)
        {
            str = s;
        }

        bool IFunc.exec(BattleCard card) {
            Debug.Log(str);

            return true;    
        }
    }

    public class Consume : IFunc
    {

        public Consume()
        {

        }

        bool IFunc.exec(BattleCard card)
        {
            return BattleSceneRoot.instance.battlecardMgr.RemoveCardinGY(card);
        }
    }

    public class DrawCard : IFunc
    {
        private int times;

        public DrawCard(int num)
        {
            times = num;
        }
        bool IFunc.exec(BattleCard card)
        {
            for (int i = 0; i < times; i++)
            {
                BattleSceneRoot.instance.battlecardMgr.DrawCard();
            }
            return true;
        }
    }

    public class AddCard : IFunc
    {

        private int num;
        private int card_id;
        public AddCard(int card_id, int num)
        {
            this.num = num;
            this.card_id = card_id;
        }

        bool IFunc.exec(BattleCard card)
        {
            for (int i = 0; i < num; i++)
            {
                BattleSceneRoot.instance.battlecardMgr.SendCardToHand(card_id,card.raw_data.owner);
            }
            return true;
        }
    }

    public class AddCard_DrawPile : IFunc {
        private int num;
        private int card_id;
        public AddCard_DrawPile(int card_id, int num) {
            this.num = num;
            this.card_id = card_id;
        }

        bool IFunc.exec(BattleCard card) {
            for (int i = 0; i < num; i++) {
                BattleSceneRoot.instance.battlecardMgr.SendCardToDeck(card_id, card.raw_data.owner);
            }
            return true;
        }
    }
/*    public class RemoveCard : IFunc             //这个方法没有实际运用,且存在设计缺陷,不再更新维护
    {
        private int num;
        private int card_id;
        public RemoveCard(int card_id, int num)
        {
            this.num = num;
            this.card_id = card_id;
        }
        bool IFunc.exec(BattleCard card)
        {
            var _card = BattleSceneRoot.instance.battlecardMgr.GetCard((uint)card_id);
            for (int i = 0; i < num; i++)
            {
                BattleSceneRoot.instance.battlecardMgr.RemoveCard(_card);
            }
            return true;
        }
    }*/

    public class ChangeCard : IFunc
    {
        private int origin_card_id;

        private int target_card_id;

        public ChangeCard(int id1, int id2)
        {
            this.origin_card_id = id1;
            this.target_card_id = id2;
        }

        bool IFunc.exec(BattleCard card)
        {
            var cards = BattleSceneRoot.instance.battlecardMgr.cards_inhand;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].raw_data.id == origin_card_id)
                {
                    var _card = BattleSceneRoot.instance.battlecardMgr.GetCard((uint)target_card_id);
                    _card.owner = card.raw_data.owner;
                    BattleSceneRoot.instance.battlecardMgr.ChangeCard(i, new BattleCard { raw_data = _card});
                    return  true;
                }
            }

            Debug.Log("变了个寂寞");
            return true;//先默认全部返回true     return false;
        }
    }

    public class DelayUseCard : IFunc
    {
        public int delay;

        public int origin_delay;

        private BattleCard card;

        public DelayUseCard(int ticks)
        {
            this.delay = ticks;
            origin_delay = delay;
        }

        bool IFunc.exec(BattleCard card)
        {
            this.card = card;
            BattleSceneRoot.physics_tick += delay_use;
            return true;
        }

        void delay_use()
        {
            delay--;
            if (delay <= 0)
            {
                var cards = BattleSceneRoot.instance.battlecardMgr.cards_inhand;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (card == cards[i])
                    {
                        BattleSceneRoot.instance.battlecardMgr.UseCardWithOutCost(i);
                        break;
                    }
                }
                delay = origin_delay;
                BattleSceneRoot.physics_tick -= delay_use;
            }
        }
    }


    public class DelayRemoveCard : IFunc
    {           //抽卡时专用

        public int delay;

        public int origin_delay;

        private BattleCard card;


        public DelayRemoveCard(int ticks)
        {
            delay = ticks;
            origin_delay = delay;
        }


        bool IFunc.exec(BattleCard card)
        {
            this.card = card;
            BattleSceneRoot.physics_tick += delay_remove;
            return true;
        }

        void delay_remove()
        {
            delay--;
            if (delay <= 0)
            {
                var cards = BattleSceneRoot.instance.battlecardMgr.cards_inhand;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (card == cards[i])
                    {
                        BattleSceneRoot.instance.battlecardMgr.RemoveCard(i);
                        break;
                    }
                }
                delay = origin_delay;
                BattleSceneRoot.physics_tick -= delay_remove;
            }
        }
    }

    public class CaravanPosState :IData
    {

        public CaravanPosState()
        {

        }
        float IData.getData() {
            return (int)BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.liftoff_status;
        }
    }

    public class CaravanDescendSpeed :IData
    {
        public CaravanDescendSpeed()
        {
        }

        float IData.getData() {
            return BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan.velocity.y;
        }
    }


    public class Jump : IFunc
    {
        public float height;
        public Jump(float height)
        {
            this.height = height;
        }

        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.jump(height);
            return true;
        }
    }

    public class Glide : IFunc
    {
        public bool bl;

        public Glide(bool bl)
        {
            this.bl = bl;
        }

        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.glide(bl);
            return true;
        }
    }


    public class Brake : IFunc
    {
        public Brake()
        {
        }

        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.brake();
            return true;
        }
    }


    public class Add_Braking_Acc : IFunc
    {
        public int acc;

        public Add_Braking_Acc(int acc)
        {
            this.acc = acc;
        }

        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.add_braking_acc(acc);
            return true;
        }
    }


    public class Drive : IFunc
    {
        public Drive()
        {
        }

        bool IFunc.exec(BattleCard card) {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.drive();
            return true;
        }
    }


    public class Add_Driving_Acc : IFunc
    {
        public int add;

        public Add_Driving_Acc(int add)
        {
            this.add = add;
        }

        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.add_driving_acc(add);
            return true;
        }
    }


    public class Add_Driving_Limit : IFunc
    {
        public int add;

        public Add_Driving_Limit(int add)
        {
            this.add = add;
        }

        public Add_Driving_Limit()
        {
            this.add = 0;
        }

        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.caravan_mgr.add_driving_limit(add);
            return true;
        }
    }


    public class ShockWave : IFunc
    {
        public ShockWave()
        { 
        }

        bool IFunc.exec(BattleCard card)
        {
            var wheel = BattleSceneRoot.instance.battleMgr.device_mgr.m_wheel.transform;
            Common.Expand.Utility.try_load_asset("card", "effect/shockwave", out Missions.Battles.Effects.ShockWave prefab);
            var e = Object.Instantiate(prefab, wheel);
            e.init();
            

            card.raw_data.use_success_func = new Consume();
            return true;
        }
    }


    public class UnstableCoal : IFunc
    {
        public UnstableCoal()
        {
        }

        bool IFunc.exec(BattleCard card)
        {
            Common.Expand.Utility.try_load_asset("card", "effect/UnstableCoal", out Sprite asset);
            var gos = GameObject.FindGameObjectsWithTag("Timber");
            foreach (var go in gos)
            {
                if (!go.TryGetComponent<SpriteRenderer>(out var cmp)) continue;
                cmp.sprite = asset;
            }

            Common.Expand.Utility.try_load_asset("projectile", "pjt_0427_timber", out SpriteRenderer asset2);
            asset2.sprite = asset;
            asset2.tag = "UnstableCoal_PJT";

            card.raw_data.use_success_func = new Consume();
            return true;
        }
    }


    public class Boom : IFunc
    {
        Animator anm;

        public Boom()
        {
        }

        bool IFunc.exec(BattleCard card)
        {
            var caravan = BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan;

            Common.Expand.Utility.try_load_asset("temp", "boom", out Animator asset);
            var gos = GameObject.FindGameObjectsWithTag("UnstableCoal_PJT");
            foreach (var go in gos)
            {
                go.GetComponent<SpriteRenderer>().enabled = false; //隐藏自身
                anm = Object.Instantiate(asset, go.transform);
                anm.Play("melee_big_circle_bullet_explosion");
                BattleSceneRoot.instance.delay_boom(anm.gameObject, 0.3f);

                if (caravan.liftoff_status == Liftoffstatus.sky && caravan.glide_status == Glidestatus.ready)
                {
                    caravan.driving_speed_limit = 55500;
                    caravan.velocity.x = 55.5f;
                    caravan.velocity.y = 20f;
                }
            }

            card.raw_data.use_success_func = new Consume();
            return true;
        } 
    }


    public class Focus_Back : IFunc
    {
        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.focus.back();
            BattleSceneRoot.instance.battleMgr.caravan_mgr.is_spurt();
            return true;
        }
    }


    public class Focus_Forward : IFunc
    {
        bool IFunc.exec(BattleCard card)
        {
            //BattleSceneRoot.instance.battleMgr.focus.is_forward = true;
            return true;
        }
    }


    public class Focus_RC : IFunc
    {
        bool IFunc.exec(BattleCard card)
        {
            BattleSceneRoot.instance.battleMgr.focus.recover();
            return true;
        }
    }


    public class ActivateDevice : IFunc {

        string active_tag;

        string module_id;

        private bool success = false;
        public ActivateDevice(string tag,string id) {
            active_tag = tag;
            module_id = id;
        }
        bool IFunc.exec(BattleCard card) {
            foreach (var device in BattleSceneRoot.instance.battleMgr.device_mgr.devices) {
                var battle_device = device.Key;
                if (battle_device.item.tags.Contains(active_tag)) {

                    battle_device.device.try_get_component<CardTrigger>(module_id, out var component);          //后续需要修改
                    if (component != null) {
                        if (component.trigger(battle_device.device.ctx)) {
                            success = true;
                        }
                    }
                }
            }
            return success;
        }
    }
}
