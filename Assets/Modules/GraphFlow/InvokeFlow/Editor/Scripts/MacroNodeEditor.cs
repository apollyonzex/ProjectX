
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(MacroNode))]
    public class MacroNodeEditor : ExpressionContextNodeWithInputEditor {

        public MacroNode macro_node => (MacroNode)base.node;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            var macro_node = this.macro_node;
            if (macro_node.macro_graph_asset.obj != null) {
                m_macro = macro_node.macro_graph_asset.obj.load_graph() as InvokeMacroGraph;
            }
            build_macro(graph);
        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (m_element_editors != null) {
                foreach (var ee in m_element_editors) {
                    ee.on_graph_open();
                }
            }
            if (m_collection_editors != null) {
                foreach (var ce in m_collection_editors) {
                    ce.on_graph_open();
                }
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (m_element_editors != null) {
                foreach (var ee in m_element_editors) {
                    ee.on_node_add_to_graph();
                }
            }
            if (m_collection_editors != null) {
                foreach (var ce in m_collection_editors) {
                    ce.on_node_add_to_graph();
                }
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            if (m_element_editors != null) {
                foreach (var ee in m_element_editors) {
                    ee.on_node_remove_from_graph();
                }
            }
            if (m_collection_editors != null) {
                foreach (var ce in m_collection_editors) {
                    ce.on_node_remove_from_graph();
                }
            }
        }

        public override void on_view_init() {
            m_port_view = view.find_output_port(new NodePropertyPort_InvokeWithVariables(NodePort.IO.Output, typeof(MacroNode).GetProperty("output"), false));
            base.on_view_init();
        }

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is MacroNode other) {
                var this_node = macro_node;
                other.macro_graph_asset = this_node.macro_graph_asset;
                other.output_count = this_node.output_count;
                other.parameters = new Expression[m_param_editors.Count];
                for (int i = 0; i < other.parameters.Length; ++i) {
                    other.parameters[i] = m_param_editors[i].target.clone() as Expression;
                }
                if (m_element_editors != null) {
                    other.elements = new ElementNode[m_element_editors.Length];
                    for (int i = 0; i < m_element_editors.Length; ++i) {
                        other.elements[i] = m_element_editors[i].get_value() as ElementNode;
                    }
                }
                if (m_collection_editors != null) {
                    other.collections = new CollectionNodeBase[m_collection_editors.Length];
                    for (int i = 0; i < m_collection_editors.Length; ++i) {
                        other.collections[i] = m_collection_editors[i].get_value() as CollectionNodeBase;
                    }
                }
            }
        }

        public override void on_body_gui() {
            base.on_body_gui();
            if (m_macro == null) {
                GUILayout.Label("<None>");
            } else {
                bool error = false;
                foreach (var e in m_param_editors) {
                    if (e.err_msg != null) {
                       error = true;
                       break;
                    }
                }
                if (error) {
                    GUILayout.Label("Error", GraphResources.styles.red_label);
                } else {
                    GUILayout.Label(macro_node.macro_graph_asset.obj.name);
                }
            }
        }

        public override void on_node_saving() {
            var node = macro_node;
            if (m_element_editors != null) {
                node.elements = new ElementNode[m_element_editors.Length];
                for (int i = 0; i < m_element_editors.Length; ++i) {
                    node.elements[i] = m_element_editors[i].get_value() as ElementNode;
                }
            } else {
                node.elements = null;
            }
            if (m_collection_editors != null) {
                node.collections = new CollectionNodeBase[m_collection_editors.Length];
                for (int i = 0; i < m_collection_editors.Length; ++i) {
                    node.collections[i] = m_collection_editors[i].get_value() as CollectionNodeBase;
                }
            } else {
                node.collections = null;
            }
        }

        public override void on_inspector_enable() {
            base.on_inspector_enable();
            if (m_element_editors != null) {
                foreach (var e in m_element_editors) {
                    e.on_inspector_enable();
                }
            }
            if (m_collection_editors != null) {
                foreach (var e in m_collection_editors) {
                    e.on_inspector_enable();
                }
            }
            foreach (var e in m_param_editors) {
                e.on_inspector_enable();
            }
        }

        public override void on_inspector_disable() {
            base.on_inspector_disable();
            if (m_element_editors != null) {
                foreach (var e in m_element_editors) {
                    e.on_inspector_disable();
                }
            }
            if (m_collection_editors != null) {
                foreach (var e in m_collection_editors) {
                    e.on_inspector_disable();
                }
            }
            foreach (var e in m_param_editors) {
                e.on_inspector_disable();
            }
        }

        protected override IEnumerator<Variable> enumerate_port_variables(InvokeWithVariables.IPort iv_port) {
            if (m_port_view.port == iv_port) {
                return enumerate_returns();
            }
            return null;
        }

        protected override void on_inspector_gui_inner() {
            var node = macro_node;
            var obj = EditorGUILayout.ObjectField("Macro", node.macro_graph_asset.obj, typeof(InvokeMacroGraphAsset), false) as InvokeMacroGraphAsset;
            if (obj != node.macro_graph_asset.obj) {
                if (obj != null) {
                    var macro = obj.graph;
                    if (!macro.context_type.IsAssignableFrom(view.graph.editor.graph.context_type)) {
                        obj = null;
                    }
                }
                if (obj != node.macro_graph_asset.obj) {
                    var old_value = (node.macro_graph_asset.obj, m_macro, m_param_editors.ToArray(), m_element_editors, m_collection_editors);
                    node.macro_graph_asset.obj = obj;
                    m_macro = obj?.graph;
                    build_macro(view.graph.editor);
                    var new_value = (obj, m_macro, m_param_editors.ToArray(), m_element_editors, m_collection_editors);
                    view.graph.undo.record(new ChangeGraphAsset { editor = this, old_value = old_value, new_value = new_value });
                    build_port_stack_frame(m_port_view, true);
                    if (old_value.m_element_editors != null) {
                        foreach (var e in old_value.m_element_editors) {
                            e.on_inspector_disable();
                            e.on_node_remove_from_graph();
                        }
                    }
                    if (old_value.m_collection_editors != null) {
                        foreach (var e in old_value.m_element_editors) {
                            e.on_inspector_disable();
                            e.on_node_remove_from_graph();
                        }
                    }
                    foreach (var e in old_value.Item3) {
                        e.on_inspector_disable();
                    }
                    if (new_value.m_element_editors != null) {
                        foreach (var e in new_value.m_element_editors) {
                            e.on_node_add_to_graph();
                            e.on_inspector_enable();
                        }
                    }
                    if (new_value.m_collection_editors != null) {
                        foreach (var e in new_value.m_element_editors) {
                            e.on_node_add_to_graph();
                            e.on_inspector_enable();
                        }
                    }
                    foreach (var e in new_value.Item3) {
                        e.on_inspector_enable();
                    }
                }
            }
            if (m_element_editors != null) {
                foreach (var e in m_element_editors) {
                    e.on_inspector_gui();
                }
            }
            if (m_collection_editors != null) {
                foreach (var e in m_collection_editors) {
                    e.on_inspector_gui();
                }
            }
            foreach (var e in m_param_editors) {
                e.on_inspector_gui();
            }
        }

        protected override void on_stack_changed() {
            base.on_stack_changed();
            foreach (var e in m_param_editors) {
                e.build();
            }
        }

        private IEnumerator<Variable> enumerate_returns() {
            if (m_macro == null || m_macro.outputs == null) {
                 return null;
            }
            return m_macro.outputs.enumerate_valid_variables();
        }
        
        private void build_macro(GraphEditor graph) {
            var node = macro_node;
            m_param_editors.Clear();
            m_element_editors = null;
            m_collection_editors = null;
            if (m_macro == null) {
                node.parameters = new Expression[0];
                node.output_count = 0;
            } else {
                if (m_macro.argument_count > 0) {
                    var parameters = new Expression[m_macro.argument_count];
                    for (int i = 0; i < parameters.Length; ++i) {
                        parameters[i] = new Expression();
                    }
                    var c = Mathf.Min(parameters.Length, node.parameters.Length);
                    for (int i = 0; i < c; ++i) {
                        parameters[i].content = node.parameters[i].content;
                    }
                    node.parameters = parameters;
                    int index = 0;
                    var iter = m_macro.inputs.enumerate_valid_variables();
                    while (iter.MoveNext()) {
                        var item = iter.Current;
                        var ee = new ExpressionEditor();
                        ee.excepted_type = (CalcExpr.ValueType)item.type;
                        ee.attach(parameters[index++], null, graph, this, $"{item.name}: {item.type.to_string()}");
                        m_param_editors.Add(ee);
                    }
                    if (!stack_changed) {
                        foreach (var ee in m_param_editors) {
                            ee.build();
                        }
                    }
                } else {
                    node.parameters = new Expression[0];
                }

                node.output_count = m_macro.return_count;

                if (m_macro.external_elements != null && m_macro.external_elements.Count != 0) {
                    m_element_editors = new PropertyElementNodeEditor[m_macro.external_elements.Count];
                    for (int i = 0; i < m_element_editors.Length; ++i) {
                        var ee = new PropertyElementNodeEditor();
                        m_element_editors[i] = ee;
                        ElementNode en = null;
                        if (node.elements != null && i < node.elements.Length) {
                            en = node.elements[i];
                        }
                        ee.attach(en, null, graph, this, m_macro.external_elements[i].name);
                    }
                }

                if (m_macro.external_collections != null && m_macro.external_collections.Count != 0) {
                    m_collection_editors = new PropertyCollectionNodeBaseEditor[m_macro.external_collections.Count];
                    for (int i = 0; i < m_collection_editors.Length; ++i) {
                        var ce = new PropertyCollectionNodeBaseEditor();
                        m_collection_editors[i] = ce;
                        CollectionNodeBase cn = null;
                        if (node.collections != null && i < node.collections.Length) {
                            cn = node.collections[i];
                        }
                        ce.attach(cn, null, graph, this, m_macro.external_collections[i].name);
                    }
                }
            }
        }

        private InvokeMacroGraph m_macro;
        private OutputPortView m_port_view;
        private List<ExpressionEditor> m_param_editors = new List<ExpressionEditor>();
        private PropertyElementNodeEditor[] m_element_editors;
        private PropertyCollectionNodeBaseEditor[] m_collection_editors;


        private class ChangeGraphAsset : GraphUndo.ChangeValue<(InvokeMacroGraphAsset, InvokeMacroGraph, ExpressionEditor[], PropertyElementNodeEditor[], PropertyCollectionNodeBaseEditor[])> {
            public MacroNodeEditor editor;

            protected override void set_value(ref (InvokeMacroGraphAsset, InvokeMacroGraph, ExpressionEditor[], PropertyElementNodeEditor[], PropertyCollectionNodeBaseEditor[]) old_value, ref (InvokeMacroGraphAsset, InvokeMacroGraph, ExpressionEditor[], PropertyElementNodeEditor[], PropertyCollectionNodeBaseEditor[]) new_value) {
                var node = editor.macro_node;
                node.macro_graph_asset.obj = new_value.Item1;
                node.parameters = new Expression[new_value.Item3.Length];
                for (int i = 0; i < node.parameters.Length; ++i) {
                    node.parameters[i] = new_value.Item3[i].target;
                }
                editor.m_param_editors.Clear();
                foreach (var item in new_value.Item3) {
                    editor.m_param_editors.Add(item);
                }
                editor.m_macro = new_value.Item2;
                if (editor.m_macro != null) {
                    node.output_count = editor.m_macro.return_count;
                } else {
                    node.output_count = 0;
                }
                editor.m_element_editors = new_value.Item4;
                editor.m_collection_editors = new_value.Item5;
                editor.build_port_stack_frame(editor.m_port_view, true);
                if (editor.inspector_enabled) {
                    if (old_value.Item4 != null) {
                        foreach (var e in old_value.Item4) {
                            e.on_inspector_disable();
                            e.on_node_remove_from_graph();
                        }
                    }
                    if (old_value.Item5 != null) {
                        foreach (var e in old_value.Item5) {
                            e.on_inspector_disable();
                            e.on_node_remove_from_graph();
                        }
                    }
                    foreach (var e in old_value.Item3) {
                        e.on_inspector_disable();
                    }
                    if (new_value.Item4 != null) {
                        foreach (var e in new_value.Item4) {
                            e.on_node_add_to_graph();
                            e.on_inspector_enable();
                        }
                    }
                    if (new_value.Item5 != null) {
                        foreach (var e in new_value.Item5) {
                            e.on_node_add_to_graph();
                            e.on_inspector_enable();
                        }
                    }
                    foreach (var e in new_value.Item3) {
                        e.on_inspector_enable();
                    }
                }
            }
        }
        
    }
}