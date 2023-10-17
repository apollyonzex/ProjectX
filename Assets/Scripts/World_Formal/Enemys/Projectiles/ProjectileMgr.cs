using Common_Formal;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World_Formal.DS;

namespace World_Formal.Enemys.Projectiles
{
    public interface IProjectileView : IModelView<ProjectileMgr>
    {
        void notify_on_tick1();
        void notify_on_init(Projectile cell);
        void notify_on_destory();
    }


    public class ProjectileMgr : Model<ProjectileMgr, IProjectileView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public WorldContext ctx;

        Dictionary<Projectile, IProjectileView> m_cell_dic = new();
        List<Projectile> m_del_cell_list = new();

        //==================================================================================================

        public ProjectileMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }


        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }


        void IMgr.init(object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            ctx = WorldContext.instance;
            ctx.add_tick(Common.Config.ProjectileMgr_Enemy_Priority, Common.Config.ProjectileMgr_Enemy_Name, tick);
            ctx.add_tick1(Common.Config.ProjectileMgr_Enemy_Priority, Common.Config.ProjectileMgr_Enemy_Name, tick1);
        }


        int time = 120; //临时
        void tick()
        {
            if (!ctx.is_battle) return;

            foreach (var (cell, _) in m_cell_dic)
            {
                cell.tick();

                //如果需要，重置pos
                if (ctx.is_need_reset)
                {
                    cell.position.x -= ctx.reset_dis;
                }
            }

            if (m_del_cell_list.Any())
            {
                foreach (var cell in m_del_cell_list)
                {
                    remove_cell(cell);
                }
                m_del_cell_list.Clear();
            }

            time--;
            if (time <= 0)
                time = 120;
        }


        void tick1()
        {
            if (!ctx.is_battle) return;

            foreach (var view in views)
            {
                view.notify_on_tick1();
            }
        }


        public bool try_add_cell(uint id, Vector2 pos, Vector2 dir, float v, ITarget target, out Projectile cell)
        {
            cell = null;
            if (time % 60 != 0) return false; //临时

            cell = new(this, id, pos, dir, v, target);
            EX_Utility.create_cell_in_scene<ProjectileMgr, IProjectileView, ProjectileView>(this, cell._desc.f_prefab, WorldSceneRoot.instance.projectiles_root, out var view);
            (view as IProjectileView).notify_on_init(cell);

            m_cell_dic.Add(cell, view);
            return true;
        }


        public void remove_cell(Projectile cell)
        {
            var view = m_cell_dic[cell];
            view.notify_on_destory();
            remove_view(view);

            m_cell_dic.Remove(cell);
        }


        public void remove_all_cell()
        {
            foreach (var (cell, view) in m_cell_dic)
            {
                view.notify_on_destory();
                remove_view(view);
            }

            m_cell_dic.Clear();
        }


        public void set_del(Projectile cell)
        {
            m_del_cell_list.Add(cell);
        }
    }
}

