namespace Foundation {

    public interface IState<T> where T : class, IState<T> {
        void enter(T last);
        void leave();
    }

    public interface IStateMachine<T> where T : class, IState<T> {
        T current { get; }
        T next { get; set; }
    }

    public class StateMachine<T> : IStateMachine<T> where T : class, IState<T> {

        public bool locked {
#if CSHARP_7_3_OR_NEWER
            get => m_locked;
#else
            get { return m_locked; }
#endif
            set {
                if (m_locked != value) {
                    if (!(m_locked = value)) {
                        check_next_state();
                    }
                }
            }
        }

        public T current => m_current;
        public T next {
#if CSHARP_7_3_OR_NEWER
            get => m_next;
#else
            get { return m_next; }
#endif
            set {
                if (m_current == value) {
                    m_next = null;
                    return;
                }
                m_next = value;
                if (!m_locked) {
                    check_next_state();
                }
            }
        }

        private void check_next_state() {
            while (m_next != null) {
                m_current?.leave();
                var last = m_current;
                m_current = m_next;
                m_next = null;
                m_current?.enter(last);
            }
        }

        protected T m_current, m_next;
        protected bool m_locked = false;

    }
}