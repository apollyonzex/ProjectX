

using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(ElementNode))]
    public class PropertyElementNodeEditor : PropertyNamedNodeEditor<ElementNodeEditor> {

        public override NodeEditorList<ElementNodeEditor> node_editor_list => (m_graph as InvokeGraphEditor).elements;

        protected override string get_display_in_body() {
            if (m_value_editor == null) {
                return "Default";
            }
            return $"'{m_value_editor.node.name}'";
        }
    }


    [PropertyEditor(typeof(ElementNode[]))]
    public class PropertyElementNodeArrayEditor : GenericPropertyEditor {

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            init_element_list();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            init_element_list();
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, (m_fi.GetValue(m_obj) as ElementNode[])?.Clone());
            }
        }

        public override void on_graph_open() {
            foreach (var ene in m_element_editors) {
                ene.on_graph_open();
            }
        }

        public override void on_node_add_to_graph() {
            foreach (var ene in m_element_editors) {
                ene.on_node_add_to_graph();
            }
        }

        public override void on_node_remove_from_graph() {
            foreach (var ene in m_element_editors) {
                ene.on_node_remove_from_graph();
            }
        }

        public override void on_inspector_disable() {
            foreach (var ene in m_element_editors) {
                ene.on_inspector_disable();
            }
        }

        public override void on_inspector_enable() {
            foreach (var ene in m_element_editors) {
                ene.on_inspector_enable();
            }
        }

        public override void on_inspector_gui() {
            var lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 32;
            m_element_list.DoLayoutList();
            EditorGUIUtility.labelWidth = lw;
        }

        void init_element_list() {
            var elements = get_value() as ElementNode[];
            if (elements == null) {
                m_element_editors = new List<PropertyElementNodeEditor>();
            } else {
                m_element_editors = new List<PropertyElementNodeEditor>(elements.Length);
                int index = -1;
                foreach (var en in elements) {
                    var ene = new PropertyElementNodeEditor();
                    ene.attach(en, null, m_graph, null, $"_{++index}");
                    ene.on_changed += on_element_changed;
                    m_element_editors.Add(ene);
                }
            }
            m_element_list = new ReorderableList(m_element_editors, typeof(PropertyElementNodeEditor), true, true, true, true);
            m_element_list.elementHeight = EditorGUIUtility.singleLineHeight;
            m_element_list.drawHeaderCallback = draw_element_list_header;
            m_element_list.onAddCallback = on_add_element;
            m_element_list.onRemoveCallback = on_remove_element;
            m_element_list.onReorderCallbackWithDetails = on_reorder_element;
            m_element_list.drawElementCallback = draw_element;

        }

        private void on_reorder_element(ReorderableList list, int oldIndex, int newIndex) {
            var elements = get_value() as ElementNode[];
            for (int i = 0; i < elements.Length; ++i) {
                var ene = m_element_editors[i];
                ene._set_name($"_{i}");
                elements[i] = ene.get_value() as ElementNode;
            }

            m_graph.view.undo.record(new ReorderElement {
                obj = this,
                old_value = oldIndex,
                new_value = newIndex,
            });
            notify_changed(true);
        }

        private void on_element_changed(PropertyEditor pe, bool by_user) {
            for (int i = 0; i < m_element_editors.Count; ++i) {
                var ene = m_element_editors[i];
                if (ene == pe) {
                    (get_value() as ElementNode[])[i] = ene.get_value() as ElementNode;
                    notify_changed(true);
                    break;
                }
            }
        }

        private void draw_element_list_header(Rect rect) {
            GUI.Label(rect, name);
        }

        private void draw_element(Rect rect, int index, bool isActive, bool isFocused) {
            var ene = m_element_editors[index];
            ene.on_list_item_gui(rect);
        }

        private void on_add_element(ReorderableList list) {
            var ene = new PropertyElementNodeEditor();
            ene.attach(null, null, m_graph, null, $"_{m_element_editors.Count}");
            ene.on_changed += on_element_changed;
            ene.on_node_add_to_graph();
            if (m_node.inspector_enabled) {
                ene.on_inspector_enable();
            }

            m_element_editors.Add(ene);

            flush();

            var undo = m_graph.view.undo;
            undo.begin_group();
            notify_changed(true);
            undo.record(new AddElement(this, ene, undo.cancel_group()));

            if (m_node is ExpressionContextNodeEditor ecn) {
                ecn.notify_referenced_elements_changed();
            }
        }

        private void on_remove_element(ReorderableList list) {
            var index = m_element_list.index;
            var ene = m_element_editors[index];
            if (m_node.inspector_enabled) {
                ene.on_inspector_disable();
            }
            ene.on_node_remove_from_graph();
            
            m_element_editors.RemoveAt(index);

            for (int i = index; i < m_element_editors.Count; ++i) {
                m_element_editors[i]._set_name($"_{i}");
            }

            flush();

            var undo = m_graph.view.undo;
            undo.begin_group();
            notify_changed(true);
            undo.record(new RemoveElement(this, ene, index, undo.cancel_group()));

            if (m_node is ExpressionContextNodeEditor ecn) {
                ecn.notify_referenced_elements_changed();
            }
        }

        void flush() {
            ElementNode[] elements;
            if (m_element_editors.Count != 0) {
                elements = new ElementNode[m_element_editors.Count];
                for (int i = 0; i < elements.Length; ++i) {
                    elements[i] = m_element_editors[i].get_value() as ElementNode;
                }
            } else {
                elements = null;
            }
            set_value(elements);
        }

        ReorderableList m_element_list;
        List<PropertyElementNodeEditor> m_element_editors;

        class AddElement : GraphUndo.ICommand {
            public readonly PropertyElementNodeArrayEditor obj;
            public readonly PropertyElementNodeEditor ene;
            public readonly GraphUndo.CommandGroup associated;

            public AddElement(PropertyElementNodeArrayEditor obj, PropertyElementNodeEditor ene, GraphUndo.CommandGroup associated) {
                this.obj = obj;
                this.ene = ene;
                this.associated = associated;
            }

            public void undo() {
                if (obj.m_node.inspector_enabled) {
                    ene.on_inspector_disable();
                }
                ene.on_node_remove_from_graph();

                obj.m_element_editors.RemoveAt(obj.m_element_editors.Count - 1);
                obj.flush();
                associated?.undo();
                obj.notify_changed(false);
                if (obj.m_node is ExpressionContextNodeEditor ecn) {
                    ecn.notify_referenced_elements_changed();
                }
            }

            public void redo() {
                ene.on_node_add_to_graph();
                if (obj.m_node.inspector_enabled) {
                    ene.on_inspector_enable();
                }
                
                obj.m_element_editors.Add(ene);
                obj.flush();
                associated?.redo();
                obj.notify_changed(false);
                if (obj.m_node is ExpressionContextNodeEditor ecn) {
                    ecn.notify_referenced_elements_changed();
                }
            }

            public int dirty_count => 1;
        }

        class RemoveElement : GraphUndo.ICommand {
            public readonly PropertyElementNodeArrayEditor obj;
            public readonly PropertyElementNodeEditor ene;
            public readonly GraphUndo.CommandGroup associated;
            public readonly int index;

            public RemoveElement(PropertyElementNodeArrayEditor obj, PropertyElementNodeEditor ene, int index, GraphUndo.CommandGroup associated) {
                this.obj = obj;
                this.ene = ene;
                this.index = index;
                this.associated = associated;
            }

            public void undo() {
                ene.on_node_add_to_graph();
                if (obj.m_node.inspector_enabled) {
                    ene.on_inspector_enable();
                }
                obj.m_element_editors.Insert(index, ene);
                for (int i = index + 1; i < obj.m_element_editors.Count; ++i) {
                    obj.m_element_editors[i]._set_name($"_{i}");
                }
                obj.flush();
                associated?.undo();
                obj.notify_changed(false);
                if (obj.m_node is ExpressionContextNodeEditor ecn) {
                    ecn.notify_referenced_elements_changed();
                }
            }

            public void redo() {
                if (obj.m_node.inspector_enabled) {
                    ene.on_inspector_disable();
                }
                ene.on_node_remove_from_graph();
                obj.m_element_editors.RemoveAt(index);
                for (int i = index; i < obj.m_element_editors.Count; ++i) {
                    obj.m_element_editors[i]._set_name($"_{i}");
                }
                obj.flush();
                associated?.redo();
                obj.notify_changed(false);
                if (obj.m_node is ExpressionContextNodeEditor ecn) {
                    ecn.notify_referenced_elements_changed();
                }
            }

            public int dirty_count => 1;
        }

        class ReorderElement : GraphUndo.ChangeValue<int> {
            public PropertyElementNodeArrayEditor obj;

            protected override void set_value(ref int old_value, ref int new_value) {
                var item = obj.m_element_editors[old_value];
                obj.m_element_editors.RemoveAt(old_value);
                obj.m_element_editors.Insert(new_value, item);
                var elements = obj.get_value() as ElementNode[];
                for (int i = 0; i < elements.Length; ++i) {
                    var ene = obj.m_element_editors[i];
                    ene._set_name($"_{i}");
                    elements[i] = ene.get_value() as ElementNode;
                }
                obj.notify_changed(false);
            }
        }
    }
}