using Common;
using Foundation;
using System.Collections.Generic;
using World;

namespace Battle.HandCards
{
    public interface IHandCardView : IModelView<HandCardMgr>
    { 
        void notify_on_init(HandCard cell);
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
        }


        public void add_cell(HandCard cell, IHandCardView view)
        {
            m_cell_list.AddLast(cell);
            add_view(view);
            view.notify_on_init(cell);

            WorldContext.instance.bctx.handcard_count = m_cell_list.Count;
        }


        public void remove_cell(HandCard cell)
        {
            m_cell_list.Remove(cell);
        }
    }
}

