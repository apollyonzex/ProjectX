using Common;
using Common_Formal;
using System;
using UnityEngine;
using World_Formal.Helpers;

namespace World_Formal.Caravans.Movement.FireBoat
{
    public class Move_Run_FireBoat : Caravan_Move_Helper.IMove_AC
    {
        void Caravan_Move_Helper.IMove_AC.@do(WorldContext ctx, CaravanMgr_Formal mgr)
        {
            var cell = mgr.cell;
            ref var pos = ref ctx.caravan_pos;
            ref var velocity = ref ctx.caravan_velocity;

            var steam_pressure = mgr.caravan_property["steam_pressure"].value;
            var steam_pressure_max = mgr.caravan_property["steam_pressure"].max;
            var steam_pressure_modify = MathF.Min(steam_pressure / (steam_pressure_max * 0.9f), 1);

            Road_Info_Helper.try_get_leap_rad(pos.x, out var ground_angle);
            var sin_ga = MathF.Sin(ground_angle);
            var cos_ga = MathF.Cos(ground_angle);

            float acc_temp = Config.current.gravity * sin_ga;
            if (ctx.caravan_acc_status == Common_Formal.Enum.EN_caravan_acc_status.driving) //加速中
            {
                acc_temp += EX_Utility.add_and_divide_1000(cell._desc.f_acc_driving, ctx.acc_driving_mod);
                acc_temp = MathF.Max(acc_temp, EX_Utility.add_and_divide_1000(cell._desc.f_acc_driving_min)) //规则：加速度不小于下限
                    + Mathf.Lerp(0, mgr.caravan_property["extra_acc"].value, steam_pressure_modify);
            } else if (ctx.caravan_acc_status == Common_Formal.Enum.EN_caravan_acc_status.braking) //制动中
              {
                acc_temp += EX_Utility.add_and_divide_1000(cell._desc.f_acc_braking, ctx.acc_braking_mod);
            }

            Vector2 acc = new(acc_temp * cos_ga, acc_temp * sin_ga);

            var velocity_temp = velocity + acc * Config.PHYSICS_TICK_DELTA_TIME;

            if (velocity_temp.x < 0) //规则：小车不可后退
            {
                velocity_temp = Vector2.zero;
            }

            var velocity_temp_mg = velocity_temp.magnitude;

            var speed_max_realtime = EX_Utility.add_and_divide_1000(cell._desc.f_speed_max * (1 - sin_ga));
            speed_max_realtime += ctx.speed_max_mod;
            speed_max_realtime = MathF.Max(speed_max_realtime, EX_Utility.add_and_divide_1000(cell._desc.f_speed_max_min))
                + Mathf.Lerp(0, mgr.caravan_property["extra_speed"].value, steam_pressure_modify);

            var delta_v = speed_max_realtime - velocity_temp_mg;
            if (delta_v < 0) {
                velocity_temp *= (velocity_temp_mg + MathF.Max(delta_v, EX_Utility.add_and_divide_1000(cell._desc.f_acc_overspeed * delta_v * delta_v * Config.PHYSICS_TICK_DELTA_TIME))) / velocity_temp_mg;
                velocity_temp_mg = velocity_temp.magnitude;
            }

            velocity = new(velocity_temp_mg * cos_ga, velocity_temp_mg * sin_ga);

            //进入飞车
            if (Road_Info_Helper.try_get_concavity(pos.x, out var concavity) && Road_Info_Helper.try_get_ground_p(pos.x, out var ground_p)) {
                var e = velocity_temp_mg * velocity_temp_mg / ground_p;
                var e1 = -Config.current.gravity * cos_ga;

                if (concavity < 0 && e > e1) {
                    Caravan_Move_Helper.instance.jump(ctx, velocity, true);
                    return;
                }
            }

            //正常行驶
            pos.x += velocity.x * Config.PHYSICS_TICK_DELTA_TIME;
            Road_Info_Helper.try_get_altitude(pos.x, out pos.y);
        }
    }
}

