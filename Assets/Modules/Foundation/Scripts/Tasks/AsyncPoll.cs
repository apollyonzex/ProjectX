

using System;
using System.Runtime.CompilerServices;

namespace Foundation.Tasks {
    public class AsyncPoll {
        public struct Timer {
            internal AsyncPoll poll;
            internal float time;

            public Awaiter GetAwaiter() {
                var obj = new TimerAwaiter(time);
                if (time > 0) {
                    poll.append(obj);
                }
                return obj;
            }
        }

        public Timer wait_for_seconds(float seconds) {
            return new Timer {
                poll = this,
                time = seconds,
            };
        }

        public struct UpdateCount {
            internal AsyncPoll poll;
            internal int count;

            public Awaiter GetAwaiter() {
                var obj = new UpdateCountAwaiter(count);
                if (count > 0) {
                    poll.append(obj);
                }
                return obj;
            }
        }

        public UpdateCount wait_for_update(int count) {
            return new UpdateCount { 
                poll = this,
                count = count,
            };
        }

        public struct Condition {
            internal AsyncPoll poll;
            internal Func<bool> condition;

            public Awaiter GetAwaiter() {
                var obj = new ConditionAwaiter(condition);
                if (!obj.IsCompleted) {
                    poll.append(obj);
                }
                return obj;
            }
        }

        public void update(float delta_time) {
            var head = m_head;
            m_head = null;
            while (head != null) {
                var obj = head;
                head = obj.next;
                if (obj.update(delta_time)) {
                    append(obj);
                }
            }
        }

        public void throw_exception(Exception exception) {
            if (exception == null) {
                return;
            }
            var head = m_head;
            m_head = null;
            while (head != null) {
                var obj = head;
                head = obj.next;
                obj.throw_exception(exception);
            }
        }

        public abstract class Awaiter : INotifyCompletion {

            internal bool IsCompleted => m_exception != null || is_completed();

            void INotifyCompletion.OnCompleted(Action continuation) {
                m_continuation = continuation;
            }

            public virtual bool update(float delta_time) {
                if (is_completed()) {
                    m_continuation?.Invoke();
                    return false;
                }
                return true;
            }

            public void throw_exception(Exception exception) {
                m_exception = exception;
                m_continuation?.Invoke();
            }

            protected abstract bool is_completed();

            internal void GetResult() {
                if (m_exception != null) {
                    throw m_exception;
                }
            }

            internal Awaiter next;

            private Action m_continuation;
            private Exception m_exception;
        }

        internal class TimerAwaiter : Awaiter {

            public TimerAwaiter(float time) {
                m_timer = time;
            }

            public override bool update(float delta_time) {
                m_timer -= delta_time;
                return base.update(delta_time);
            }

            protected override bool is_completed() {
                return m_timer <= 0;
            }

            private float m_timer;
        }

        internal class UpdateCountAwaiter : Awaiter {

            public UpdateCountAwaiter(int count) {
                m_count = count;
            }

            public override bool update(float delta_time) {
                m_count -= 1;
                return base.update(delta_time);
            }

            protected override bool is_completed() {
                return m_count <= 0;
            }

            private int m_count;
        }

        internal class ConditionAwaiter : Awaiter {
            public ConditionAwaiter(Func<bool> condition) {
                m_condition = condition;
                m_completed = condition.Invoke();
            }

            public override bool update(float delta_time) {
                m_completed = m_condition.Invoke();
                return base.update(delta_time);
            }

            protected override bool is_completed() {
                return m_completed;
            }

            public bool completed => m_completed;

            private Func<bool> m_condition;
            private bool m_completed;
        }

        private void append(Awaiter awaiter) {
            awaiter.next = m_head;
            m_head = awaiter;
        }

        private Awaiter m_head;
    }
}
