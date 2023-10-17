using System.Collections.Generic;

namespace Common_Formal
{
    public class Mission : Singleton<Mission>
    {
        Dictionary<string, IMgr> m_mgrs_name_indice = new();

        //==================================================================================================

        public void attach_mgr(string name, IMgr mgr)
        {
            EX_Utility.dic_cover_add(ref m_mgrs_name_indice, name, mgr);
        }


        public void detach_mgr(string name)
        {
            if (!m_mgrs_name_indice.ContainsKey(name)) return;
            m_mgrs_name_indice.Remove(name);
        }


        public bool try_get_mgr(string name, out IMgr mgr)
        {
            mgr = null;
            if (!m_mgrs_name_indice.ContainsKey(name)) return false;

            mgr = m_mgrs_name_indice[name];
            return true;
        }


        public bool try_get_mgr<T>(string name, out T mgr) where T : class, IMgr
        {
            mgr = null;
            if (!try_get_mgr(name, out var imgr)) return false;

            mgr = imgr as T;
            return true;
        }


        public void remove_all_mgr()
        {
            m_mgrs_name_indice.Clear();
        }
    }
}

