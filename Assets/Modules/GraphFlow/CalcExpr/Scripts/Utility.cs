
using System;
using System.Runtime.InteropServices;

namespace CalcExpr {

    public static class Utility {

        public static bool get_value_type(Type type, out ValueType ty) {
            if (type == typeof(int)) {
                ty = ValueType.Integer;
                return true;
            }
            if (type == typeof(float)) {
                ty = ValueType.Floating;
                return true;
            }
            if (type == typeof(bool)) {
                ty = ValueType.Boolean;
                return true;
            }
            ty = ValueType.Unknown;
            return false;
        }

        public static Type get_type(ValueType ty) {
            switch (ty) {
                case ValueType.Integer: return typeof(int);
                case ValueType.Floating: return typeof(float);
                case ValueType.Boolean: return typeof(bool);
            }
            return null;
        }

        public static object convert_from(ValueType ty, uint val) {
            switch (ty) {
                case ValueType.Integer: return (int)val;
                case ValueType.Floating:
                    unsafe {
                        return *(float*)&val;
                    }
                case ValueType.Boolean: return val != 0;
            }
            return null;
        }

        public static float convert_float_from(uint val) {
            unsafe {
                return *(float*)&val;
            }
        }

        public static uint convert_to(ValueType ty, object val) {
            switch (ty) {
                case ValueType.Integer: return (uint)(int)val;
                case ValueType.Floating:
                    unsafe {
                        var v = (float)val;
                        return *(uint*)&v;
                    }
                case ValueType.Boolean: return (bool)val ? 1u : 0;
            }
            return 0;
        }

        public static uint convert_to(float val) {
            unsafe { return *(uint*)&val; }
        }

        public static string name(this ValueType self) {
            switch (self) {
                default:
                case ValueType.Unknown: return "string";
                case ValueType.Integer: return "int";
                case ValueType.Floating: return "float";
                case ValueType.Boolean: return "bool";
            }
        }


        internal const string PLUGIN_NAME =
#if UNITY_STANDALONE_WIN
            "calc_expr"
#elif UNITY_IOS
            "__Internal"
#else
            "libcalc_expr"
#endif
            ;

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr parser_create();


        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void parser_destroy(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int parser_parse(IntPtr parser, byte[] content_ptr, UIntPtr content_len);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_err_msg_len(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_err_msg_content(IntPtr parser, byte[] buffer, UIntPtr len);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_external_count(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_external_name_len(IntPtr parser, UIntPtr index);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_external_name_content(IntPtr parser, UIntPtr index, byte[] buffer, UIntPtr len);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void parser_set_external_type(IntPtr parser, UIntPtr index, int ty);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_function_count(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_function_name_len(IntPtr parser, UIntPtr index);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_function_name_content(IntPtr parser, UIntPtr index, byte[] buffer, UIntPtr len);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_function_call_count(IntPtr parser, UIntPtr index);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void parser_function_call_location(IntPtr parser, UIntPtr index, UIntPtr call_index, out int row, out int col);


        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_function_call_argc(IntPtr parser, UIntPtr index, UIntPtr call_index);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_function_call_argv(IntPtr parser, UIntPtr index, UIntPtr call_index, ValueType[] buf, UIntPtr len);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void parser_define_function_index(IntPtr parser, UIntPtr index, int ty);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int parser_validate(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int parser_result_type(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int parser_get_const_bits(IntPtr parser, out uint val);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_compiled_external_count(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int parser_external_compiled_index(IntPtr parser, UIntPtr index, out int sym);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr parser_compiled_function_count(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int parser_function_compiled_index(IntPtr parser, UIntPtr index, out int sym);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr parser_create_byte_code(IntPtr parser);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr byte_code_len(IntPtr byte_code);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr byte_code_content(IntPtr byte_code, uint[] buffer, UIntPtr len);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void byte_code_destroy(IntPtr byte_code);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr vm_create();

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void vm_destroy(IntPtr vm);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int vm_attach(IntPtr vm, uint[] buffer, UIntPtr length);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void vm_set_external_i32(IntPtr vm, UIntPtr index, int val);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void vm_set_external_f32(IntPtr vm, UIntPtr index, float val);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void vm_set_external_bool(IntPtr vm, UIntPtr index, int val);


        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int vm_run(IntPtr vm);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int CallIndexFunc(IntPtr obj, UIntPtr index, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] argv, UIntPtr argc, out uint ret);


        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int vm_run_with_functions(IntPtr vm, IntPtr obj, CallIndexFunc func);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int vm_get_result_i32(IntPtr vm, ref int val);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int vm_get_result_f32(IntPtr vm, ref float val);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int vm_get_result_bool(IntPtr vm, ref int val);
    }

}