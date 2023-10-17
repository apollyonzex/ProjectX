using Common_Formal;
using System.Collections.Generic;

namespace World_Formal.Helpers
{
    public class Battle_Reward_Helper : Singleton<Battle_Reward_Helper>
    {
        public bool try_create_reward(out List<(uint, uint)> rewards)
        {
            AutoCode.Tables.Contents.Record record = null;
            var ctx = WorldContext.instance;
            rewards = new();

            foreach (var rec in DB.instance.contents.records)
            {
                if (rec.f_id == ctx.world_id && ctx.journey_difficult < rec.f_journey_difficult)            //配表时需要保证从上到下是从小到大
                {
                    record = rec;
                    break;
                }
            }
            if (record == null)
            {
                UnityEngine.Debug.Log($"未找到匹配的world_id {ctx.world_id}  journey_difficult{ctx.journey_difficult}");
                return false;
            }

            var reward_standard = record.f_reward_standard;

            for (int i = 0; i < reward_standard.Item1; i++)
            {
                bool drop = UnityEngine.Random.Range(1, 100) < reward_standard.Item2;

                if (drop)
                {
                    var random_index = UnityEngine.Random.Range(0, reward_standard.Item3.Length);
                    var d = reward_standard.Item3[random_index];

                    var sum = 0;
                    foreach (var pro in reward_standard.Item4)
                    {
                        sum += pro.value;
                    }
                    var probability = UnityEngine.Random.Range(0, sum + 1);
                    uint rank = 0;
                    foreach (var pro in reward_standard.Item4)
                    {
                        if (probability > pro.value)
                        {
                            probability -= pro.value;
                        }
                        else
                        {
                            rank = pro.key;
                            break;
                        }
                    }

                    rewards.Add((d, rank));
                }
            }

            return true;
        }



    }
}

