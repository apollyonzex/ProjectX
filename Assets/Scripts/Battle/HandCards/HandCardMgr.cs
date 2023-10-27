using Common;
using Foundation;
using System;
using System.Collections.Generic;
using World;

namespace Battle.HandCards
{
    public interface IHandCardView : IModelView<HandCardMgr>
    { 
        void notify_on_init(HandCard cell);
        void notify_on_tick1();
    }


    public class HandCardMgr : Model<HandCardMgr, IHandCardView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        LinkedList<HandCard> m_cell_list = new();

        //==================================================================================================

        public HandCardMgr(string name, params object[] objs)
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

            var ctx = WorldContext.instance;
            ctx.add_tick(Config.HandCardMgr_Priority, Config.HandCardMgr_Name, tick);
            ctx.add_tick1(Config.HandCardMgr_Priority, Config.HandCardMgr_Name, tick1);
        }


        void tick()
        {
            
        }


        void tick1()
        {
            foreach (var view in views)
            {
                view.notify_on_tick1();
            }
        }


        public void add_cell(HandCard cell, IHandCardView view)
        {
            m_cell_list.AddLast(cell);

            view.notify_on_init(cell);
            add_view(view);

            WorldContext.instance.bctx.handcard_count = m_cell_list.Count;
        }


        public void remove_cell(HandCard cell)
        {
            m_cell_list.Remove(cell);
        }


        public void play(HandCard cell)
        { 
            
        }
    }
}

