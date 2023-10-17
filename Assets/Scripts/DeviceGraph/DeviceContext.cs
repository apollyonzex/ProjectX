

using CalcExpr;
using Common;
using Devices;
using GraphNode;
using System.Collections.Generic;
using Worlds.CardSpace;
using Worlds.Missions.Battles.Enemies;

namespace DeviceGraph
{
    public class DeviceContext : IContext
    {
        public readonly Device device;
        public Projectile current_projectile;
        public Enemy current_enemy;
        public Vector2? current_normal;

        public bool current_bool = false;


        System.Type IContext.context_type => typeof(DeviceContext);

        public DeviceContext(Device device)
        {
            this.device = device;
        }

        public struct PushBool : System.IDisposable {
            DeviceContext ctx;
            bool b;

            public PushBool(DeviceContext ctx, bool b) {
                this.ctx = ctx;
                this.b = ctx.current_bool;
                ctx.current_bool = b;
            }

            public void Dispose() {
                ctx.current_bool = b;
            }
        }

        public struct PushProjectile : System.IDisposable {
            DeviceContext ctx;
            Projectile projectile;

            public PushProjectile(DeviceContext ctx, Projectile projectile) {
                this.ctx = ctx;
                this.projectile = ctx.current_projectile;
                ctx.current_projectile = projectile;
            }

            public void Dispose() {
                ctx.current_projectile = projectile;
            }
        }

        public struct PushEnemy : System.IDisposable {
            DeviceContext ctx;
            Enemy enemy;

            public PushEnemy(DeviceContext ctx, Enemy enemy) {
                this.ctx = ctx;
                this.enemy = ctx.current_enemy;
                ctx.current_enemy = enemy;
            }

            public void Dispose() {
                ctx.current_enemy = enemy;
            }
        }

        public struct PushNormal : System.IDisposable {
            DeviceContext ctx;
            Vector2? normal;

            public PushNormal(DeviceContext ctx, Vector2? normal) {
                this.ctx = ctx;
                this.normal = ctx.current_normal;
                ctx.current_normal = normal;
            }

            public void Dispose() {
                ctx.current_normal = normal;
            }
        }

        public PushProjectile projectile(Projectile projectile) => new PushProjectile(this, projectile); 

        public PushEnemy enemy(Enemy enemy) => new PushEnemy(this, enemy);

        public PushNormal normal(Vector2? normal) => new PushNormal(this, normal);

        public PushBool boolean(bool b) => new PushBool(this, b);

        //==================================================================================================


        [GraphAction]
        public bool projectile_vessel_add_and_become_empty(ProjectileVessel vessel, int additive) {
            var old = vessel.value;
            vessel.value += additive;
            return old > 0 && vessel.value == 0;
        }

        [GraphAction]
        public bool projectile_vessel_add_and_become_full(ProjectileVessel vessel, int additive) {
            var old = vessel.value;
            var max_value = vessel.max_value;
            vessel.value += additive;
            return old < max_value && vessel.value == max_value;
        }

        [GraphAction]
        public bool device_vessel_add_and_become_empty(DeviceVessel vessel, int additive) {
            var old = vessel.value;
            vessel.value += additive;
            return old > 0 && vessel.value == 0;
        }

        [GraphAction]
        public bool device_vessel_add_and_become_full(DeviceVessel vessel, int additive) {
            var old = vessel.value;
            var max_value = vessel.max_value;
            vessel.value += additive;
            return old < max_value && vessel.value == max_value;
        }
        [GraphAction]
        public bool device_vessel_add(DeviceVessel vessel,int additive) {
            var old = vessel.value;
            var max_value = vessel.max_value;
            vessel.value += additive;
            return true;
        }

