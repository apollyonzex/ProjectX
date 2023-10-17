
using CalcExpr;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {

    public interface IPropertyEditorWithReturnType {
        System.Type return_type { get; }
    }

    public abstract class ActionEditorBase : GenericPropertyEditor, IPropertyEditorWithContextType, IPropertyEditorWithReturnType {

        public abstract Action create_action();
        public abstract ExpressionEditorBase create_parameter_editor();

        public System.Type context_type {
            get => m_context_type;
            set => set_context_type(value, false);
        }

        public void set_context_type(System.Type context_type, bool init) {
            if (init) {
                m_context_type = context_type;
            } else if (m_context_type != context_type) {
                m_context_type = context_type;
                reset();
            }
        }

        public System.Type method_context_type => m_context_type != null ? m_context_type : m_graph.graph.context_type;

        public System.Type expected_return {
            get => m_expected_return;
            set => set_expected_return(value, false);
        }

        public void set_expected_return(System.Type ret_type, bool init) {
            if (init) {
                m_expected_return = ret_type;
            } else if (m_expected_return != ret_type) {
                m_expected_return = ret_type;
                reset();
            }
        }

        public void reset() {
            if (m_graph == null || m_graph.view.undo.operating) {
                return;
            }
            var target = get_value() as Action;
            if (target != null) {
                var cmd = new ChangeTarget {
                    editor = this,
                    old_value = (target, m_index, m_paramter_editors),
                    new_value = (null, 0, null),
                };
                if (m_paramter_editors != null && m_node.inspector_enabled) {
                    foreach (var ee in m_paramter_editors) {
                        ee.on_inspector_disable();
                    }
                }
                set_value(null);
                m_paramter_editors = null;
                m_index = 0;
                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                cmd.associated = undo.cancel_group();
                undo.record(cmd);
            }
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init_attr();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init_attr();
        }

        public override void on_node_add_to_graph() {
            base.on_node_add_to_graph();
            init();
        }

        public override void on_graph_open() {
            base.on_graph_open();
            init();
        }


        public override void on_inspector_disable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_disable();
                }
            }
        }

        public override void on_inspector_enable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_enable();
                }
            }
        }

        public override void on_inspector_gui() {
            string[] names;
            ActionMethod[] methods;
            if (m_expected_return != null) {
                Utility.get_context_methods(method_context_type, m_expected_return, out names, out methods);
            } else {
                Utility.get_context_methods(method_context_type, out names, out methods);
            }


            var new_index = EditorGUILayout.Popup(name, m_index, names);
            if (new_index != m_index) {
                var new_method = methods[new_index];
                Action new_target = null;
                ExpressionEditorBase[] new_ees = null;
                if (new_method != null) {
                    new_target = create_action();
                    new_target.method = new_method;
                    new_target.parameters = new IExpression[new_method.parameter_types.Length];
                    new_ees = generate_parameter_editors(new_target, true);
                }
                var cmd = new ChangeTarget {
                    editor = this,
                    old_value = (get_value() as Action, m_index, m_paramter_editors),
                    new_value = (new_target, new_index, new_ees),
                };
                if (m_paramter_editors != null) {
                    foreach (var ee in m_paramter_editors) {
                        ee.on_inspector_disable();
                    }
                }
                set_value(new_target);
                m_index = new_index;
                m_paramter_editors = new_ees;
                if (new_ees != null) {
                    foreach (var ee in new_ees) {
                        ee.on_inspector_enable();
                    }
                }
                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                cmd.associated = undo.cancel_group();
                undo.record(cmd);
            }

            if (m_paramter_editors != null && m_paramter_editors.Length != 0) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_gui();
                }
                EditorGUILayout.EndVertical();
            }
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, (get_value() as Action)?.clone());
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                if (m_index != -1) {
                    string[] names;
                    if (m_expected_return != null) {
                        Utility.get_context_methods(method_context_type, m_expected_return, out names, out _);
                    } else {
                        Utility.get_context_methods(method_context_type, out names, out _);
                    }
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

        void init_attr() {
            var attr = m_fi?.GetCustomAttribute<ActionReturnAttribute>();
            if (attr != null) {
                m_expected_return = attr.type;
            }
        }

        void init() {
            var target = get_value() as Action;
            MethodInfo method_info = null;
            if (target != null) {
                target.method.initialize(method_context_type);
                if (target.method.method_info != null) {
                    m_paramter_editors = generate_parameter_editors(target, true);
                }
                method_info = target.method.method_info;
            }
            ActionMethod[] methods;
            if (m_expected_return != null) {
                Utility.get_context_methods(method_context_type, m_expected_return, out _, out methods);
            } else {
                Utility.get_context_methods(method_context_type, out _, out methods);
            }
            for (int i = 0; i < methods.Length; ++i) {
                if (methods[i]?.method_info == method_info) {
                    m_index = i;
                    break;
                }
            }
        }

        private ExpressionEditorBase[] generate_parameter_editors(Action target, bool init) {
            ExpressionEditorBase[] ret;
            if (target.method.method_info != null) {
                var ps = target.method.method_info.GetParameters();
                ret = new ExpressionEditorBase[ps.Length];
                for (int i = 0; i < ps.Length; ++i) {
                    var ee = create_parameter_editor();
                    var pi = ps[i];
                    ee.excepted_type = (ValueType)target.method.parameter_types[i];
                    if (ee.excepted_type == ValueType.Unknown) {
                        ee.build_option = ExpressionEditorBase.BuildOption.Skip;
                    } else if (init) {
                        ee.build_option = ExpressionEditorBase.BuildOption.Manual;
                    }
                    ee.context_type = m_context_type;
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
                        }
                    }
                    target.parameters[i] = ee.target;
                    if (ee.build_option == ExpressionEditorBase.BuildOption.Manual) {
                        ee.build_option = ExpressionEditorBase.BuildOption.Auto;
                        ee.build();
                    }
                    ret[i] = ee;
                }
            } else {
                ret = null;
            }
            return ret;
        }

        System.Type IPropertyEditorWithReturnType.return_type => (get_value() as Action)?.method.method_info.ReturnType;

        private int m_index = -1;
        private ExpressionEditorBase[] m_paramter_editors;
        private System.Type m_context_type;
        private System.Type m_expected_return;

        private class ChangeTarget : GraphUndo.ChangeValue<(Action target, int index, ExpressionEditorBase[] ees)> {

            public ActionEditorBase editor;
            public GraphUndo.CommandGroup associated;

            public override void undo() {
                base.undo();
                associated?.undo();
                editor.notify_changed(false);
            }

            public override void redo() {
                base.redo();
                associated?.redo();
                editor.notify_changed(false);
            }

            protected override void set_value(ref (Action target, int index, ExpressionEditorBase[] ees) old_value, ref (Action target, int index, ExpressionEditorBase[] ees) new_value) {
                editor.set_value(new_value.target);
                editor.m_index = new_value.index;
                editor.m_paramter_editors = new_value.ees;

                if (editor.m_node.inspector_enabled) {
                    if (old_value.ees != null) {
                        foreach (var ee in old_value.ees) {
                            ee.on_inspector_disable();
                        }
                    }
                    if (new_value.ees != null) {
                        foreach (var ee in new_value.ees) {
                            ee.on_inspector_enable();
                        }
                    }
                }
            }

        }
    }

    public abstract class ActionEditorBase<T> : GenericPropertyEditor, IPropertyEditorWithContextType, IPropertyEditorWithReturnType {

        public abstract Action<T> create_action();
        public abstract ExpressionEditorBase create_parameter_editor();

        public System.Type context_type {
            get => m_context_type;
            set {
                if (m_context_type != value) {
                    m_context_type = value;
                    reset();
                }
            }
        }

        public void set_context_type(System.Type context_type, bool init) {
            if (init) {
                m_context_type = context_type;
            } else {
                this.context_type = context_type;
            }
        }

        public System.Type method_context_type => m_context_type != null ? m_context_type : m_graph.graph.context_type;

        public System.Type expected_return {
            get => m_expected_return;
            set {
                if (m_expected_return != value) {
                    m_expected_return = value;
                    reset();
                }
            }
        }

        public void set_expected_return(System.Type ret_type, bool init) {
            if (init) {
                m_expected_return = ret_type;
            } else {
                expected_return = ret_type;
            }
        }

        public void reset() {
            if (m_graph == null || m_graph.view.undo.operating) {
                return;
            }
            var target = get_value() as Action<T>;
            if (target != null) {
                var cmd = new ChangeTarget {
                    editor = this,
                    old_value = (target, m_index, m_paramter_editors),
                    new_value = (null, 0, null),
                };
                if (m_paramter_editors != null && m_node.inspector_enabled) {
                    foreach (var ee in m_paramter_editors) {
                        ee.on_inspector_disable();
                    }
                }
                set_value(null);
                m_paramter_editors = null;
                m_index = 0;
                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                cmd.associated = undo.cancel_group();
                undo.record(cmd);
            }
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init_attr();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init_attr();
        }

        public override void on_graph_open() {
            init();
        }

        public override void on_node_add_to_graph() {
            init();
        }

        public override void on_inspector_disable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_disable();
                }
            }
        }

        public override void on_inspector_enable() {
            if (m_paramter_editors != null) {
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_enable();
                }
            }
        }

        public override void on_inspector_gui() {
            string[] names;
            ActionMethod<T>[] methods;
            if (m_expected_return != null) {
                Utility.get_context_methods(method_context_type, m_expected_return, out names, out methods);
            } else {
                Utility.get_context_methods(method_context_type, out names, out methods);
            }

            var new_index = EditorGUILayout.Popup(name, m_index, names);
            if (new_index != m_index) {
                var new_method = methods[new_index];
                Action<T> new_target = null;
                ExpressionEditorBase[] new_ees = null;
                if (new_method != null) {
                    new_target = create_action();
                    new_target.method = new_method;
                    new_target.parameters = new IExpression[new_method.parameter_types.Length];
                    new_ees = generate_parameter_editors(new_target, true);
                }
                var cmd = new ChangeTarget {
                    editor = this,
                    old_value = (get_value() as Action<T>, m_index, m_paramter_editors),
                    new_value = (new_target, new_index, new_ees),
                };
                if (m_paramter_editors != null) {
                    foreach (var ee in m_paramter_editors) {
                        ee.on_inspector_disable();
                    }
                }
                set_value(new_target);
                m_index = new_index;
                m_paramter_editors = new_ees;
                if (new_ees != null) {
                    foreach (var ee in new_ees) {
                        ee.on_inspector_enable();
                    }
                }
                var undo = m_graph.view.undo;
                undo.begin_group();
                notify_changed(true);
                cmd.associated = undo.cancel_group();
                undo.record(cmd);
            }

            if (m_paramter_editors != null && m_paramter_editors.Length != 0) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                foreach (var ee in m_paramter_editors) {
                    ee.on_inspector_gui();
                }
                EditorGUILayout.EndVertical();
            }
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, (get_value() as Action<T>)?.clone());
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                if (m_index != -1) {
                    string[] names;
                    if (m_expected_return != null) {
                        Utility.get_context_methods<T>(method_context_type, m_expected_return, out names, out _);
                    } else {
                        Utility.get_context_methods<T>(method_context_type, out names, out _);
                    }
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

        void init_attr() {
            var attr = m_fi?.GetCustomAttribute<ActionReturnAttribute>();
            if (attr != null) {
                m_expected_return = attr.type;
            }
        }


        void init() {
            var target = get_value() as Action<T>;
            MethodInfo method_info = null;
            if (target != null) {
                target.method.initialize(method_context_type);
                if (target.method.method_info != null) {
                    m_paramter_editors = generate_parameter_editors(target, false);
                }
                method_info = target.method.method_info;
            }

            ActionMethod<T>[] methods;
            if (m_expected_return != null) {
                Utility.get_context_methods(method_context_type, m_expected_return, out _, out methods);
            } else {
                Utility.get_context_methods(method_context_type, out _, out methods);
            }
            for (int i = 0; i < methods.Length; ++i) {
                if (methods[i]?.method_info == method_info) {
                    m_index = i;
                    break;
                }
            }
        }

        private ExpressionEditorBase[] generate_parameter_editors(Action<T> target, bool init) {
            ExpressionEditorBase[] ret;
            if (target.method.method_info != null) {
                var ps = target.method.method_info.GetParameters();
                ret = new ExpressionEditorBase[target.method.parameter_types.Length];
                for (int i = 0; i < ret.Length; ++i) {
                    var ee = create_parameter_editor();
                    var pi = ps[i + 1];
                    ee.excepted_type = (ValueType)target.method.parameter_types[i];
                    if (ee.excepted_type == ValueType.Unknown) {
                        ee.build_option = ExpressionEditorBase.BuildOption.Skip;
                    } else if (init) {
                        ee.build_option = ExpressionEditorBase.BuildOption.Manual;
                    }
                    ee.context_type = m_context_type;
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
                        }
                    }
                    target.parameters[i] = ee.target;
                    if (ee.build_option == ExpressionEditorBase.BuildOption.Manual) {
                        ee.build_option = ExpressionEditorBase.BuildOption.Auto;
                        ee.build();
                    }
                    ret[i] = ee;
                }
            } else {
                ret = null;
            }
            return ret;
        }

        System.Type IPropertyEditorWithReturnType.return_type => (get_value() as Action<T>)?.method.method_info.ReturnType;

        private int m_index = -1;
        private ExpressionEditorBase[] m_paramter_editors;
        private System.Type m_context_type;
        private System.Type m_expected_return;

        private class ChangeTarget : GraphUndo.ChangeValue<(Action<T> target, int index, ExpressionEditorBase[] ees)> {

            public ActionEditorBase<T> editor;
            public GraphUndo.CommandGroup associated;

            public override void undo() {
                base.undo();
                associated?.undo();
                editor.notify_changed(false);
            }

            public override void redo() {
                base.redo();
                associated?.redo();
                editor.notify_changed(false);
            }

            protected override void set_value(ref (Action<T> target, int index, ExpressionEditorBase[] ees) old_value, ref (Action<T> target, int index, ExpressionEditorBase[] ees) new_value) {
                editor.set_value(new_value.target);
                editor.m_index = new_value.index;
                editor.m_paramter_editors = new_value.ees;

                if (editor.m_node.inspector_enabled) {
                    if (old_value.ees != null) {
                        foreach (var ee in old_value.ees) {
                            ee.on_inspector_disable();
                        }
                    }
                    if (new_value.ees != null) {
                        foreach (var ee in new_value.ees) {
                            ee.on_inspector_enable();
                        }
                    }
                }

            }

        }
    }
}