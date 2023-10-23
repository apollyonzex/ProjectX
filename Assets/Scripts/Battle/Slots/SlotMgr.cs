using Common;
using Foundation;
using System.Collections.Generic;

namespace Battle.Slots
{
    public interface ISlotView : IModelView<SlotMgr>
    { 
        void notify_on_init(Slot cell);
    }


    public class SlotMgr : Model<SlotMgr, ISlotView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        Dictionary<Slot, ISlotView> m_cell_dic = new();

        //==================================================================================================

        public SlotMgr(string name, params object[] objs)
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


        public void add_cell(Slot cell, ISlotView view)
        {
            m_cell_dic.Add(cell, view);
            add_view(view);
        }


        public void load_view()
        {
            foreach (var (cell, view) in m_cell_dic)
            {
                view.notify_on_init(cell);
            }
        }
    }
}

