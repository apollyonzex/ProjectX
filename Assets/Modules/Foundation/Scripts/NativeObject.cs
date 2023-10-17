
using System;

namespace Foundation {

    public abstract class NativeObject : IDisposable {

        public NativeObject() {

        }

        public NativeObject(IntPtr ptr) {
            m_ptr = ptr;
        }
        
        ~NativeObject() {
            _dispose(false);
        }

        public void dispose() {
            _dispose(true);
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose() {
            dispose();
        }

        void _dispose(bool disposing) {
            if (!m_disposed) {
                will_dispose(disposing);
                m_disposed = true;
            }
        }

        public bool disposed => m_disposed;

        public void assert_not_disposed() {
            if (m_disposed) {
                throw new ObjectDisposedException(ToString());
            }
        }

        protected abstract void will_dispose(bool disposing);

        protected IntPtr m_ptr;
        private bool m_disposed;
    }

}