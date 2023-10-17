using Devices;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Worlds.Missions.Battles
{
    public static class Utility
    {

        public static Collider2D[] results = new Collider2D[256];


        private static void sort_results(int turn,Vector2 center) {

            Array.Sort(results, (c1,c2) => {
                if (c1 == null && c2 == null) {
                    return 0;
                }
                if (c1 == null) {
                    return 1;
                }
                if(c2 == null) {
                    return -1;
                }
                Vector2 pos_i = c1.transform.position;
                Vector2 pos_j = c2.transform.position;
                var dis_i = (pos_i - center).magnitude;
                var dis_j = (pos_j - center).magnitude;
                // return (int)(dis_i - dis_j);          不用这个 0.7 = 0

                var result = 0;
                if (dis_i < dis_j) { 
                    result = -1; 
                }
                else if(dis_i> dis_j) {
                    result = 1;
                }
                return result;
            });
        }
        public static ITarget select_target_in_circle<T>(Vector2 center,float radius,Device.Faction faction){
            var turn = Physics2D.OverlapCircleNonAlloc(center, radius, results);

            sort_results(turn, center);

            for (int i = 0; i < turn; i++) {
                if (results[i].TryGetComponent<T>(out var view)) {           //暂定
                    if (view is Projectiles.IProjectileView pv) {
                        if (pv.cell.ctx.device.faction!=faction)
                            return pv.cell;
                    }
                }
            }
            return null;
        }

        public static ITarget select_target_in_fan(Vector2 center,float radius,Vector2 axis,float angle,Device.Faction faction) {

            var turn = Physics2D.OverlapCircleNonAlloc(center, radius, results);

            sort_results(turn, center);

            if (faction == Device.Faction.player) {
                for (int i = 0; i < turn; i++) {

                    var r = results[i].transform.position;
                    var dir = new Vector2(r.x,r.y) - center;

                    if (UnityEngine.Vector2.Angle(axis, dir) < angle) {
                        if (results[i].TryGetComponent<Enemies.IEnemyView>(out var view)) {           //暂定
                            if (view.cell.is_dead)
                                continue;
                            return view.cell;
                        }
                    }
                }
            } else if (faction == Device.Faction.enemy) {
                for (int i = 0; i < turn; i++) {

                    var r = results[i].transform.position;
                    var dir = new Vector2(r.x, r.y) - center;

                    if (UnityEngine.Vector2.Angle(axis, dir) < angle) {
                        if (results[i].TryGetComponent<Caravan.IBattleCaravanView>(out var view)) {
                            return view.cell;            //player
                        }
                    }        
                }
            }

            return null;
        }

        public static ITarget select_target_in_circle(Vector2 center, float radius, Device.Faction faction)
        {
            var turn = Physics2D.OverlapCircleNonAlloc(center, radius, results);

            sort_results(turn, center);

            if (faction == Device.Faction.player)
            {
                for (int i = 0; i < turn; i++)
                {
                    if (results[i].TryGetComponent<Enemies.IEnemyView>(out var view))
                    {           //暂定
                        if (view.cell.is_dead)
                            continue;
                        return view.cell;
                    }
                }
            }
            else if (faction == Device.Faction.enemy)
            {
                for (int i = 0; i < turn; i++)
                {
                    if (results[i].TryGetComponent<Caravan.IBattleCaravanView>(out var view))
                    {
                        return view.cell;            //player
                    }                                                                                               //勉强可以
                }
            }
            return null;
        }


        public static List<ITarget> select_all_target_in_circle(Vector2 center, float radius, Device.Faction faction)
        {
            var turn = Physics2D.OverlapCircleNonAlloc(center, radius, results);
            List<ITarget> targets = new List<ITarget>();
            if (faction == Device.Faction.player)
            {
                for (int i = 0; i < turn; i++)
                {
                    if (results[i].TryGetComponent<Enemies.IEnemyView>(out var view))           //需要修改
                    {           //暂定
                        if (view.cell.is_dead)
                            continue;
                        targets.Add(view.cell);
                    }
                    if (results[i].TryGetComponent<Floor>(out var f))
                    {           //暂定
                        targets.Add(f);
                    }
                    if (results[i].TryGetComponent<Projectiles.IProjectileView>(out var p)) {
                        if(p.cell.ctx.device.faction == faction) {
                            continue;
                        }
                        targets.Add(p.cell);
                    }
                }
            }
            else if (faction == Device.Faction.enemy)
            {
                for (int i = 0; i < turn; i++)
                {
                    if (results[i].TryGetComponent<Caravan.IBattleCaravanView>(out var view))
                    {
                        targets.Add(view.cell);                                               //player
                    }
                    if (results[i].TryGetComponent<Floor>(out var f))
                    {           //暂定
                        targets.Add(f);
                    }
                    if (results[i].TryGetComponent<Projectiles.IProjectileView>(out var p)) {
                        if (p.cell.ctx.device.faction == faction) {
                            continue;
                        }
                        targets.Add(p.cell);
                    }
                }
            }
            return targets;
        }


        public static Quaternion quick_rotate(Vector2 v)
        {
            return Quaternion.LookRotation(Vector3.forward, new Vector3(-v.y, v.x));
        }


        /// <summary>
        /// 双方的collider都为矩形时使用
        /// 判断是否接触
        /// </summary>
        public static bool check_contain_two_rectangleArea(Vector2 self_pos, Vector2 self_collider, Vector2 target_pos, Vector2 target_collider)
        {
            var self_collider_half = self_collider / 2;
            var target_collider_half = target_collider / 2;   

            float t_x1 = target_pos.x - target_collider_half.x;
            float t_x2 = target_pos.x + target_collider_half.x;
            float t_y1 = target_pos.y - target_collider_half.y;
            float t_y2 = target_pos.y + target_collider_half.y;

            float s_x1 = self_pos.x - self_collider_half.x;
            float s_x2 = self_pos.x + self_collider_half.x;
            float s_y1 = self_pos.y - self_collider_half.y;
            float s_y2 = self_pos.y + self_collider_half.y;

            var s_area = self_collider.x * self_collider.y;
            var t_area = target_collider.x * target_collider.y;

            bool bl_x;
            bool bl_y;

            if (s_area <= t_area)
            {
                bl_x = ((s_x1 >= t_x1) && (s_x1 <= t_x2)) || ((s_x2 >= t_x1) && (s_x2 <= t_x2));            
                bl_y = ((s_y1 >= t_y1) && (s_y1 <= t_y2)) || ((s_y2 >= t_y1) && (s_y2 <= t_y2));
            }
            else 
            {
                bl_x = ((t_x1 >= s_x1) && (t_x1 <= s_x2)) || ((t_x2 >= s_x1) && (t_x2 <= s_x2));
                bl_y = ((t_y1 >= s_y1) && (t_y1 <= s_y2)) || ((t_y2 >= s_y1) && (t_y2 <= s_y2));
            }

            return bl_x && bl_y;
        }
    }

}

