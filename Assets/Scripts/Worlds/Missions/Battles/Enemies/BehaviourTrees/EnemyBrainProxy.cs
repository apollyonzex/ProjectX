using BehaviourFlow;
using CalcExpr;
using GraphNode;
using System;
using System.Collections.Generic;
using static Worlds.Missions.Battles.Enemies.BehaviourTrees.EnemyBrain_Enum;


namespace Worlds.Missions.Battles.Enemies.BehaviourTrees
{
    public class EnemyBrainProxy : BehaviourFlow.IContext
    {
        Type GraphNode.IContext.context_type => typeof(EnemyBrainProxy);

        public readonly EnemyBrain owner;

        //==================================================================================================


        public EnemyBrainProxy(EnemyBrain owner)
        {
            this.owner = owner;
        }


        IEnumerator<(string name, float value)> BehaviourFlow.IContext.enumerate_shared_floats()
        {
            throw new NotImplementedException();
        }

        IEnumerator<(string name, int value)> BehaviourFlow.IContext.enumerate_shared_ints()
        {
            throw new NotImplementedException();
        }

        float BehaviourFlow.IContext.get_shared_float(string name)
        {
            throw new NotImplementedException();
        }

        int BehaviourFlow.IContext.get_shared_int(string name)
        {
            throw new NotImplementedException();
        }

        void BehaviourFlow.IContext.set_shared_float(string name, float value)
        {
            throw new NotImplementedException();
        }

        void BehaviourFlow.IContext.set_shared_int(string name, int value)
        {
            throw new NotImplementedException();
        }


        [ExprConst]
        public bool has_target => owner.target != null;//是否有目标

        [ExprConst("target.distance.x")]
        public float target_distance_x => owner.target_dis_x;//离目标的x距离

        [ExprConst("target.distance.y")]
        public float target_distance_y => owner.target_dis_y;//离目标的y距离

        [ExprConst]
        public bool is_Front => owner.side == Side_State.Front;//自身是否位于目标前方

        [ExprConst]
        public bool is_Behind => owner.side == Side_State.Behind;//自身是否位于目标后方

        [ExprConst("self.position.x")]
        public float self_x => owner.cell.position.x;//自身x

        [ExprConst("self.position.y")]
        public float self_y => owner.cell.position.y;//自身y

        [ExprConst("target.position.x")]
        public float target_x => owner.target.position.x;//目标x

        [ExprConst("target.position.y")]
        public float target_y => owner.target.position.y;//目标y

        [ExprConst("target.velocity.x")]
        public float target_velocity_x => owner.target.velocity.x;//目标移动速度(x轴)

        [ExprConst("target.velocity.y")]
        public float target_velocity_y => owner.target.velocity.y;//目标移动速度(y轴)

        [ExprConst]
        public float speed => owner.cell.move_speed;//自身速度大小，根据配置表获得


        [GraphAction("Set Value/Velocity_Vector2")]
        public bool set_velocity(BTExecutorBase _, float x, float y)
        {
            owner.velocity.x = x;
            owner.velocity.y = y;
            return true;
        }


        [GraphAction("Set Value/Velocity_X")]
        public bool set_velocity_x(BTExecutorBase _, [ShowInBody(format = "vx => {0}")] float x)
        {
            owner.velocity.x = x;
            return true;
        }


        [GraphAction("Set Value/Velocity_Y")]
        public bool set_velocity_y(BTExecutorBase _, float y)
        {
            owner.velocity.y = y;
            return true;
        }


        [GraphAction("Set Value/Acc_Vector2")]
        public bool set_acc(BTExecutorBase _, float x, float y)
        {
            owner.acc.x = x;
            owner.acc.y = y;
            return true;
        }


        [GraphAction("Set Value/Acc_X")]
        public bool set_acc_x(BTExecutorBase _, float x)
        {
            owner.acc.x = x;
            return true;
        }


        [GraphAction("Set Value/Acc_Y")]
        public bool set_acc_y(BTExecutorBase _, float y)
        {
            owner.acc.y = y;
            return true;
        }


        [GraphAction("Move/to Target")]
        public bool moving_to_target(BTExecutorBase _)
        {
            owner.moving_state = Moving_State.ToTarget;
            return true;
        }


        [GraphAction("Move/Free")]
        public bool moving_free(BTExecutorBase _)
        {
            owner.moving_state = Moving_State.Free;
            return true;
        }


        [GraphAction("Target/Make Player as Target")]
        public bool make_player_as_target(BTExecutorBase _)
        {
            var target = BattleSceneRoot.instance.battleMgr.caravan_mgr.caravan;
            owner.target = target;
            owner.upd_parameters();
            return false;
        }


        [GraphAction("Move/Stop")]
        public bool moving_stop(BTExecutorBase _)
        {
            owner.moving_state = Moving_State.Stop;
            return true;
        }


        [GraphAction()]
        public bool change_battle_state(BTExecutorBase _, [ShowInBody(format = "battle_state : {0}")] int id)
        {
            return owner.try_enter_battle_state(id);
        }


        [GraphAction()]
        public bool create_projectile(BTExecutorBase _)
        {
            owner.main_state = Main_State.Missile;
            return true;
        }


        [GraphAction()]
        public bool jump(BTExecutorBase _, float height, float vx)
        {
            owner.jump(height, vx);
            return true;
        }
    }

}

