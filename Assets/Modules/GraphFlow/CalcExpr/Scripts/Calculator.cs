
using System;
using System.Runtime.InteropServices;

namespace CalcExpr {

    public class Calculator : Foundation.NativeObject {

        public Calculator() : base(Utility.vm_create()) {

        }

        protected override void will_dispose(bool disposing) {
            Utility.vm_destroy(m_ptr);
        }

        public bool attach(uint[] data) {
            if (!disposed) {
                return Utility.vm_attach(m_ptr, data, new UIntPtr((uint)data.Length)) != 0;
            }
            return false;
        }

        public void set_external(int index, int val) {
            if (disposed) {
                return;
            }
            Utility.vm_set_external_i32(m_ptr, new UIntPtr((uint)index), val);
        }

        public void set_external(int index, float val) {
            if (disposed) {
                return;
            }
            Utility.vm_set_external_f32(m_ptr, new UIntPtr((uint)index), val);
        }

        public void set_external(int index, bool val) {
            if (disposed) {
                return;
            }
            Utility.vm_set_external_bool(m_ptr, new UIntPtr((uint)index), val ? 1 : 0);
        }

        public bool run() {
            if (disposed) {
                return false;
            }
            return Utility.vm_run(m_ptr) != 0;
        }

        public bool run(IIndexFunction function) {
            if (disposed) {
                return false;
            }
            var handle = GCHandle.Alloc(function);
            var ret = Utility.vm_run_with_functions(m_ptr, GCHandle.ToIntPtr(handle), s_call);
            handle.Free();
            return ret != 0;
        }

        public bool get_result(out int val) {
            val = 0;
            if (disposed) {
                return false;
            }
            if (Utility.vm_get_result_i32(m_ptr, ref val) == 0) {
                return false;
            }
            return true;
        }

        public bool get_result(out float val) {
            val = 0;
            if (disposed) {
                return false;
            }
            if (Utility.vm_get_result_f32(m_ptr, ref val) == 0) {
                return false;
            }
            return true;
        }

        public bool get_result(out bool val) {
            val = false;
            if (disposed) {
                return false;
            }
            int t = 0;
            if (Utility.vm_get_result_bool(m_ptr, ref t) == 0) {
                return false;
            }
            val = t != 0;
            return true;
        }


        [AOT.MonoPInvokeCallback(typeof(Utility.CallIndexFunc))]
        private static int s_call(IntPtr obj, UIntPtr index, uint[] argv, UIntPtr argc, out uint ret) {
            var function = GCHandle.FromIntPtr(obj).Target as IIndexFunction;
            if (function.call((int)index.ToUInt32(), argv, out ret)) {
                return 1;
            }
            return 0;
        }


    }

}