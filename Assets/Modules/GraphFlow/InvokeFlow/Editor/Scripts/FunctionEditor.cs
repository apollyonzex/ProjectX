
using CalcExpr;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using GraphNode.Editor;
using GraphNode;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(Function))]
    public class FunctionEditor : GenericPropertyEditor {
        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init();
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, (get_value() as Function)?.clone());
            }
        }

        public override void on_inspector_disable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_disable();
                }
            }
            if (m_out_name_editors != null) {
                foreach (var one in m_out_name_editors) {
                    one.on_inspector_disable();
                }
            }
        }

        public override void on_inspector_enable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_enable();
                }
            }
            if (m_out_name_editors != null) {
                foreach (var one in m_out_name_editors) {
                    one.on_inspector_enable();
                }
            }
        }


        public override void on_inspector_gui() {
            Utility.get_context_methods(m_graph.graph.context_type, out var names, out var methods);

            EditorGUI.BeginChangeCheck();
            var new_index = EditorGUILayout.Popup(name, m_index, names);
            if (EditorGUI.EndChangeCheck() && new_index != m_index) {
                var new_method = methods[new_index];
                Function new_target = null;
                ExpressionEditor[] new_ees = null;
                OutNameEditor[] new_ones = null;
                if (new_method != null) {
                    new_target = new Function() {
                        method = new_method,
                        parameters = new Expression[new_method.input_indices.Length],
                        out_names = new string[new_method.output_indices.Length],
                    };
                    foreach (ref var e in Foundation.ArraySlice.create(new_target.parameters)) {
                        e = new Expression();
                    }
                    new_ees = generate_parameter_editors(new_target, true);
                    new_ones = generate_out_name_editors(new_target, true);
                }
                m_graph.view.undo.record(new ChangeTarget {
                    editor = this,
                    old_value = (get_value() as Function, m_index, m_paramter_editors, m_out_name_editors),
                    new_value = (new_target, new_index, new_ees, new_ones),
                });
                if (m_paramter_editors != null) {
                    foreach (var ee in m_paramter_editors) {
                        ee.on_inspector_disable();
                    }
                }
                if (m_out_name_editors != null) {
                    foreach (var one in m_out_name_editors) {
                        one.on_inspector_disable();
                    }
                }
                set_value(new_target);
                m_index = new_index;
                m_paramter_editors = new_ees;
                m_out_name_editors = new_ones;
                if (new_ees != null) {
                    foreach (var ee in new_ees) {
                        ee.on_inspector_enable();
                    }
                }
                if (new_ones != null) {
                    foreach (var one in new_ones) {
                        one.on_inspector_enable();
                    }
                }
                notify_changed(true);
            }

            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_gui();
                }
            }
            if (m_out_name_editors != null) {
                foreach (var one in m_out_name_editors) {
                    one.on_inspector_gui();
                }
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                if (m_index != -1) {
                    Utility.get_context_methods(m_graph.graph.context_type, out var names, out _);
                    if (m_show_in_body.format != null) {
                        if (m_show_in_body.format != string.Empty) {
                            GUILayout.Label(string.Format(m_show_in_body.format, names[m_index]));
                        }
                    } else {
                        GUILayout.Label(names[m_index]);
                    }
                }
            }
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_body_gui();
                }
            }
        }

        public void build_parameters() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.build();
                }
            }
        }

        public class OutNameEditor : PropertyStringEditor {
            protected override bool validate(string new_value) {
                return VariablesEditor.check_name(new_value);
            }

            public int index;
            public new event System.Action<int, bool> on_changed;

            protected override void notify_changed(bool by_user) {
                base.notify_changed(by_user);
                on_changed?.Invoke(index, by_user);
            }

        }

        private void init() {
            var target = get_value() as Function;
            MethodInfo method_info = null;
            if (target != null) {
                target.method.initialize(m_graph.graph.context_type);
                if (target.method.method_info != null) {
                    method_info = target.method.method_info;
                    m_paramter_editors = generate_parameter_editors(target, false);
                    m_out_name_editors = generate_out_name_editors(target, false);
                } else {
                    set_value(null);
                }
            }

            Utility.get_context_methods(m_graph.graph.context_type, out _, out var methods);
            for (int i = 0; i < methods.Length; ++i) {
                if (methods[i]?.method_info == method_info) {
                    m_index = i;
                    break;
                }
            }
        }

        private ExpressionEditor[] generate_parameter_editors(Function target, bool init) {
            ExpressionEditor[] ret;
            if (target.method.method_info != null) {
                var ps = target.method.method_info.GetParameters();
                ret = new ExpressionEditor[target.method.input_indices.Length];
                for (int i = 0; i < ret.Length; ++i) {
                    var ee = new ExpressionEditor();
                    var idx = target.method.input_indices[i];
                    var pi = ps[idx];
                    switch (target.method.parameter_types[idx]) {
                        case FunctionParameterType.Integer:
                        case FunctionParameterType.IntegerRef:
                            ee.excepted_type = ValueType.Integer;
                            break;

                        case FunctionParameterType.Floating:
                        case FunctionParameterType.FloatingRef:
                            ee.excepted_type = ValueType.Floating;
                            break;

                        case FunctionParameterType.Boolean:
                        case FunctionParameterType.BooleanRef:
                            ee.excepted_type = ValueType.Boolean;
                            break;
   
                        default:
                            ee.excepted_type = ValueType.Unknown;
                            ee.build_option = ExpressionEditorBase.BuildOption.Skip;
                            break;
                    }
                    ee.attach(target.parameters[i], null, m_graph, m_node, pi.Name, 0, pi.GetCustomAttribute<ShowInBodyAttribute>());
                    if (init) {
                        var defval = pi.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();
                        if (defval != null) {
                            switch (ee.excepted_type) {
                                case ValueType.Integer: {
                                    if (defval.Value is int val) {
                                        ee.target.content = val.ToString();
                                    }
                                    break;
                                }
                                case ValueType.Floating: {
                                    if (defval.Value is float val) {
                                        if (val == float.PositiveInfinity) {
                                            ee.target.content = "inf";
                                        } else if (val == float.NegativeInfinity) {
                                            ee.target.content = "-inf";
                                        } else {
                                            ee.target.content = val.ToString("0.0");
                                        }
                                    }
                                    break;
                                }
                                case ValueType.Boolean: {
                                    if (defval.Value is bool val) {
                                        ee.target.content = val ? "true" : "false";
                                    }
                                    break;
                                }
                                case ValueType.Unknown: {
                                    if (defval.Value is string val) {
                                        ee.target.content = val;
                                    }
                                    break;
                                }
                            }
                            ee.build();
                        }
                    }
                    ret[i] = ee;
                }
            } else {
                ret = null;
            }
            return ret;
        }

        private OutNameEditor[] generate_out_name_editors(Function target, bool init) {
            OutNameEditor[] ret;
            if (target.method.method_info != null) {
                var ps = target.method.method_info.GetParameters();
                ret = new OutNameEditor[target.method.output_indices.Length];
                if (!init && target.out_names == null) {
                    target.out_names = new string[ret.Length];
                    for (int i = 0; i < ret.Length; ++i) {
                        target.out_names[i] = ps[target.method.output_indices[i]].Name;
                    }
                } else if (target.out_names.Length != ret.Length) {
                    target.out_names = new string[ret.Length];
                }

                for (int i = 0; i < ret.Length; ++i) {
                    var se = new OutNameEditor();
                    se.index = i;
                    se.on_changed += on_out_name_changed;
                    var idx = target.method.output_indices[i];
                    var pi = ps[idx];
                    se.attach(target.out_names[i], null, m_graph, m_node, pi.Name, 0, pi.GetCustomAttribute<ShowInBodyAttribute>());
                    ret[i] = se;
                }
            } else {
                ret = null;
            }
            return ret;
        }

        private void on_out_name_changed(int index, bool by_user) {
            var target = get_value() as Function;
            if (target != null) {
                target.out_names[index] = (string)m_out_name_editors[index].get_value();
            }
            notify_changed(by_user);
        }

        private int m_index = -1;
        private ExpressionEditor[] m_paramter_editors;
        private OutNameEditor[] m_out_name_editors;

        private class ChangeTarget : GraphUndo.ChangeValue<(Function target, int index, ExpressionEditor[] ees, OutNameEditor[] ones)> {

            public FunctionEditor editor;

            protected override void set_value(ref (Function target, int index, ExpressionEditor[] ees, OutNameEditor[] ones) old_value, ref (Function target, int index, ExpressionEditor[] ees, OutNameEditor[] ones) new_value) {
                editor.set_value(new_value.target);
                editor.m_index = new_value.index;
                editor.m_paramter_editors = new_value.ees;
                editor.m_out_name_editors = new_value.ones;

                if (editor.m_node.inspector_enabled) {
                    if (old_value.ees != null) {
                        foreach (var ee in old_value.ees) {
                            ee.on_inspector_disable();
                        }
                    }
                    if (old_value.ones != null) {
                        foreach (var one in old_value.ones) {
                            one.on_inspector_disable();
                        }
                    }
                    if (new_value.ees != null) {
                        foreach (var ee in new_value.ees) {
                            ee.on_inspector_enable();
                        }
                    }
                    if (new_value.ones != null) {
                        foreach (var one in new_value.ones) {
                            one.on_inspector_enable();
                        }
                    }
                }
                editor.notify_changed(false);
            }

        }

    }

    [PropertyEditor(typeof(Function<>))]
    public class FunctionEditor<T> : GenericPropertyEditor {
        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init();
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, (get_value() as Function<T>)?.clone());
            }
        }

        public override void on_inspector_disable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_disable();
                }
            }
            if (m_out_name_editors != null) {
                foreach (var one in m_out_name_editors) {
                    one.on_inspector_disable();
                }
            }
        }

        public override void on_inspector_enable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_enable();
                }
            }
            if (m_out_name_editors != null) {
                foreach (var one in m_out_name_editors) {
                    one.on_inspector_enable();
                }
            }
        }


        public override void on_inspector_gui() {
            Utility.get_context_methods<T>(m_graph.graph.context_type, out var names, out var methods);

            EditorGUI.BeginChangeCheck();
            var new_index = EditorGUILayout.Popup(name, m_index, names);
            if (EditorGUI.EndChangeCheck() && new_index != m_index) {
                var new_method = methods[new_index];
                Function<T> new_target = null;
                ExpressionEditor[] new_ees = null;
                FunctionEditor.OutNameEditor[] new_ones = null;
                if (new_method != null) {
                    new_target = new Function<T>() {
                        method = new_method,
                        parameters = new Expression[new_method.input_indices.Length],
                        out_names = new string[new_method.output_indices.Length],
                    };
                    foreach (ref var e in Foundation.ArraySlice.create(new_target.parameters)) {
                        e = new Expression();
                    }
                    new_ees = generate_parameter_editors(new_target, true);
                    new_ones = generate_out_name_editors(new_target, true);
                }
                m_graph.view.undo.record(new ChangeTarget {
                    editor = this,
                    old_value = (get_value() as Function<T>, m_index, m_paramter_editors, m_out_name_editors),
                    new_value = (new_target, new_index, new_ees, new_ones),
                });
                if (m_paramter_editors != null) {
                    foreach (var ee in m_paramter_editors) {
                        ee.on_inspector_disable();
                    }
                }
                if (m_out_name_editors != null) {
                    foreach (var one in m_out_name_editors) {
                        one.on_inspector_disable();
                    }
                }
                set_value(new_target);
                m_index = new_index;
                m_paramter_editors = new_ees;
                m_out_name_editors = new_ones;
                if (new_ees != null) {
                    foreach (var ee in new_ees) {
                        ee.on_inspector_enable();
                    }
                }
                if (new_ones != null) {
                    foreach (var one in new_ones) {
                        one.on_inspector_enable();
                    }
                }
                notify_changed(true);
            }

            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_gui();
                }
            }
            if (m_out_name_editors != null) {
                foreach (var one in m_out_name_editors) {
                    one.on_inspector_gui();
                }
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                if (m_index != -1) {
                    Utility.get_context_methods<T>(m_graph.graph.context_type, out var names, out _);
                    if (m_show_in_body.format != null) {
                        if (m_show_in_body.format != string.Empty) {
                            GUILayout.Label(string.Format(m_show_in_body.format, names[m_index]));
                        }
                    } else {
                        GUILayout.Label(names[m_index]);
                    }
                }
            }
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_body_gui();
                }
            }
        }

        public void build_parameters() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.build();
                }
            }
        }

        private void init() {
            var target = get_value() as Function<T>;
            MethodInfo method_info = null;
            if (target != null) {
                target.method.initialize(m_graph.graph.context_type);
                if (target.method.method_info != null) {
                    method_info = target.method.method_info;
                    m_paramter_editors = generate_parameter_editors(target, false);
                    m_out_name_editors = generate_out_name_editors(target, false);
                } else {
                    set_value(null);
                }
            }

            Utility.get_context_methods<T>(m_graph.graph.context_type, out _, out var methods);
            m_index = 0;
            for (int i = 1; i < methods.Length; ++i) {
                if (methods[i].method_info == method_info) {
                    m_index = i;
                    break;
                }
            }
        }

        private ExpressionEditor[] generate_parameter_editors(Function<T> target, bool init) {
            ExpressionEditor[] ret;
            if (target.method.method_info != null) {
                var ps = target.method.method_info.GetParameters();
                ret = new ExpressionEditor[target.method.input_indices.Length];
                for (int i = 0; i < ret.Length; ++i) {
                    var ee = new ExpressionEditor();
                    var idx = target.method.input_indices[i];
                    var pi = ps[idx + 1];
                    switch (target.method.parameter_types[idx]) {
                        case FunctionParameterType.Integer:
                        case FunctionParameterType.IntegerRef:
                            ee.excepted_type = ValueType.Integer;
                            break;

                        case FunctionParameterType.Floating:
                        case FunctionParameterType.FloatingRef:
                            ee.excepted_type = ValueType.Floating;
                            break;

                        case FunctionParameterType.Boolean:
                        case FunctionParameterType.BooleanRef:
                            ee.excepted_type = ValueType.Boolean;
                            break;

                        default:
                            ee.excepted_type = ValueType.Unknown;
                            ee.build_option = ExpressionEditorBase.BuildOption.Skip;
                            break;
                    }
                    ee.attach(target.parameters[i], null, m_graph, m_node, pi.Name, 0, pi.GetCustomAttribute<ShowInBodyAttribute>());
                    if (init) {
                        var defval = pi.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();
                        if (defval != null) {
                            switch (ee.excepted_type) {
                                case ValueType.Integer: {
                                    if (defval.Value is int val) {
                                        ee.target.content = val.ToString();
                                    }
                                    break;
                                }
                                case ValueType.Floating: {
                                    if (defval.Value is float val) {
                                        if (val == float.PositiveInfinity) {
                                            ee.target.content = "inf";
                                        } else if (val == float.NegativeInfinity) {
                                            ee.target.content = "-inf";
                                        } else {
                                            ee.target.content = val.ToString("0.0");
                                        }
                                    }
                                    break;
                                }
                                case ValueType.Boolean: {
                                    if (defval.Value is bool val) {
                                        ee.target.content = val ? "true" : "false";
                                    }
                                    break;
                                }
                                case ValueType.Unknown: {
                                    if (defval.Value is string val) {
                                        ee.target.content = val;
                                    }
                                    break;
                                }
                            }
                            ee.build();
                        }
                    }
                    ret[i] = ee;
                }
            } else {
                ret = null;
            }
            return ret;
        }
        private FunctionEditor.OutNameEditor[] generate_out_name_editors(Function<T> target, bool init) {
            FunctionEditor.OutNameEditor[] ret;
            if (target.method.method_info != null) {
                var ps = target.method.method_info.GetParameters();
                ret = new FunctionEditor.OutNameEditor[target.method.output_indices.Length];
                if (!init && target.out_names == null) {
                    target.out_names = new string[ret.Length];
                    for (int i = 0; i < ret.Length; ++i) {
                        target.out_names[i] = ps[target.method.output_indices[i] + 1].Name;
                    }
                } else if (target.out_names.Length != ret.Length) {
                    target.out_names = new string[ret.Length];
                }

                for (int i = 0; i < ret.Length; ++i) {
                    var se = new FunctionEditor.OutNameEditor();
                    se.index = i;
                    se.on_changed += on_out_name_changed;
                    var idx = target.method.output_indices[i];
                    var pi = ps[idx + 1];
                    se.attach(target.out_names[i], null, m_graph, m_node, pi.Name, 0, pi.GetCustomAttribute<ShowInBodyAttribute>());
                    ret[i] = se;
                }
            } else {
                ret = null;
            }
            return ret;
        }

        private void on_out_name_changed(int index, bool by_user) {
            var target = get_value() as Function<T>;
            if (target != null) {
                target.out_names[index] = (string)m_out_name_editors[index].get_value();
            }
            notify_changed(by_user);
        }

        private int m_index = -1;
        private ExpressionEditor[] m_paramter_editors;
        private FunctionEditor.OutNameEditor[] m_out_name_editors;

        private class ChangeTarget : GraphUndo.ChangeValue<(Function<T> target, int index, ExpressionEditor[] ees, FunctionEditor.OutNameEditor[] ones)> {

            public FunctionEditor<T> editor;

            protected override void set_value(ref (Function<T> target, int index, ExpressionEditor[] ees, FunctionEditor.OutNameEditor[] ones) old_value, ref (Function<T> target, int index, ExpressionEditor[] ees, FunctionEditor.OutNameEditor[] ones) new_value) {
                editor.set_value(new_value.target);
                editor.m_index = new_value.index;
                editor.m_paramter_editors = new_value.ees;
                editor.m_out_name_editors = new_value.ones;

                if (editor.m_node.inspector_enabled) {
                    if (old_value.ees != null) {
                        foreach (var ee in old_value.ees) {
                            ee.on_inspector_disable();
                        }
                    }
                    if (old_value.ones != null) {
                        foreach (var one in old_value.ones) {
                            one.on_inspector_disable();
                        }
                    }
                    if (new_value.ees != null) {
                        foreach (var ee in new_value.ees) {
                            ee.on_inspector_enable();
                        }
                    }
                    if (new_value.ones != null) {
                        foreach (var one in new_value.ones) {
                            one.on_inspector_enable();
                        }
                    }
                }
                editor.notify_changed(false);
            }

        }

    }
}