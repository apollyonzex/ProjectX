
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Foundation {

    public class DynamicPlugin : IDisposable {

        public static DynamicPlugin load(string path) {
#if UNITY_STANDALONE_WIN
            var handle = LoadLibrary(path.Replace('/', '\\'));
            if (handle == IntPtr.Zero) {
                Debug.LogError($"LoadLibrary({path}) error: {Marshal.GetLastWin32Error()}");
            } else {
                return new DynamicPlugin(handle);
            }
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            var handle = dlopen(path, 0);
            if (handle == IntPtr.Zero) {
                Debug.LogError($"dlopen({path}) error: {Marshal.PtrToStringAnsi(dlerror())}");
            } else {
                return new DynamicPlugin(handle);
            }
#endif
            return null;
        }

        private DynamicPlugin(IntPtr handle) {
            m_handle = handle;
        }

        ~DynamicPlugin() {
            dispose();
        }

        public T get_symbol<T>(string name) where T : Delegate {
            if (m_handle == IntPtr.Zero) {
                throw new ObjectDisposedException(null);
            }
#if UNITY_STANDALONE_WIN
            var sym = GetProcAddress(m_handle, name);
            if (sym == IntPtr.Zero) {
                Debug.LogError($"GetProcAddress({name}) error: {Marshal.GetLastWin32Error()}");
            } else {
                return Marshal.GetDelegateForFunctionPointer<T>(sym);
            }
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            var sym = dlsym(m_handle, name);
            if (sym == IntPtr.Zero) {
                Debug.LogError($"dlsym({name}) error: {Marshal.PtrToStringAnsi(dlerror())}");
            } else {
                return Marshal.GetDelegateForFunctionPointer<T>(sym);
            }
#endif            
            return null;
        }

        public Delegate get_symbol(string name, Type type) {
            if (m_handle == IntPtr.Zero) {
                throw new ObjectDisposedException(null);
            }
#if UNITY_STANDALONE_WIN
            var sym = GetProcAddress(m_handle, name);
            if (sym == IntPtr.Zero) {
                Debug.LogError($"GetProcAddress({name}) error: {Marshal.GetLastWin32Error()}");
            } else {
                return Marshal.GetDelegateForFunctionPointer(sym, type);
            }
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            var sym = dlsym(m_handle, name);
            if (sym == IntPtr.Zero) {
                Debug.LogError($"dlsym({name}) error: {Marshal.PtrToStringAnsi(dlerror())}");
            } else {
                return Marshal.GetDelegateForFunctionPointer(sym, type);
            }
#endif            
            return null;
        }

        public void Dispose() {
            dispose();
            GC.SuppressFinalize(this);
        }

        private void dispose() {
            if (m_handle == IntPtr.Zero) {
                return;
            }
#if UNITY_STANDALONE_WIN
            if (FreeLibrary(m_handle) == 0) {
                Debug.LogError($"FreeLibrary error: {Marshal.GetLastWin32Error()}");
            }
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            if (dlclose(m_handle) != 0) {
                Debug.LogError($"dlclose error: {Marshal.PtrToStringAnsi(dlerror())}");
            }
#endif
            m_handle = IntPtr.Zero;
        }

#if UNITY_STANDALONE_WIN
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("kernel32")]
        private static extern int FreeLibrary(IntPtr handle);

        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr handle, string symbol);



#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlopen(string path, int mode);


        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dlclose(IntPtr handle);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlerror();
#endif

        private IntPtr m_handle;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T>(T t);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1>(T0 t0, T1 t1);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1, T2>(T0 t0, T1 t1, T2 t2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Action<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<R>();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T, R>(T t);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, R>(T0 t0, T1 t1);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, T2, R>(T0 t0, T1 t1, T2 t2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, T2, T3, R>(T0 t0, T1 t1, T2 t2, T3 t3);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, T2, T3, T4, R>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, T2, T3, T4, T5, R>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, T2, T3, T4, T5, T6, R>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate R Func<T0, T1, T2, T3, T4, T5, T6, T7, R>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    }

}