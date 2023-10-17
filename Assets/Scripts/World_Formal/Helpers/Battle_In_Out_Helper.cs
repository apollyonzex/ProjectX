using Common;
using Common_Formal;
using System;
using System.Collections.Generic;
using System.Linq;
using World_Formal.CaravanBackPack;
using World_Formal.Enemys;
using World_Formal.WareHouse;

namespace World_Formal.Helpers
{
    public class Battle_In_Out_Helper : Common_Formal.Singleton<Battle_In_Out_Helper>
    {
        WorldContext m_ctx;
        WorldSceneRoot m_root;

        WorldContext ctx => m_ctx ?? WorldContext.instance;
        WorldSceneRoot root => m_root ?? WorldSceneRoot.instance;

        Action<bool> m_callback;

        //==================================================================================================

        public void start_battle(Action<bool> callback, bool is_direct_start = false)
        {
            ctx.is_battle = true;
            m_callback = callback;

            if (is_direct_start)
                ctx.is_battle_start_directly = true;

            //临时按钮 - 快速胜利/投降，用于测试
            root.btn_win.onClick.AddListener(win);
            root.btn_defeat.onClick.AddListener(defeat);
            root.load_ui();

            select_battle();
            Caravan_Move_Helper.instance.move(ctx);

            Mission.instance.try_get_mgr(Common.Config.DeviceMgr_Name, out Caravans.Devices.DeviceMgr deviceMgr);
            deviceMgr.start_battle();
        }


        public void end_battle(bool is_win)
        {
            ctx.is_battle = false;

            root.load_ui();

            m_callback?.Invoke(is_win);
            m_callback = null;

            clear_battle();
            Caravan_Move_Helper.instance.stop(ctx);

            //清除按钮事件
            root.btn_win.onClick.RemoveAllListeners();
            root.btn_defeat.onClick.RemoveAllListeners();
        }


        public void win()
        {
            end_battle(true);

            //发放奖励
            root.win_reward._active();
        }


        public void defeat()
        {
            end_battle(false);
        }


        void select_battle()
        {
            var journey_difficult = ctx.journey_difficult;
            var world_id = ctx.world_id;

            //规则：选取表中所有不大于当前[旅途难度]的journey_difficulty值的最大值 
            Dictionary<uint, int> sub_dic = new();
            foreach (var (id,d) in DB.instance.content_dic.Keys)
            {
                if (id == world_id)
                {
                    int sub = (int)d - (int)journey_difficult;
                    if (sub > 0) continue;
                    sub_dic.Add(d, -sub);
                }
            }
            sub_dic = sub_dic.OrderBy(e => e.Value).ToDictionary(e => e.Key, e => e.Value);
            journey_difficult = sub_dic.FirstOrDefault().Key;

            //难度选取完成，开始加载数据
            DB.instance.contents.try_get(world_id, journey_difficult, out var r);
            List<uint> temp = new();
            var f_battle = r.f_battle;
            ref var played = ref ctx.played_battles;

            foreach (var e in f_battle)
            {
                if (played.ContainsKey(e)) continue;
                temp.Add(e);
            }

            uint selected_id;
            if (temp.Any()) //规则：如果有一些battle从未选过，则在该池子中等概率选取
            {
                var index = new System.Random().Next(0, temp.Count);
                selected_id = temp[index];
                played.Add(selected_id, 1);
                UnityEngine.Debug.Log($"选取未玩battle。进入battle：{selected_id}");
            }
            else //规则：如果所有battle都已玩过至少一次，则等概率选取
            {
                var index = new System.Random().Next(0, f_battle.Length);
                selected_id = f_battle[index];
                played[selected_id] ++;
                UnityEngine.Debug.Log($"已玩过该难度所有battle。进入battle：{selected_id}");
            }

            ctx.battleDS = new(selected_id);
        }


        void clear_battle()
        {
            Mission.instance.try_get_mgr(Config.EnemyMgr_Name, out EnemyMgr enemy_mgr);
            enemy_mgr.remove_all_cell();

            ctx.battleDS = null;
            ctx.is_battle_start_directly = false;
            ctx.be_holded_enemy_count = 0;
        }
    }
}

