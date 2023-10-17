using GraphNode;
using System;
using UnityEngine;
using World_Formal.Enemys.Projectiles;
using World_Formal.Helpers;

namespace World_Formal.BT_GraphFlow.Nodes
{
    [System.Serializable]
    [Graph(typeof(BT_Graph))]
    public class Create_ProjectileNode : BT_Node
    {
        [ShowInBody(format = "projectile_id -> {0}")]
        public int projectile_id;

        [ShowInBody(format = "dir_x -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression dir_x;

        [ShowInBody(format = "dir_y -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression dir_y;

        [ShowInBody(format = "velocity -> {0}")]
        [ExpressionType(CalcExpr.ValueType.Floating)]
        public BT_Expression velocity;

        //================================================================================================

        #region Input
        [Input]
        [Display("_", seq = 1)]
        public System.Action<BT_Context> _i { get; set; }
        #endregion


        #region Output
        [Display("collide_target")]
        [Output(can_multi_connect = true)]
        public Action<BT_Context, Projectile> collide_target { get; set; }


        [Display("collide_ground")]
        [Output(can_multi_connect = true)]
        public Action<BT_Context, Projectile> collide_ground { get; set; }


        [Display("collide_projectile")]
        [Output(can_multi_connect = true)]
        public Action<BT_Context, Projectile> collide_projectile { get; set; }
        #endregion


        public override void do_self(BT_Context ctx)
        {
            var _dir_x = dir_x.do_calc_float(ctx);
            var _dir_y = dir_y.do_calc_float(ctx);
            var _velocity = velocity.do_calc_float(ctx);

            //创建子弹
            if (!Enemy_Move_Helper.instance.try_create_projectile(ctx, (uint)projectile_id, new Vector2(_dir_x, _dir_y), _velocity, out var cell))
            {
                ctx.is_last_method_ret = false;
                return;
            }

            //初始化事件
            {
                cell.do_when_collide_target += () =>
                {
                    collide_target?.Invoke(ctx, cell);
                };

                cell.do_when_collide_ground += () =>
                {
                    collide_ground?.Invoke(ctx, cell);
                };
            }
            

            ctx.is_last_method_ret = true;
        }
    }
}

