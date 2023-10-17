using AutoCode.Tables;

namespace Common_Formal
{
    public class DB : Singleton<DB>
    {
        StringLz m_string_lz;
        public StringLz string_lz
        {
            get
            {
                if (m_string_lz == null)
                {
                    EX_Utility.try_load_table("string_lz", out m_string_lz);
                }

                return m_string_lz;
            }
        }


        UintLz m_uint_lz;
        public UintLz uint_lz
        {
            get
            {
                if (m_uint_lz == null)
                {
                    EX_Utility.try_load_table("uint_lz", out m_uint_lz);
                }

                return m_uint_lz;
            }
        }


        StringLz m_string_lz_without_running;
        public StringLz string_lz_without_running
        {
            get
            {
                if (m_string_lz_without_running == null)
                {
                    EX_Utility.try_load_table_without_running("string_lz", "string_lz", out m_string_lz_without_running);
                }

                return m_string_lz_without_running;
            }
        }


        UintLz m_uint_lz_without_running;
        public UintLz uint_lz_without_running
        {
            get
            {
                if (m_uint_lz_without_running == null)
                {
                    EX_Utility.try_load_table_without_running("uint_lz", "uint_lz", out m_uint_lz_without_running);
                }

                return m_uint_lz_without_running;
            }
        }


        Level m_level;
        public Level level {
            get {
                if (m_level == null) {
                    EX_Utility.try_load_table("level", out m_level);
                }
                return m_level; 
            }
        }


        SceneResource m_sceneresource;
        public  SceneResource sceneresource {
            get {
                if(m_sceneresource == null) {
                    EX_Utility.try_load_table("scene_resource", out m_sceneresource);
                }
                return m_sceneresource;
            }
        }


        SpineAnimPrm m_spineAnimPrm;
        public SpineAnimPrm spineAnimPrm
        {
            get
            {
                if (m_spineAnimPrm == null)
                {
                    EX_Utility.try_load_table("spine_anim_prm", out m_spineAnimPrm);
                }
                return m_spineAnimPrm;
            }
        }
    }

}