        [GraphAction]
        public bool take_damage(Enemy _enemy, [ShowInBody(format = "  - [damage] : {0}")] int damage, float knock_back, float knock_dir_x, float knock_dir_y) {
            if (Config.current.open_damage_to_enemy) {
                _enemy.be_impacted(knock_back, new UnityEngine.Vector2(knock_dir_x, knock_dir_y));
                _enemy.be_damaged(damage);
            }
            return true;
        }

        [GraphAction]
        public bool destroy_self(Projectile p) {
            p.destroy();
            return true;
        }

        [GraphAction]
        public bool set_device_bool([ShowInBody(format ="  - [module_id] : {0}")] string module_id, [ShowInBody(format = "  - [bool] : {0}")] bool value) {
            if (device.try_get_component<DeviceBoolean>(module_id, out var component)) {
                component.value = value;
                return true;
            }
            return false;
        }

        [GraphAction]
        public bool set_device_float([ShowInBody(format = "  - [module_id] : {0}")] string module_id, [ShowInBody(format = "  - [float] : {0}")] float value) {
            if (device.try_get_component<DeviceFloat>(module_id, out var component)) {
                component.value = value;
                return true;
            }
            return false;
        }

        [GraphAction]
        public bool add_device_float([ShowInBody(format = "  - [module_id] : {0}")] string module_id, [ShowInBody(format = "  - [float] : {0}")] float value) {
            if (device.try_get_component<DeviceFloat>(module_id, out var component)) {
                component.value += value;
                return true;
            }
            return false;
        }

        [GraphAction]
        public bool set_device_vector2([ShowInBody(format = "  - [module_id] : {0}")] string module_id, [ShowInBody(format = "  - [float] : {0}")] float x, [ShowInBody(format = "  - [float] : {0}")] float y) {
            if (device.try_get_component<DeviceVector2>(module_id, out var component)) {
                component.x  = x;
                component.y = y;
                return true;
            }
            return false;
        }

        [GraphAction]
        public bool jump(CaravanData _, [ShowInBody(format = "  - [can_jump] : {0}")] bool can_jump, [ShowInBody(format = "  - [float] : {0}")] float height) {
            var caravan_mgr = Worlds.Missions.Battles.BattleSceneRoot.instance.battleMgr.caravan_mgr;
            if (can_jump) {
                caravan_mgr.jump(height);
                return true;
            }
            return false;
        }

        [GraphAction]
        public bool add_driving_speed(CaravanData _, [ShowInBody(format = "  - [int] : {0}")] int value) {
            var caravan_mgr = Worlds.Missions.Battles.BattleSceneRoot.instance.battleMgr.caravan_mgr;
            caravan_mgr.add_velocity((float)value/1000);
            return true;
        }

        [GraphAction]
        public bool add_driving_speed_limit(CaravanData _, [ShowInBody(format = "  - [int] : {0}")] int value) {
            var caravan_mgr = Worlds.Missions.Battles.BattleSceneRoot.instance.battleMgr.caravan_mgr;
            caravan_mgr.add_driving_limit(value);
            return true;
        }

        [GraphAction]
        public bool add_to_draw_list(Card _, [ShowInBody(format = "  - [int] : {0}")] int card_id, [ShowInBody(format = "  - [int] : {0}")] int card_num) {

            for(int i = 0; i < card_num; i++) {
                Worlds.Missions.Battles.BattleSceneRoot.instance.battlecardMgr.SendCardToDeck(card_id,device.item);
            }
            return true;
        }

        [GraphAction]
        public bool set_ctx_bool([ShowInBody(format = "  - [bool] : {0}")] bool value) {
            current_bool = value;
            return current_bool;
        }

        [ExprConst("table.damage")]
        public int damage => device.default_damage;


        //==================================================================================================
        Dictionary<string, object> m_device_configs_dic = new();

        public void set_device_config(string name, object dc)
        {
            m_device_configs_dic.Add(name, dc);
        }


        public bool get_device_config(string name, out object dc)
        {
            if (!m_device_configs_dic.TryGetValue(name, out dc)) return false;
            return true;
        }

    }



}
