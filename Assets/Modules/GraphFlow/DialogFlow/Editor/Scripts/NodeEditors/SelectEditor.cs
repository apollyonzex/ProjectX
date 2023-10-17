
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace DialogFlow.Editor.NodeEditors {

    [NodeEditor(typeof(Nodes.Select))]
    public class SelectEditor : DialogNodeEditor {

        public Nodes.Select select_node => (Nodes.Select)m_node;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            var this_node = select_node;
            if (this_node.values == null) {
                this_node.values = new List<Nodes.Select.Output>(1);
                this_node.values.Add(new Nodes.Select.Output());
            } else {
                var index = -1;
                foreach (var output in this_node.values) {
                    output.index = ++index;
                }
            }

            m_list = new ReorderableList(this_node.values, typeof(Nodes.Select.Output));
            m_list.drawHeaderCallback = rect => GUI.Label(rect, "Values");
            m_list.onAddCallback = on_add_item;
            m_list.onRemoveCallback = on_remove_item;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = on_reorder_port;
        }

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is Nodes.Select other) {
                var this_node = select_node;
                other.values = new List<Nodes.Select.Output>(this_node.values.Count);
                for (int i = 0; i < this_node.values.Count; ++i) {
                    other.values.Add(new Nodes.Select.Output());
                }
            }
        }

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            m_list.DoLayoutList();
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            foreach (var output in select_node.values) {
                yield return output;
            }
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            var this_node = select_node;
            for (int i = 0; i < this_node.values.Count; ++i) {
                if (this_node.values[i] == port) {
                    id = i + 1;
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            var this_node = select_node;
            if (id > 0 && id <= this_node.values.Count) {
                port = this_node.values[id - 1];
                return true;
            }
            port = null;
            return false;
        }

        private void on_add_item(ReorderableList _) {
            var this_node = select_node;
            var output = new Nodes.Select.Output() { index = this_node.values.Count };
            var output_view = view.add_dynamic_port(output);

            view.graph.undo.record(new AddItem {
                editor = this,
                output = output,
                output_view = output_view,
            });
            this_node.values.Add(output);
        }

        private void on_remove_item(ReorderableList _) {
            var this_node = select_node;
            var index = m_list.index;
            var output = this_node.values[index];

            var output_view = view.dynamic_output_ports[index];
            var output_conn = output_view.get_connection_array();
            view.dynamic_output_ports.RemoveAt(index);


            this_node.values.RemoveAt(index);
            for (int i = index; i < this_node.values.Count; ++i) {
                this_node.values[i].index = i;
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

        private void on_reorder_port(ReorderableList _, int old_index, int new_index) {
            view.graph.undo.record(new ChangeIndex {
                editor = this,
                old_index = old_index,
                new_index = new_index,
            });
            var this_node = select_node;
            var output_view = view.dynamic_output_ports[old_index];
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, output_view);
            view.size_changed = true;
            var (min, max) = old_index < new_index ? (old_index, new_index) : (new_index, old_index);
            for (int i = min; i <= max; ++i) {
                this_node.values[i].index = i;
            }
        }

        private ReorderableList m_list;


        private void change_index(int old_index, int new_index) {
            var this_node = select_node;
            var output = this_node.values[old_index];
            var output_view = view.dynamic_output_ports[old_index];
            this_node.values.RemoveAt(old_index);
            this_node.values.Insert(new_index, output);
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, output_view);
            view.size_changed = true;
            var (min, max) = old_index < new_index ? (old_index, new_index) : (new_index, old_index);
            for (int i = min; i <= max; ++i) {
                this_node.values[i].index = i;
            }
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            GUI.Label(rect, index.ToString());
        }

        private class AddItem : GraphUndo.ICommand {
            public SelectEditor editor;
            public Nodes.Select.Output output;
            public PortView output_view;

            public void undo() {
                var this_node = editor.select_node;
                var index = this_node.values.Count - 1;
                editor.view.dynamic_output_ports.RemoveAt(index);
                this_node.values.RemoveAt(index);
                editor.view.size_changed = true;
            }

            public void redo() {
                editor.view.dynamic_output_ports.Add((OutputPortView)output_view);
                editor.select_node.values.Add(output);
            }

            public int dirty_count => 1;
        }

        private class RemoveItem : GraphUndo.ICommand {
            public SelectEditor editor;
            public Nodes.Select.Output output;
            public PortView output_view;
            public ConnectionView[] output_conn;
            public int index;

            public void undo() {
                var this_node = editor.select_node;
                this_node.values.Insert(index, output);
                for (int i = index + 1; i < this_node.values.Count; ++i) {
                    this_node.values[i].index = i;
                }
                editor.view.dynamic_output_ports.Insert(index, (OutputPortView)output_view);
                foreach (var conn in output_conn) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public void redo() {
                var this_node = editor.select_node;
                this_node.values.RemoveAt(index);
                for (int i = index; i < this_node.values.Count; ++i) {
                    this_node.values[i].index = i;
                }
                editor.view.dynamic_output_ports.RemoveAt(index);
                foreach (var conn in output_conn) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public int dirty_count => 1;
        }

        private class ChangeIndex : GraphUndo.ICommand {
            public SelectEditor editor;
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