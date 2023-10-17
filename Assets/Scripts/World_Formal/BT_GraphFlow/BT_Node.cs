using GraphNode;
using System.Reflection;

namespace World_Formal.BT_GraphFlow
{
    [System.Serializable]
    public class BT_Node : Node
    {

        //================================================================================================

        public void get_method_info<T>(T t, string name, out MethodInfo method) where T : BT_Node
        {
            var type = t.GetType();
            method = type.GetMethod("do_self");

            var methods = type.GetMethods();
            foreach (var e in methods)
            {
                var display = e.GetCustomAttribute<DisplayAttribute>();
                if (display == null) continue;

                if (display.name == name)
                {
                    method = e;
                    break;
                }               
            }
        }


        public int get_method_seq<T>(T t, string name) where T : BT_Node
        {
            get_method_info(t ,name, out var method);

            var display = method.GetCustomAttribute<DisplayAttribute>();
            if (display == null)
                return 0;

            return display.seq;
        }


        public virtual void do_self(BT_Context ctx)
        {
        }


        public virtual void do_back(BT_Context ctx)
        {
        }
    }
}

