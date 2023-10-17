
using System;

namespace CalcExpr {

    public enum ValueType : int {
        Unknown = 0,
        Integer = 1,
        Floating = 2,
        Boolean = 3,
    }

    public class Parser : Foundation.NativeObject {

        public string err_msg { get; private set; } = string.Empty;

        public Parser() : base(Utility.parser_create()) {
            
        }

        protected override void will_dispose(bool disposing) {
            Utility.parser_destroy(m_ptr);
        }

        public bool parse(string content) {
            if (disposed) {
                return false;
            }
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            if (Utility.parser_parse(m_ptr, buffer, new UIntPtr((uint)buffer.Length)) == 0) {
                update_err_msg();
                return false;
            }
            return true;
        }

        public int get_external_count() {
            if (disposed) {
                return 0;
            }
            return (int)Utility.parser_external_count(m_ptr).ToUInt32();
        }

        public string get_external_name(int index) {
            if (disposed) {
                return string.Empty;
            }
            var len = Utility.parser_external_name_len(m_ptr, new UIntPtr((uint)index)).ToUInt32();
            if (len == 0) {
                return string.Empty;
            }
            var buffer = new byte[len];
            Utility.parser_external_name_content(m_ptr, new UIntPtr((uint)index), buffer, new UIntPtr(len));
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        public void set_external_type(int index, ValueType type) {
            if (disposed) {
                return;
            }
            Utility.parser_set_external_type(m_ptr, new UIntPtr((uint)index), (int)type);
        }

        public int get_function_count() {
            if (disposed) {
                return 0;
            }
            return (int)Utility.parser_function_count(m_ptr).ToUInt32();
        }

        public string get_function_name(int index) {
            if (disposed) {
                return string.Empty;
            }
            var len = Utility.parser_function_name_len(m_ptr, new UIntPtr((uint)index)).ToUInt32();
            if (len == 0) {
                return string.Empty;
            }
            var buffer = new byte[len];
            Utility.parser_function_name_content(m_ptr, new UIntPtr((uint)index), buffer, new UIntPtr(len));
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        public (int row, int col, ValueType[] args)[] get_function_calls(int index) {
            if (disposed) {
                return new (int row, int col, ValueType[] args)[0];
            }
            var function_index = new UIntPtr((uint)index);
            var call_count = Utility.parser_function_call_count(m_ptr, function_index).ToUInt32();
            var ret = new (int row, int col, ValueType[] args)[call_count];
            for (uint i = 0; i < call_count; ++i) {
                ref var info = ref ret[i];
                var call_index = new UIntPtr(i);
                Utility.parser_function_call_location(m_ptr, function_index, call_index, out info.row, out info.col);
                var argc = Utility.parser_function_call_argc(m_ptr, function_index, call_index).ToUInt32();
                info.args = new ValueType[argc];
                Utility.parser_function_call_argv(m_ptr, function_index, call_index, info.args, new UIntPtr(argc));
            }
            return ret;
        }

        public void define_function(int index, ValueType ret_type) {
            if (disposed) {
                return;
            }
            Utility.parser_define_function_index(m_ptr, new UIntPtr((uint)index), (int)ret_type);
        }

        public bool validate() {
            if (disposed) {
                return false;
            }
            if (Utility.parser_validate(m_ptr) == 0) {
                update_err_msg();
                return false;
            }
            return true;
        }

        public ValueType get_result_type() {
            if (disposed) {
                return ValueType.Unknown;
            }
            return (ValueType)Utility.parser_result_type(m_ptr);
        }

        public bool get_const_bits(out uint val) {
            if (disposed) {
                val = 0;
                return false;
            }
            return Utility.parser_get_const_bits(m_ptr, out val) != 0;
        }

        public int get_compiled_external_count() {
            if (disposed) {
                return 0;
            }
            return (int)Utility.parser_compiled_external_count(m_ptr).ToUInt32();
        }

        public bool get_external_compiled_index(int index, out int sym) {
            if (disposed) {
                sym = 0;
                return false;
            }
            return Utility.parser_external_compiled_index(m_ptr, new UIntPtr((uint)index), out sym) != 0;
        }

        public int get_compiled_function_count() {
            if (disposed) {
                return 0;
            }
            return (int)Utility.parser_compiled_function_count(m_ptr).ToUInt32();
        }

        public bool get_function_compiled_index(int index, out int sym) {
            if (disposed) {
                sym = 0;
                return false;
            }
            return Utility.parser_function_compiled_index(m_ptr, new UIntPtr((uint)index), out sym) != 0;
        }

        public uint[] build() {
            if (disposed) {
                return null;
            }
            var byte_code = Utility.parser_create_byte_code(m_ptr);
            if (byte_code == IntPtr.Zero) {
                return null;
            }
            var len = Utility.byte_code_len(byte_code);
            var data = new uint[len.ToUInt32()];
            Utility.byte_code_content(byte_code, data, len);
            Utility.byte_code_destroy(byte_code);
            return data;
        }


        void update_err_msg() {
            var len = Utility.parser_err_msg_len(m_ptr).ToUInt32();
            if (len == 0) {
                err_msg = string.Empty;
            } else {
                var buffer = new byte[len];
                Utility.parser_err_msg_content(m_ptr, buffer, new UIntPtr(len));
                err_msg = System.Text.Encoding.UTF8.GetString(buffer);
            }
        }
    }

}