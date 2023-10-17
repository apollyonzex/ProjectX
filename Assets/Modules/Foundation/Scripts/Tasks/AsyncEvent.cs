

using System;
using System.Runtime.CompilerServices;

namespace Foundation.Tasks {
    public class AsyncEvent {
        public class Awaiter : INotifyCompletion {
            public bool IsCompleted => m_completed;
            public void GetResult() {
                if (m_exception != null) {
                    throw m_exception;
                }
            }
            internal Awaiter next;
            void INotifyCompletion.OnCompleted(Action continuation) {
                m_continuation = continuation;
            }
            internal void rise(Exception exception = null) {
                m_exception = exception;
                m_completed = true;
                m_continuation?.Invoke();
            }

            private Action m_continuation;
            private Exception m_exception;
            private bool m_completed;
        }

        public Awaiter GetAwaiter() {
            var obj = new Awaiter();
            obj.next = m_head;
            m_head = obj;
            return obj;
        }

        public void rise(Exception exception = null) {
            var head = m_head;
            m_head = null;
            while (head != null) {
                var obj = head;
                head = obj.next;

                obj.rise(exception);
            }
        }

        private Awaiter m_head;
    }

    public class AsyncEvent<T> {
        public class Awaiter : INotifyCompletion {
            public bool IsCompleted => m_completed;
            public T GetResult() {
                if (m_exception != null) {
                    throw m_exception;
                }
                return m_value;
            }
            internal Awaiter next;
            void INotifyCompletion.OnCompleted(Action continuation) {
                m_continuation = continuation;
            }
            internal void throw_exception(Exception exception) {
                m_completed = true;
                m_exception = exception;
                if (m_exception == null) {
                    throw new ArgumentNullException();
                }
                m_continuation?.Invoke();
            }
            internal void rise(T value) {
                m_completed = true;
                m_value = value;
                m_continuation?.Invoke();
            }

            private Action m_continuation;
            private Exception m_exception;
            private T m_value;
            private bool m_completed;
        }

        public Awaiter GetAwaiter() {
            var obj = new Awaiter();
            obj.next = m_head;
            m_head = obj;
            return obj;
        }

        public void throw_exception(Exception exception) {
            var head = m_head;
            m_head = null;
            while (head != null) {
                var obj = head;
                head = obj.next;

                obj.throw_exception(exception);
            }
        }

        public void rise(T value) {
            var head = m_head;
            m_head = null;
            while (head != null) {
                var obj = head;
                head = obj.next;

                obj.rise(value);
            }
        }

        private Awaiter m_head;
    }
}
