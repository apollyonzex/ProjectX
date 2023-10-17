using Common_Formal.DS;
using System.Collections.Generic;

namespace Common_Formal.Helpers
{
    public class Spine_Load_Helper : Singleton<Spine_Load_Helper>
    {
        /// <summary>
        /// 加载动画
        /// </summary>
        public SpineDS @do(SpineDS ds, string name)
        {
            ds.name = name;
            if (DB.instance.spineAnimPrm.try_get(ds.name, out var prm))
            {
                ds.loop = prm.f_loop;
            }

            return ds;
        }


        /// <summary>
        /// 加载init动画 - dic
        /// </summary>
        public Dictionary<T, SpineDS> do_on_dic_init<T>(Dictionary<T, SpineDS> dic)
        {
            var temp_dic = new Dictionary<T, SpineDS>();
            foreach (var (key, ds) in dic)
            {
                temp_dic.Add(key, @do(ds, ds.init));
            }
            return temp_dic;
        }
    }
}

