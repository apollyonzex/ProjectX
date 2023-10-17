using Common;
using Common_Formal;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World_Formal.Enemys
{
    public interface IEnemyView : IModelView<EnemyMgr>
    {
        void notify_on_init(Enemy cell);

        void notify_on_tick1();

        void notify_on_destory();
    }


    public class EnemyMgr : Model<EnemyMgr, IEnemyView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public WorldContext ctx;

        Dictionary<Enemy, IEnemyView> m_cell_dic = new();
        List<Enemy> m_del_cell_list = new();

        public int catching_layer;
        public int catching_mask;

        //==================================================================================================

        public EnemyMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);

            catching_layer = LayerMask.NameToLayer("catching_monster");
            catching_mask = LayerMask.GetMask("catching_monster");
        }


        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }


        void IMgr.init(object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            ctx = WorldContext.instance;
            ctx.add_tick(Config.EnemyMgr_Priority, Config.EnemyMgr_Name, tick);
            ctx.add_tick1(Config.EnemyMgr_Priority, Config.EnemyMgr_Name, tick1);
        }


        void tick()
        {
            if (!ctx.is_battle) return;

            foreach (var (cell, _) in m_cell_dic)
            {
                cell.tick();

                //如果需要，重置pos
                if (ctx.is_need_reset)
                {
                    cell.bctx.position.x -= ctx.reset_dis;
                }
            }

            if (!m_del_cell_list.Any()) return;
            foreach (var cell in m_del_cell_list)
            {
                remove_cell(cell);
            }
            m_del_cell_list.Clear();
        }


        void tick1()
        {
            if (!ctx.is_battle) return;

            foreach (var view in views)
            {
                view.notify_on_tick1();
            }
        }


        public void add_cell(uint id, Vector2 pos)
        {
            Enemy cell = new(this, id, pos);
            EX_Utility.create_cell_in_scene<EnemyMgr, IEnemyView, EnemyView>(this, cell._desc.f_view, WorldSceneRoot.instance.monster_node, out var view);
            (view as IEnemyView).notify_on_init(cell);
           
            m_cell_dic.Add(cell, view);
        }


        public void remove_cell(Enemy cell)
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


        public void change_layer(Enemy enemy, int layer)
        {
            m_cell_dic.TryGetValue(enemy, out var view);
            (view as EnemyView).change_Layer(layer);
        }


        /// <summary>
        /// 将cell添加到删除列表
        /// </summary>
        public void set_del(Enemy cell)
        {
            m_del_cell_list.Add(cell);
        }
    }
}

