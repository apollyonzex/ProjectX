using UnityEngine;
using World_Formal.BT_GraphFlow;
using World_Formal.Enemys.Projectiles;

namespace World_Formal.Enemys
{
    public interface IEnemy_Move
    {
        void move(BT_Context ctx, Vector2 velocity);

        void jump(BT_Context ctx, float vx, float height);

        bool try_create_projectile(BT_Context ctx, uint id, Vector2 pos, Vector2 dir, float velocity, out Projectile projectile);

        void notify_on_land(BT_Context ctx);


        /// <summary>
        /// 初始化运动参数
        /// </summary>
        void init_move_prms(BT_Context ctx);
    }
}

