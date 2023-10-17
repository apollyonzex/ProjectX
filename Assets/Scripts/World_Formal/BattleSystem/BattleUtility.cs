using System.Collections.Generic;
using UnityEngine;
using Common_Formal;
using World_Formal.DS;
namespace World_Formal.BattleSystem
{
    public static class BattleUtility
    {
        public static Collider2D[] results = new Collider2D[256];


        private static void sort_results(int turn, Vector2 center)
        {

            System.Array.Sort(results, (c1, c2) => {
                if (c1 == null && c2 == null)
                {
                    return 0;
                }
                if (c1 == null)
                {
                    return 1;
                }
                if (c2 == null)
                {
                    return -1;
                }
                Vector2 pos_i = c1.transform.position;
                Vector2 pos_j = c2.transform.position;
                var dis_i = (pos_i - center).magnitude;
                var dis_j = (pos_j - center).magnitude;
                // return (int)(dis_i - dis_j);          不用这个 0.7 = 0

                var result = 0;
                if (dis_i < dis_j)
                {
                    result = -1;
                }
                else if (dis_i > dis_j)
                {
                    result = 1;
                }
                return result;
            });
        }
        /// <summary>
        /// 寻找中心圆范围内的最近的(阵容不同)有效目标
        /// </summary>
        public static ITarget select_target_in_circle(Vector2 center, float radius, Enum.EN_faction faction)
        {
            var turn = Physics2D.OverlapCircleNonAlloc(center, radius, results);

            sort_results(turn, center);

            if (faction == Enum.EN_faction.player)
            {
                for (int i = 0; i < turn; i++)
                {
                    if (results[i].TryGetComponent<Enemys.EnemyView>(out var ev))
                    {
                        return ev.cell;
                    }
                }
            }
            else if (faction == Enum.EN_faction.enemy)
            {
                for (int i = 0; i < turn; i++)
                {
                                                                                                                  
                }
            }
            return null;
        }
        /// <summary>
        /// 寻找中心圆范围内的所有有效目标
        /// </summary>
        public static List<ITarget> select_all_target_in_circle(Vector2 center, float radius, Enum.EN_faction faction)
        {
            var turn = Physics2D.OverlapCircleNonAlloc(center, radius, results);
            if (turn == 0)
                return new List<ITarget>();

            List<ITarget> targets = new List<ITarget>();
            if (faction == Enum.EN_faction.player)
            {
                for (int i = 0; i < turn; i++)
                {
                    if (results[i].TryGetComponent<Enemys.EnemyView>(out var ev))
                    {
                        targets.Add(ev.cell);
                    }
                }
            }
            else if (faction == Enum.EN_faction.enemy)
            {
                for (int i = 0; i < turn; i++)
                {

                }
            }
            return targets;
        }
    }
}
