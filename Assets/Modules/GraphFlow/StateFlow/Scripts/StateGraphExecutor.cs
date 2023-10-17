
using System.Collections.Generic;

namespace StateFlow {
    public class StateGraphExecutor {

        public StateGraph graph => m_graph_current;

        public void set_next_state(IContext context, StateNodeBase state) {
            if (m_leaving || m_graph_current == null || m_next == Next.Graph) {
                return;
            }
            m_next = Next.State;
            m_graph_next = null;
            m_state_next = state;
            if (!m_running) {
                using (var guard = new RunningGuard(context, this)) {
                    do_next(context);
                }
            }
        }

        public void set_next_graph(IContext context, StateGraph graph) {
            if (m_leaving) {
                return;
            }
            m_next = Next.Graph;
            m_graph_next = graph;
            m_state_next = null;
            if (!m_running) {
                using (var guard = new RunningGuard(context, this)) {
                    do_next(context);
                }
            }
        }

        public void tick(IContext context) {
            if (m_running || m_state_current == null) {
                return;
            }
            using (var guard = new RunningGuard(context, this)) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                using (new InvokeFlow.Recorder.GraphInvoke(context, m_graph_current))
#endif
                {
                    m_state_current.do_tick(context);
                }
                if (m_next != Next.None) {
                    do_next(context);
                }
            }
        }

        public bool rise_event(IContext context, string name, params StateEventParam[] parameters) {
            if (m_running) {
                m_events.Enqueue((name, parameters));
                return false;
            }
            if (m_state_current != null) {
                using (var guard = new RunningGuard(context, this)) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                    using (new InvokeFlow.Recorder.GraphInvoke(context, m_graph_current))
#endif
                    {
                        m_state_current.rise_event(context, name, parameters);
                    }
                    if (m_next != Next.None) {
                        do_next(context);
                    }
                }
            }
            return true;
        }

        StateNodeBase m_state_current;
        StateNodeBase m_state_next;
        StateGraph m_graph_current;
        StateGraph m_graph_next;
        Next m_next;
        bool m_running;
        bool m_leaving;

        enum Next {
            None,
            State,
            Graph,
        }

        struct RunningGuard : System.IDisposable {
            public IContext context;
            public StateGraphExecutor owner;
            public StateGraphExecutor previous;

            public RunningGuard(IContext context, StateGraphExecutor owner) {
                this.context = context;
                this.owner = owner;
                owner.m_running = true;
                previous = context.executor;
                context.executor = owner;
            }

            public void Dispose() {
                if (owner != null) {
                    owner.rise_events(context);
                    owner.m_running = false;
                    context.executor = previous;
                }
            }
        }

        struct LeavingGuard : System.IDisposable {
            public StateGraphExecutor owner;

            public LeavingGuard(StateGraphExecutor owner) {
                this.owner = owner;
                owner.m_leaving = true;
            }

            public void Dispose() {
                if (owner != null) {
                    owner.m_leaving = false;
                    owner.m_state_current = null;
                }
            }
        }

        void do_next(IContext context) {
        l_next:
            if (m_state_current != null) {
                using (var leaving = new LeavingGuard(this)) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                    using (new InvokeFlow.Recorder.GraphInvoke(context, m_graph_current))
#endif
                    {
                        m_state_current.do_leave(context);
                    }
                }
            }
            var next = m_next;
            m_next = Next.None;
            switch (next) {
                case Next.State:
                    m_state_current = m_state_next;
                    m_state_next = null;
                    if (m_state_current != null) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                        using (new InvokeFlow.Recorder.GraphInvoke(context, m_graph_current))
#endif
                        {
                            m_state_current.do_enter(context);
                        }
                    } else if (m_graph_current != null) {
                        m_graph_current.fini_stack(context);
                        m_graph_current = null;
                    }
                    break;
                case Next.Graph:
                    if (m_graph_current != null) {
                        m_graph_current.fini_stack(context);
                    }
                    m_graph_current = m_graph_next;
                    m_graph_next = null;
                    if (m_graph_current != null) {
                        m_graph_current.init_stack(context);
                        m_state_current = m_graph_current.entry;
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                        using (new InvokeFlow.Recorder.GraphInvoke(context, m_graph_current))
#endif
                        {
                            m_state_current.do_enter(context);
                        }
                    }
                    break;
            }
            if (m_next != Next.None) {
                goto l_next;
            }
        }

        void rise_events(IContext context) {
        l_loop:
            if (m_state_current == null) {
                m_events.Clear();
                return;
            }
            if (m_events.Count == 0) {
                return;                
            }
            var ev = m_events.Dequeue();
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new InvokeFlow.Recorder.GraphInvoke(context, m_graph_current))
#endif
            {
                m_state_current.rise_event(context, ev.name, ev.parameters);
            }
            goto l_loop;
        }

        Queue<(string name, StateEventParam[] parameters)> m_events = new Queue<(string name, StateEventParam[] parameters)>();
    }
}