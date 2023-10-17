using Common_Formal;
using AutoCode.Tables;
using System.Collections.Generic;
using World_Formal.Caravans;
using Common_Formal.DS;

namespace Camp
{
    public class DB : Singleton<DB>
    {
        World m_world;
        public World world
        {
            get
            {
                if (m_world == null)
                {
                    EX_Utility.try_load_table("world", out m_world);
                }

                return m_world;
            }
        }


        World m_world_without_running;
        public World world_without_running
        {
            get
            {
                if (m_world_without_running == null)
                {
                    EX_Utility.try_load_table_without_running("world", "world", out m_world_without_running);
                }

                return m_world_without_running;
            }
        }


        SpineCampDevice m_spineCampDevice;
        public SpineCampDevice spineCampDevice
        {
            get
            {
                if (m_spineCampDevice == null)
                {
                    EX_Utility.try_load_table("spine_camp_device", out m_spineCampDevice);
                }
                return m_spineCampDevice;
            }
        }

        public Dictionary<uint, SpineDS> spineCampDevice_info
        {
            get
            {
                Dictionary<uint, SpineDS> dic = new();
                foreach (var e in spineCampDevice.records)
                {
                    SpineDS ds = new()
                    {
                        name = e.f_init,
                        init = e.f_init,
                    };
                    dic.Add(e.f_id, ds);
                }
                return dic;
            }       
        }


        SpineCampCaravan m_spineCampCaravan;
        public SpineCampCaravan spineCampCaravan
        {
            get
            {
                if (m_spineCampCaravan == null)
                {
                    EX_Utility.try_load_table("spine_camp_caravan", out m_spineCampCaravan);
                }
                return m_spineCampCaravan;
            }
        }

        public Dictionary<uint, SpineDS> spineCampCaravan_info
        {
            get
            {
                Dictionary<uint, SpineDS> dic = new();
                foreach (var e in spineCampCaravan.records)
                {
                    SpineDS ds = new()
                    {
                        name = e.f_init,
                        init = e.f_init,
                    };
                    dic.Add(e.f_id, ds);
                }
                return dic;
            }
        }
    }
}

