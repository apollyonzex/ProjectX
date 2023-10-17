using Foundation;

namespace Common_Formal
{
    public interface IMgr
    {
        public string name { get; }
        void init(params object[] objs);
        void destroy();
    }
}

