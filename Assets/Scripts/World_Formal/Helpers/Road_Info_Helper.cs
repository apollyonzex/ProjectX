using Common;
using Common_Formal;
using UnityEngine;
using World_Formal.Enviroment;
using World_Formal.Enviroment.Road;

namespace World_Formal.Helpers
{
    public class Road_Info_Helper
    {
        static RoadMgr m_roadMgr;
        static RoadMgr roadMgr
        {
            get
            {
                if (m_roadMgr == null)
                {
                    Mission.instance.try_get_mgr(Common.Config.EnviromentMgr_Name, out EnviromentMgr emgr);
                    m_roadMgr = emgr.roadMgr;
                }
                return m_roadMgr;
            }
        }

        //==================================================================================================

        /// <summary>
        /// 获取高度
        /// </summary>
        public static float try_get_altitude(float x)
        {
            if (try_get_altitude(x, out var y))
                return y;

            return 0;
        }


        /// <summary>
        /// 获取高度
        /// </summary>
        public static bool try_get_altitude(float x, out float y)
        {
            y = 0;

            if (roadMgr == null)
                return false;

            y = roadMgr.road_height(x);
            return true;
        }


        /// <summary>
        /// 获取斜率
        /// </summary>
        public static bool try_get_slope(float x, out float slope)
        {
            slope = 0;

            if (roadMgr == null)
                return false;

            slope = roadMgr.road_slope(x);
            return true;
        }


        /// <summary>
        /// 获取路面倾斜弧度
        /// </summary>
        public static bool try_get_leap_rad(float x, out float rad)
        {
            rad = 0;
            if (try_get_slope(x, out float slope))
            {
                rad = Mathf.Atan(slope);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 获取路面凹凸（二阶导）
        /// </summary>
        public static bool try_get_concavity(float x, out float value)
        {
            value = 0;

            if (roadMgr == null)
                return false;

            value = roadMgr.road_bump(x);
            return true;
        }


        /// <summary>
        /// 获取路面曲率半径
        /// </summary>
        public static bool try_get_ground_p(float x, out float value)
        {
            value = 0;

            if (roadMgr == null)
                return false;

            value = roadMgr.road_radius(x);
            return true;
        }


        public static void @reset()
        {
            m_roadMgr = null;
        }
    }
}

