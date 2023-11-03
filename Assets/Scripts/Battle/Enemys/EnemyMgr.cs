using Common;
using Foundation;
using System;
using System.Collections.Generic;
using World;

namespace Battle.Enemys
{
    public interface IEnemyView : IModelView<EnemyMgr>
    { 
        void notify_on_init(Enemy cell);
    }


    public class EnemyMgr : Model<EnemyMgr, IEnemyView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public Dictionary<uint, Enemy> cell_dic = new();

        //==================================================================================================

        public EnemyMgr(string name, params object[] objs)
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
            WorldContext.instance.add_tick(Config.EnemyMgr_Priority, Config.EnemyMgr_Name, tick);
        }


        void tick()
        {
            //foreach (var (_,cell) in cell_dic)
            //{
            //    cell.bctx.tick();
            //}
        }


        public void add_cell(Enemy cell, IEnemyView view)
        {
            cell_dic.Add(cell.id, cell);

            //add_view(view);
            //view.notify_on_init(cell);
        }
    }
}

