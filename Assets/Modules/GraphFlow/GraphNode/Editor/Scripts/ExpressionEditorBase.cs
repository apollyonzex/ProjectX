
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using CalcExpr;

namespace GraphNode.Editor {

    public interface IPropertyEditorWithContextType {
        void set_context_type(System.Type type, bool init);
    }

    
    public abstract class ExpressionEditorBase : GenericPropertyEditor, IPropertyEditorWithContextType {

        public ExpressionBase target { get; private set; }

        public ValueType excepted_type = ValueType.Unknown;
        public ValueType value_type { get; private set; }

        public string err_msg { get; set; }

        public enum BuildOption {
            Auto = 0,
            Manual,
            Skip,
        }

        public BuildOption build_option = BuildOption.Auto;

        public abstract ExpressionBase create_expression();

        public System.Type context_type {
            get => m_context_type;
            set {
                if (target != null) {
                    if (m_context_type != value) {
                        m_context_type = value;
                        if (build_option == BuildOption.Auto) {
                            build();
                        }
                    }
                } else {
                    m_context_type = value;
                }
            }
        }

        public void set_context_type(System.Type type, bool init) {
            if (init) {
                m_context_type = type;
            } else {
                context_type = type;
            }
        }

        public System.Type expression_context_type => m_context_type != null ? m_context_type : m_graph.graph.context_type;

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init_attr();
            init();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init_attr();
            init();
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, this.target?.clone());
            }
        }

        public override object get_value() {
            return target.content;
        }

        public override void set_value(object value) {
            target.content = (string)value;
        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (build_option == BuildOption.Auto) {
                build();
            }
        }

        public override void on_node_add_to_graph() {
            base.on_node_add_to_graph();
            if (build_option == BuildOption.Auto) {
                build();
            }
        }

        public override void on_inspector_enable() {

        }
        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            var graph = m_graph.view;
            if (!string.IsNullOrEmpty(name)) {
                EditorGUILayout.PrefixLabel(name);
            }
            var value = target.content;
            string new_value;
            EditorGUI.BeginChangeCheck();
            if (m_cmd != null) {
                GUI.SetNextControlName(name);
                new_value = EditorGUILayout.TextArea(value, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2), GUILayout.MaxWidth(GraphInspector.current.max_width));
                if (GUI.GetNameOfFocusedControl() != name) {
                    m_cmd = null;
                }
            } else {
                new_value = EditorGUILayout.TextArea(value, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2), GUILayout.MaxWidth(GraphInspector.current.max_width));
            }
            if (EditorGUI.EndChangeCheck()) {
                if (m_cmd != null && graph.undo.is_last(m_cmd)) {
                    m_cmd.new_value = new_value;
                } else {
                    m_cmd = create_undo_command(value, new_value, null);
                    graph.undo.record(m_cmd);
                }
                set_value(new_value);
                notify_changed(true);
            }
            if (err_msg != null) {
                EditorGUILayout.HelpBox(err_msg, MessageType.Error);
            }
        }

        public override void on_body_gui() {
            if (err_msg != null) {
                GUILayout.BeginHorizontal();
                if (!string.IsNullOrEmpty(name)) {
                    GUILayout.Label($"'{name}'", GUILayout.ExpandWidth(false));
                }
                GUILayout.Label("Error", GraphResources.styles.red_label);
                GUILayout.EndHorizontal();
            } else if (m_show_in_body != null) {
                if (m_show_in_body.format != null) {
                    GUILayout.Label(string.Format(m_show_in_body.format, display(true)));
                } else {
                    GUILayout.Label(display(false));
                }
            }
        }

        protected override void notify_changed(bool by_user) {
            if (build_option == BuildOption.Auto) {
                build();
            }
            base.notify_changed(by_user);
        }

        public string display(bool with_format) {
            if (target.constant.HasValue) {
               switch (value_type) {
                    case ValueType.Integer:
                        return ((int)target.constant.Value).ToString();
                    case ValueType.Floating:
                        return CalcExpr.Utility.convert_float_from(target.constant.Value).ToString();
                    case ValueType.Boolean:
                        return target.constant.Value != 0 ? "true" : "false";
                }
            }
            return is_display_detials ? target.content : with_format ? "{{ ... }}" : "{ ... }";
        }

        public virtual bool is_display_detials => true;

        public bool build() {
            value_type = ValueType.Unknown;
            target.reset();

            if (build_option == BuildOption.Skip) {
                err_msg = null;
                return true;
            }

            if (string.IsNullOrEmpty(target.content)) {
                value_type = excepted_type;
                target.constant = 0;
                err_msg = null;
                return true;
            }

            var parser = ExpressionParser.instance;
            if (!parser.parse(target.content)) {
                err_msg = parser.err_msg;
                return false;
            }
            err_msg = null;

            var external_count = parser.get_external_count();
            init_external(external_count);
            for (int i = 0; i < external_count; ++i) {
                var external_name = parser.get_external_name(i);
                if (get_external(i, external_name, out var ty)) {
                    parser.set_external_type(i, ty);
                } else {
                    err_msg += "Invalid variable \'" + external_name + "\'\n";
                }
            }

            var function_count = parser.get_function_count();
            var functions = new ExpressionFunction[function_count];
            if (function_count > 0) {
                var dict = Utility.get_expression_functions(expression_context_type);
                if (dict == null) {
                    for (int i = 0; i < function_count; ++i) {
                        var fn = parser.get_function_name(i);
                        err_msg += $"Unknown function \'{fn}\'\n";
                    }
                } else {
                    for (int i = 0; i < function_count; ++i) {
                        var fn = parser.get_function_name(i);
                        if (dict.TryGetValue(fn, out var func)) {
                            functions[i] = func;
                            parser.define_function(i, func.invoker.ret_type);
                        } else {
                            err_msg += $"Unknown function \'{fn}\'\n";
                        }
                    }
                }
            }
            if (err_msg != null) {
                drop_external();
                return false;
            }
            if (!parser.validate()) {
                err_msg = parser.err_msg;
                drop_external();
                return false;
            }

            if (function_count > 0) {
                for (int i = 0; i < function_count; ++i) {
                    foreach (var (row, col, args) in parser.get_function_calls(i)) {
                        var func = functions[i];
                        if (func != null && !func.invoker.check(args)) {
                            err_msg += $"{row}:{col}: \'{parser.get_function_name(i)}\' invalid argument(s)";
                        }
                    }
                }
            }

            if (err_msg != null) {
                drop_external();
                return false;
            }

            var rt = parser.get_result_type();
            if (excepted_type != ValueType.Unknown && rt != excepted_type) {
                err_msg = $"Invalid result type: got {rt}, expected {excepted_type}";
                drop_external();
                return false;
            }

            if (parser.get_const_bits(out var constant)) {
                value_type = rt;
                target.constant = constant;
                drop_external();
                return true;
            }

            var code = parser.build();
            if (code == null) {
                err_msg = "Build failed";
                drop_external();
                return false;
            }

            var compiled_external_count = parser.get_compiled_external_count();
            if (compiled_external_count == external_count) {
                store_external();
            } else {
                store_compiled_external(compiled_external_count);
                for (int i = 0; i < external_count; ++i) {
                    if (parser.get_external_compiled_index(i, out var sym)) {
                        move_external_to_compiled(i, sym);
                    }
                }
                drop_external();
            }

            ExpressionFunction[] compiled_functions;

            var compiled_function_count = parser.get_compiled_function_count();
            if (compiled_function_count == function_count) {
                compiled_functions = functions;
            } else {
                compiled_functions = new ExpressionFunction[compiled_function_count];
                for (int i = 0; i < function_count; ++i) {
                    if (parser.get_function_compiled_index(i, out var sym)) {
                        compiled_functions[sym] = functions[i];
                    }
                }
            }

            value_type = rt;
            target.code = code;
            target.functions = compiled_functions;
            return true;
        }

        protected virtual void init_external(int count) {

        }

        protected virtual void drop_external() {

        }

        protected virtual bool get_external(int index, string name, out ValueType ty) {
            ty = ValueType.Unknown;
            return false;
        }

        protected virtual void store_external() {

        }

        protected virtual void store_compiled_external(int count) {

        }

        protected virtual void move_external_to_compiled(int index, int target) {

        }

        void init_attr() {
            var attr = m_fi?.GetCustomAttribute<ExpressionTypeAttribute>();
            if (attr != null) {
                excepted_type = attr.type;
            }
        }

        void init() {
            if (m_fi != null) {
                target = (ExpressionBase)m_fi.GetValue(obj);
                if (target == null) {
                    target = create_expression();
                    m_fi.SetValue(obj, target);
                }
            } else {
                target = obj as ExpressionBase ?? create_expression();
            }
        }

        private ChangeValue<string> m_cmd = null;
        private System.Type m_context_type = null;
    }

    public static class ExpressionParser {

        public static Parser instance {
            get {
                if (s_instance == null) {
                    s_instance = new Parser();
                }
                return s_instance;
            }
        }

        private static Parser s_instance = null;
    }
}