using Common_Formal;
using System;
using System.Collections.Generic;
using World_Formal.Caravans;
using World_Formal.Card;

namespace World_Formal.Card.Controls
{
    public static class Controls_Utility
    {
        [ControlsAction("caravan/accelerate")]
        public static void accelerate_caravan()
        {
            WorldContext.instance.caravan_acc_status = Common_Formal.Enum.EN_caravan_acc_status.driving;
        }
        [ControlsAction("caravan/brake")]
        public static void brake_caravan()
        {
            WorldContext.instance.caravan_acc_status = Common_Formal.Enum.EN_caravan_acc_status.braking;
        }

        public static void caravan_jump()
        {
            World_Formal.Helpers.Caravan_Move_Helper.instance.jump(WorldContext.instance, 10, false);
        }

        public static void do_nothing()
        {

        }

        public static void draw_card()
        {
            Mission.instance.try_get_mgr(Common.Config.CardMgr_Name, out CardManager cardMgr);
            cardMgr.DrawCard(null,0);           //直接向0号区抽卡
        }

        public static void debug_float(float value)
        {
            UnityEngine.Debug.Log($"------------------VALUE  : {value}-------------------");
        }

        public static void add_steam_pressure()
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal caravanMgr);
            caravanMgr.caravan_property["steam_pressure"].change_value(Common.Config.PHYSICS_TICK_DELTA_TIME);
        }

        public static void add_steam_pressure(float f)
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal caravanMgr);
            caravanMgr.caravan_property["steam_pressure"].change_value(f * Common.Config.PHYSICS_TICK_DELTA_TIME);
        }

        public static void braking(float f)
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal caravanMgr);
            WorldContext.instance.caravan_acc_status = Common_Formal.Enum.EN_caravan_acc_status.braking;
            WorldContext.instance.acc_braking_mod = caravanMgr.cell._desc.f_acc_braking * f;
        }

        public static void braking()
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal caravanMgr);
            WorldContext.instance.caravan_acc_status = Common_Formal.Enum.EN_caravan_acc_status.braking;
            WorldContext.instance.acc_braking_mod = caravanMgr.cell._desc.f_acc_braking * 1f;
        }

        public static void decrease_steam_pressure()
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal caravanMgr);
            caravanMgr.caravan_property["steam_pressure"].change_value(caravanMgr.caravan_property["steam_pressure_decrease"].value * Common.Config.PHYSICS_TICK_DELTA_TIME);
        }
    }
}
