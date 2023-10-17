using Common_Formal;
using Foundation;
using System.Collections.Generic;

namespace Camp.Level_Entrances
{
    public interface ILevel_EntranceView : IModelView<Level_EntranceMgr>
    { 
        
    }


    public class Level_EntranceMgr : Model<Level_EntranceMgr, ILevel_EntranceView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        Dictionary<string, (Level_Entrance, Level_EntranceView)> m_info_string_indice = new();

        //==================================================================================================

        public Level_EntranceMgr(string name, params object[] objs)
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


        public void add_cell(string key, Level_Entrance cell, Level_EntranceView view)
        {
            (Level_Entrance, Level_EntranceView) value = new(cell, view);
            EX_Utility.dic_cover_add(ref m_info_string_indice, key, value);
        }


        public string get_key(Level_Entrance cell)
        {
            return get_key(cell.world_id, cell.seq);
        }


        public string get_key(uint world_id, uint seq)
        {
            return $"{world_id}-{seq}";
        }


        public bool try_get_info(string key, out Level_Entrance cell, out Level_EntranceView view)
        {
            cell = null;
            view = null;
            if (!m_info_string_indice.TryGetValue(key, out var v))
                return false;

            cell = v.Item1;
            view = v.Item2;
            return true;
        }
    }
}

