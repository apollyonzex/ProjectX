
namespace Foundation {

    public class Game : MonoBehaviourSingleton<Game> {

        public interface IState : IState<IState> {
            void update();
            void fixed_update();
            void late_update();
        }

        public class State : IState {
            public virtual void enter(IState last) {
                
            }

            public virtual void leave() {
                
            }

            public virtual void update() {

            }

            public virtual void fixed_update() {

            }

            public virtual void late_update() {

            }
        }

        public IStateMachine<IState> state => m_state;

        public void lock_state() {
            if (++m_lock_count == 1) {
                m_state.locked = true;
            }
        }

        public void unlock_state() {
            if (--m_lock_count == 0) {
                m_state.locked = false;
            }
        }

        private void Update() {
            lock_state();
            m_state.current?.update();
        }

        private void FixedUpdate() {
            m_state.current?.fixed_update();
        }

        private void LateUpdate() {
            m_state.current?.late_update();
            unlock_state();
        }


        private StateMachine<IState> m_state = new StateMachine<IState>();
        private int m_lock_count = 0;
    }

}