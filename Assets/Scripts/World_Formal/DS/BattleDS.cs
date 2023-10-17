using AutoCode.Tables;
using Common;
using Common_Formal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World_Formal.Enemys;
using World_Formal.Helpers;
using World_Formal.UI;

namespace World_Formal.DS
{
    public class BattleDS
    {
        public AutoCode.Tables.Battle.Record battle_desc;
        public List<MonsterGroup.Record> monster_group_desc = new();
        public float caravan_pos;

        public float length => battle_desc.f_length;
        public float ratio => caravan_pos / length;
        public string percentage => $"{string.Format("{0:0.00}", ratio * 100)}%";

        WorldSceneRoot root;
        Vector2 m_tip_start;
        Vector2 m_tip_end;
        Dictionary<float, progress_monster_tip> m_monster_tips_dic = new();
        float m_next_monster_upcoming_ratio;

        EnemyMgr m_enemy_mgr;

        //==================================================================================================

        public BattleDS(uint battle_id)
        {
            DB.instance.battle.try_get(battle_id, out battle_desc);
            EX_Utility.try_load_asset("ui_Formal", "parts/progress_monster_tip", out progress_monster_tip model);
            root = WorldSceneRoot.instance;
            m_tip_start = root.battle_progress_start.localPosition;
            m_tip_end = root.battle_progress_end.localPosition;

            foreach (var (k, r) in DB.instance.monster_group_dic.Where(t => t.Key.Item1 == battle_desc.f_monster_group))
            {
                monster_group_desc.Add(r);

                var schedule = r.f_schedule;
                var schedule_pos = m_tip_start + (m_tip_end - m_tip_start) * schedule;

                var is_tip_active = r.f_battle_icon.Item1 != ""; //规则：如果时机没有icon，则不显示
                root.add_process_monster_tip(schedule_pos, model, is_tip_active, out var tip);
                if (!m_monster_tips_dic.ContainsKey(schedule))
                    m_monster_tips_dic.Add(schedule, tip);
            }

            m_monster_tips_dic = m_monster_tips_dic.OrderBy(e => e.Key).ToDictionary(e => e.Key, e => e.Value);
            m_next_monster_upcoming_ratio = m_monster_tips_dic.First().Key;

            Mission.instance.try_get_mgr(Config.EnemyMgr_Name, out m_enemy_mgr);
        }


        public void update_progress(WorldContext ctx)
        {
            caravan_pos += ctx.caravan_velocity.x * Config.PHYSICS_TICK_DELTA_TIME;
            root.update_battle_progress(ratio);

            //pass怪物节点
            if (m_monster_tips_dic.Any())
            {
                if (caravan_pos >= m_next_monster_upcoming_ratio * length)
                {
                    do_when_encounter_monster(m_next_monster_upcoming_ratio);

                    var tip = m_monster_tips_dic[m_next_monster_upcoming_ratio];
                    tip.set_passed(true);

                    m_monster_tips_dic.Remove(m_next_monster_upcoming_ratio);
                    if (m_monster_tips_dic.Any())
                        m_next_monster_upcoming_ratio = m_monster_tips_dic.First().Key;
                }
            }

            //到达终点，触发胜利
            if (caravan_pos >= length)
                Battle_In_Out_Helper.instance.win();
        }


        public void do_when_encounter_monster(float schedule)
        {
            //添加每一个符合时机的怪物群
            foreach (var r in monster_group_desc.Where(t => t.f_schedule == schedule))
            {
                var num = new System.Random().Next(r.f_num_min, 1 + r.f_num_max);
                while (num > 0)
                {
                    WorldSceneRoot.instance.add_enemy_per_random_time(() =>
                    {
                        m_enemy_mgr.add_cell(r.f_monster, new Vector2(r.f_pos_x, r.f_pos_y));
                    });
                    num--;
                }
            }
        }
    }
}

