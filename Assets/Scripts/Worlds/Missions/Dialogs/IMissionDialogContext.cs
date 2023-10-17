

using Assets.Scripts.World_Formal.Dialog_Formal;
using CalcExpr;
using GraphNode;

namespace Worlds.Missions.Dialogs {
    public interface IMissionDialogContext : IXContext {

        [GraphAction]
        void enter_normal_battle();

        [GraphAction]
        void enter_elite_battle();

        [GraphAction]
        void destroy();

        [GraphAction]
        void end_level_victory();

        [GraphAction]
        void end_level_fail();

        [GraphAction]
        void get_random_device(int pool_id, int item_num, int loop_times, float hp_remaining);

        [ExprConst]
        int hp_of_caravan_body { get; }

        [ExprFunc]
        int rnd_2(int w1, int w2);
        [ExprFunc]
        int rnd_3(int w1, int w2, int w3);
        [ExprFunc]
        int rnd_4(int w1, int w2, int w3, int w4);
        [ExprFunc]
        int rnd_5(int w1, int w2, int w3, int w4, int w5);
        [ExprFunc]
        int rnd_6(int w1, int w2, int w3, int w4, int w5, int w6);
        [ExprFunc]
        int rnd_7(int w1, int w2, int w3, int w4, int w5, int w6, int w7);
        [ExprFunc]
        int rnd_8(int w1, int w2, int w3, int w4, int w5, int w6, int w7, int w8);
        [ExprFunc]
        int rnd_9(int w1, int w2, int w3, int w4, int w5, int w6, int w7, int w8, int w9);
        [ExprFunc]
        int rnd_10(int w1, int w2, int w3, int w4, int w5, int w6, int w7, int w8, int w9, int w10);

        void push_waiting(Nodes.Waiting node);
    }
}
