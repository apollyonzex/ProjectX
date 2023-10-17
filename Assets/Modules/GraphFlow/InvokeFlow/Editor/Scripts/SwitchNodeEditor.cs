
using GraphNode;
using GraphNode.Editor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(SwitchNode))]
    public class SwitchNodeEditor : ExpressionContextNodeWithInputEditor {

        public new SwitchNode node => m_node as SwitchNode;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            var this_node = this.node;
            if (this_node.outputs == null) {
                this_node.outputs = new List<SwitchNode.OutputPort>(1);
                this_node.outputs.Add(new SwitchNode.OutputPort());
            } else {
                var index = -1;
                foreach (var output in this_node.outputs) {
                    output.index = ++index;
                }
            }

            m_list = new ReorderableList(this_node.outputs, typeof(SwitchNode.OutputPort));
            m_list.drawHeaderCallback = rect => GUI.Label(rect, "Values");
            m_list.onAddCallback = on_add_item;
            m_list.onRemoveCallback = on_remove_item;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = on_reorder_port;
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            foreach (var output in node.outputs) {
                yield return output;
            }
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            var node = this.node;
            for (int i = 0; i < node.outputs.Count; ++i) {
                if (node.outputs[i] == port) {
                    id = i + 1;
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            var node = this.node;
            if (id > 0 && id <= node.outputs.Count) {
                port = node.outputs[id - 1];
                return true;
            }
            port = null;
            return false;
        }

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is SwitchNode other) {
                var this_node = this.node;
                other.outputs = new List<SwitchNode.OutputPort>(this_node.outputs.Count);
                for (int i = 0; i < this_node.outputs.Count; ++i) {
                    other.outputs.Add(new SwitchNode.OutputPort());
                }
            }
        }

        protected override void on_inspector_gui_inner() {
            m_list.DoLayoutList();
        }

        void on_add_item(ReorderableList _) {
            var node = this.node;
            var output = new SwitchNode.OutputPort() { index = node.outputs.Count };
            var output_view = view.add_dynamic_port(output);

            node.outputs.Add(output);

            view.graph.undo.record(new AddItem {
                editor = this,
                output = output,
                output_view = output_view,
            });
        }

        void on_remove_item(ReorderableList _) {
            var node = this.node;
            var index = m_list.index;
            var output = node.outputs[index];

            var output_view = view.dynamic_output_ports[index];
            var output_conn = output_view.get_connection_array();
            view.dynamic_output_ports.RemoveAt(index);


            node.outputs.RemoveAt(index);
            for (int i = index; i < node.outputs.Count; ++i) {
                node.outputs[i].index = i;
            }
            view.size_changed = true;
            foreach (var conn in output_conn) {
                view.graph.remove_connection_unchecked(conn);
            }
            view.graph.undo.record(new RemoveItem {
                editor = this,
                output = output,
                output_view = output_view,
                output_conn = output_conn,
                index = index,
            });
        }

        void on_reorder_port(ReorderableList _, int old_index, int new_index) {
            view.graph.undo.record(new ChangeIndex {
                editor = this,
                old_index = old_index,
                new_index = new_index,
            });
            var node = this.node;
            var output_view = view.dynamic_output_ports[old_index];
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, output_view);
            view.size_changed = true;
            var (min, max) = old_index < new_index ? (old_index, new_index) : (new_index, old_index);
            for (int i = min; i <= max; ++i) {
                node.outputs[i].index = i;
            }
        }

        ReorderableList m_list;

        void change_index(int old_index, int new_index) {
            var node = this.node;
            var output = node.outputs[old_index];
            var output_view = view.dynamic_output_ports[old_index];
            node.outputs.RemoveAt(old_index);
            node.outputs.Insert(new_index, output);
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, output_view);
            view.size_changed = true;
            var (min, max) = old_index < new_index ? (old_index, new_index) : (new_index, old_index);
            for (int i = min; i <= max; ++i) {
                node.outputs[i].index = i;
            }
        }

        void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            GUI.Label(rect, index.ToString());
        }

        class AddItem : GraphUndo.ICommand {
            public SwitchNodeEditor editor;
            public SwitchNode.OutputPort output;
            public PortView output_view;

            public void undo() {
                var node = editor.node;
                var index = node.outputs.Count - 1;
                editor.view.dynamic_output_ports.RemoveAt(index);
                node.outputs.RemoveAt(index);
                editor.view.size_changed = true;
            }

            public void redo() {
                editor.view.dynamic_output_ports.Add((OutputPortView)output_view);
                editor.node.outputs.Add(output);
            }

            public int dirty_count => 1;
        }

        class RemoveItem : GraphUndo.ICommand {
            public SwitchNodeEditor editor;
            public SwitchNode.OutputPort output;
            public PortView output_view;
            public ConnectionView[] output_conn;
            public int index;

            public void undo() {
                var node = editor.node;
                node.outputs.Insert(index, output);
                for (int i = index + 1; i < node.outputs.Count; ++i) {
                    node.outputs[i].index = i;
                }
                editor.view.dynamic_output_ports.Insert(index, (OutputPortView)output_view);
                foreach (var conn in output_conn) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public void redo() {
                var node = editor.node;
                node.outputs.RemoveAt(index);
                for (int i = index; i < node.outputs.Count; ++i) {
                    node.outputs[i].index = i;
                }
                editor.view.dynamic_output_ports.RemoveAt(index);
                foreach (var conn in output_conn) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public int dirty_count => 1;
        }

        class ChangeIndex : GraphUndo.ICommand {
            public SwitchNodeEditor editor;
            public int old_index;
            public int new_index;

            public void undo() {
                editor.change_index(new_index, old_index);
            }

            public void redo() {
                editor.change_index(old_index, new_index);
            }

            public int dirty_count => 1;
        }
    }
}