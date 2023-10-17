namespace Common
{
    public class Singleton<T> where T : new()
    {
        public static T instance => m_instance ?? _init();
        static T m_instance;

        //==================================================================================================

        public static T _init()
        {
            m_instance = new();
            return m_instance;
        }
    }
}

