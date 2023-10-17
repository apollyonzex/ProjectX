
using System;
using System.Runtime.InteropServices;

namespace Foundation.Editor {

    public interface IExcelTableExternalParser {
        bool parse(string content);
        byte[] data { get; }
        string err_msg { get; }
    }

    public static class ExcelTableUtility {

        internal const string PLUGIN_NAME =
#if UNITY_EDITOR_WIN
            "vntblcore"
#else
            "libvntblcore"
#endif
            ;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void StringFunc(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void StringU32Func(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len, uint id);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void EP_Drop(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int EP_Parse(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate UIntPtr EP_GetLength(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate UIntPtr EP_GetContent(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len);


        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr excel_file_open(byte[] path, IntPtr path_length, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void excel_file_close(IntPtr file);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int excel_file_load_workbook(IntPtr file, IntPtr obj, StringFunc output, StringU32Func info);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr excel_file_load_sheet(IntPtr file, uint sheet_id, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void excel_sheet_release(IntPtr obj);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_create(IntPtr file, uint sheet_id, uint mask, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_create_ex(IntPtr file, IntPtr readers, uint sheet_id, uint mask, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_create_with_sheet(IntPtr sheet, uint mask, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_create_with_sheet_ex(IntPtr sheet, IntPtr readers, uint mask, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void builder_destroy(IntPtr builder);


        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_generate_code_data(IntPtr builder, byte[] name, IntPtr name_length);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_generate_rust_code_data(IntPtr builder, byte[] name, IntPtr name_length);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr data_length(IntPtr data);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int data_equals(IntPtr data, byte[] content, IntPtr length);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr data_content(IntPtr data, byte[] content, IntPtr length);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void data_destroy(IntPtr data);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr builder_generate_table_data(IntPtr builder, IntPtr obj, StringFunc output);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr external_readers_create();

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void external_readers_destroy(IntPtr obj);

        [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void external_readers_insert(IntPtr obj, byte[] name, IntPtr name_len, IntPtr parser, EP_Drop drop, EP_Parse parse, EP_GetLength get_data_len, EP_GetContent get_data, EP_GetLength get_err_len, EP_GetContent get_err_msg);


        [AOT.MonoPInvokeCallback(typeof(StringFunc))]
        private static void cb_output(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len) {
            var output = GCHandle.FromIntPtr(obj).Target as IOutput;
            output.output(System.Text.Encoding.UTF8.GetString(buf));
        }

        [AOT.MonoPInvokeCallback(typeof(StringFunc))]
        private static void cb_info(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len, uint id) {
            var output = GCHandle.FromIntPtr(obj).Target as IOutput;
            output.info(System.Text.Encoding.UTF8.GetString(buf), id);
        }

        public interface IOutput {
            void output(string content);
            void info(string name, uint id);
        }

        public class ExcelFile : NativeObject {
            private ExcelFile(IntPtr ptr) : base(ptr) {

            }

            protected override void will_dispose(bool _) {
                excel_file_close(m_ptr);
            }

            public static ExcelFile open(string path, IOutput output) {
                var bytes = System.Text.Encoding.UTF8.GetBytes(path);
                var handle = GCHandle.Alloc(output);
                var ptr = excel_file_open(bytes, new IntPtr(bytes.Length), GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ptr != IntPtr.Zero) {
                    return new ExcelFile(ptr);
                }
                return null;
            }

            public bool load_workbook(IOutput output) {
                if (disposed) {
                    return false;
                }
                var handle = GCHandle.Alloc(output);
                var ret = excel_file_load_workbook(m_ptr, GCHandle.ToIntPtr(handle), cb_output, cb_info);
                handle.Free();
                return ret != 0;
            }

            public Builder load_builder(uint sheet_id, uint mask, IOutput output) {
                if (disposed) {
                    return null;
                }
                var handle = GCHandle.Alloc(output);
                var ret = builder_create(m_ptr, sheet_id, mask, GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ret != IntPtr.Zero) {
                    return new Builder(ret);
                }
                return null;
            }

            public Builder load_builder(uint sheet_id, uint mask, ExternalParsers parsers, IOutput output) {
                if (parsers == null || parsers.disposed) {
                    return load_builder(sheet_id, mask, output);
                }
                if (disposed) {
                    return null;
                }
                var handle = GCHandle.Alloc(output);
                var ret = builder_create_ex(m_ptr, parsers.ptr, sheet_id, mask, GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ret != IntPtr.Zero) {
                    return new Builder(ret);
                }
                return null;
            }

            public ExcelSheet load_sheet(uint sheet_id, IOutput output) {
                if (disposed) {
                    return null;
                }
                var handle = GCHandle.Alloc(output);
                var ret = excel_file_load_sheet(m_ptr, sheet_id, GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ret != IntPtr.Zero) {
                    return new ExcelSheet(ret);
                }
                return null;
            }
        }

        public class ExcelSheet : NativeObject {
            internal ExcelSheet(IntPtr ptr) : base(ptr) {

            }

            public Builder create_builder(uint mask, IOutput output) {
                if (disposed) {
                    return null;
                }
                var handle = GCHandle.Alloc(output);
                var ret = builder_create_with_sheet(m_ptr, mask, GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ret != IntPtr.Zero) {
                    return new Builder(ret);
                }
                return null;
            }

            public Builder create_builder(uint mask, ExternalParsers parsers, IOutput output) {
                if (disposed) {
                    return null;
                }
                if (parsers == null || parsers.disposed) {
                    return create_builder(mask, output);
                }
                var handle = GCHandle.Alloc(output);
                var ret = builder_create_with_sheet_ex(m_ptr, parsers.ptr, mask, GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ret != IntPtr.Zero) {
                    return new Builder(ret);
                }
                return null;
            }

            protected override void will_dispose(bool _) {
                excel_sheet_release(m_ptr);
            }
        }

        public class Builder : NativeObject {
            internal Builder(IntPtr ptr) : base(ptr) {

            }

            public Data generate_code(string name) {
                if (disposed) {
                    return null;
                }
                var bytes = System.Text.Encoding.UTF8.GetBytes(name);
                var ret = builder_generate_code_data(m_ptr, bytes, new IntPtr(bytes.Length));
                if (ret != IntPtr.Zero) {
                    return new Data(ret);
                }
                return null;
            }

            public Data generate_rust_code(string name) {
                if (disposed) {
                    return null;
                }
                var bytes = System.Text.Encoding.UTF8.GetBytes(name);
                var ret = builder_generate_rust_code_data(m_ptr, bytes, new IntPtr(bytes.Length));
                if (ret != IntPtr.Zero) {
                    return new Data(ret);
                }
                return null;
            }

            public Data generate_table(IOutput output) {
                if (disposed) {
                    return null;
                }
                var handle = GCHandle.Alloc(output);
                var ret = builder_generate_table_data(m_ptr, GCHandle.ToIntPtr(handle), cb_output);
                handle.Free();
                if (ret != IntPtr.Zero) {
                    return new Data(ret);
                }
                return null;
            }

            protected override void will_dispose(bool _) {
                builder_destroy(m_ptr);
            }
        }

        public class Data : NativeObject, IEquatable<byte[]> {

            internal Data(IntPtr ptr) : base(ptr) {

            }

            public bool Equals(byte[] other) {
                if (disposed) {
                    return other == null;
                }
                return data_equals(m_ptr, other, new IntPtr(other.Length)) != 0;
            }

            protected override void will_dispose(bool _) {
                data_destroy(m_ptr);
            }

            public byte[] get_content() {
                if (disposed) {
                    return null;
                }
                var length = data_length(m_ptr).ToInt32();
                var content = new byte[length];
                data_content(m_ptr, content, new IntPtr(length));
                return content;
            }
        }

        public class ExternalParsers : NativeObject {
            internal ExternalParsers() : base(external_readers_create()) {

            }

            internal IntPtr ptr => m_ptr;

            public void insert(string name, IExcelTableExternalParser parser) {
                if (disposed || string.IsNullOrEmpty(name) || parser == null) {
                    return;
                }
                var handle = GCHandle.Alloc(parser);
                var bytes = System.Text.Encoding.UTF8.GetBytes(name);
                external_readers_insert(m_ptr, bytes, new IntPtr(bytes.Length), GCHandle.ToIntPtr(handle), drop, parse, get_data_len, get_data_content, get_err_len, get_err_msg);
            }

            protected override void will_dispose(bool _) {
                external_readers_destroy(m_ptr);
            }

            [AOT.MonoPInvokeCallback(typeof(EP_Drop))]
            static void drop(IntPtr obj) {
                GCHandle.FromIntPtr(obj).Free();
            }

            [AOT.MonoPInvokeCallback(typeof(EP_Parse))]
            static int parse(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len) {
                var parser = GCHandle.FromIntPtr(obj).Target as IExcelTableExternalParser;
                return parser.parse(System.Text.Encoding.UTF8.GetString(buf)) ? 1 : 0;
            }

            [AOT.MonoPInvokeCallback(typeof(EP_GetLength))]
            static UIntPtr get_data_len(IntPtr obj) {
                var parser = GCHandle.FromIntPtr(obj).Target as IExcelTableExternalParser;
                return new UIntPtr((uint)parser.data.Length);
            }

            [AOT.MonoPInvokeCallback(typeof(EP_GetContent))]
            static UIntPtr get_data_content(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len) {
                var parser = GCHandle.FromIntPtr(obj).Target as IExcelTableExternalParser;
                var data = parser.data;
                var len = Math.Min(data.Length, (int)(uint)buf_len);
                Array.Copy(data, buf, len);
                return new UIntPtr((uint)len);
            }

            [AOT.MonoPInvokeCallback(typeof(EP_GetLength))]
            static UIntPtr get_err_len(IntPtr obj) {
                var parser = GCHandle.FromIntPtr(obj).Target as IExcelTableExternalParser;
                return new UIntPtr((uint) System.Text.Encoding.UTF8.GetByteCount(parser.err_msg));
            }

            [AOT.MonoPInvokeCallback(typeof(EP_GetContent))]
            static UIntPtr get_err_msg(IntPtr obj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buf, UIntPtr buf_len) {
                var parser = GCHandle.FromIntPtr(obj).Target as IExcelTableExternalParser;
                var data = System.Text.Encoding.UTF8.GetBytes(parser.err_msg);
                var len = Math.Min(data.Length, (int)(uint)buf_len);
                Array.Copy(data, buf, len);
                return new UIntPtr((uint)len);
            }
        }
    }

}