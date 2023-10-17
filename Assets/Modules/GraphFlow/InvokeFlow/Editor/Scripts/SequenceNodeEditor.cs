
using GraphNode;
using GraphNode.Editor;
using System.Collections.Generic;

using UnityEngine;
using UnityEditorInternal;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(SequenceNode))]
    public class SequenceNodeEditor : InvokeNodeWithInputEditor {

        public SequenceNode sequence_node => (SequenceNode)node;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            var sequence_node = this.sequence_node;
            if (sequence_node.outputs == null) {
                sequence_node.outputs = new List<SequenceNode.OutputPort>();
                sequence_node.outputs.Add(new SequenceNode.OutputPort(1));
                sequence_node.outputs.Add(new SequenceNode.OutputPort(2));
                m_last_id = 2;
            } else {
                foreach (var port in sequence_node.outputs) {
                    m_last_id = Mathf.Max(port.id, m_last_id);
                }
            }
            m_list = new ReorderableList(sequence_node.outputs, typeof(SequenceNode.OutputPort));
            m_list.drawHeaderCallback = rect => GUI.Label(rect, "Ports");
            m_list.onAddCallback = on_add_port;
            m_list.onRemoveCallback = on_remove_port;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = on_reorder_port;

            
        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {  
            foreach (var port in sequence_node.outputs) {
                yield return port;
            }
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            if (port is SequenceNode.OutputPort op) {
                id = op.id;
                return true;
            }
            id = 0;
            return false;
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            foreach (var op in sequence_node.outputs) {
                if (op.id == id) {
                    port = op;
                    return true;
                } 
            }
            port = null;
            return false;
        }

        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is SequenceNode other) {
                var sequence_node = this.sequence_node;
                other.outputs = new List<SequenceNode.OutputPort>(sequence_node.outputs.Count);
                foreach (var port in sequence_node.outputs) {
                    other.outputs.Add(new SequenceNode.OutputPort(port.id));
                }
            }
        }

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            m_list.DoLayoutList();
            if (GUILayout.Button("Reassign Ids")) {
                reassign_ids();
            }
        }

        private ReorderableList m_list;
        private int m_last_id = 0;

        private void on_add_port(ReorderableList _) {
            var port = new SequenceNode.OutputPort(++m_last_id);
            var port_view = view.add_dynamic_port(port);
            view.graph.undo.record(new AddPort {
                editor = this,
                port_view = port_view,
            });
            sequence_node.outputs.Add(port);
        }

        private void on_remove_port(ReorderableList _) {
            var index = m_list.index;
            var port_view = view.dynamic_output_ports[index];
            var conn = port_view.get_first_connection();
            view.dynamic_output_ports.RemoveAt(index);
            sequence_node.outputs.RemoveAt(index);
            view.size_changed = true;
            if (conn != null) {
                view.graph.remove_connection_unchecked(conn);
            }
            view.graph.undo.record(new RemovePort {
                editor = this,
                port_view = port_view,
                connection = conn,
                index = index,
            });
        }

        private void on_reorder_port(ReorderableList _, int old_index, int new_index) {
            view.graph.undo.record(new ChangeIndex {
                editor = this,
                old_index = old_index,
                new_index = new_index,
            });
            var port_view = view.dynamic_output_ports[old_index];
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, port_view);
            view.size_changed = true;
        }

        private void change_index(int old_index, int new_index) {
            var seq = sequence_node;
            var port = seq.outputs[old_index];
            var port_view = view.dynamic_output_ports[old_index];
            seq.outputs.RemoveAt(old_index);
            seq.outputs.Insert(new_index, port);
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, port_view);
            view.size_changed = true;
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            var port_view = view.dynamic_output_ports[index];
            var port = port_view.port as SequenceNode.OutputPort;
            if (port_view.connected) {
                GUI.Label(rect, $"{port.id} [CONNECTED]");
            } else {
                GUI.Label(rect, $"{port.id}");
            }
        }

        private void reassign_ids() {
            var node = sequence_node;
            var old_ids = new int[node.outputs.Count];
            for (int i = 0; i < old_ids.Length; ++i) {
                old_ids[i] = node.outputs[i].id;
            }
            view.graph.undo.record(new ReassignIds {
                editor = this,
                old_ids = old_ids,
                old_last_id = m_last_id,
            });
            m_last_id = 0;
            foreach (var port in node.outputs) {
                port.id = ++m_last_id;
            }
        }

        private class AddPort : GraphUndo.ICommand {
            public SequenceNodeEditor editor;
            public PortView port_view;

            public void undo() {
                --editor.m_last_id;
                var node = editor.sequence_node;
                var index = node.outputs.Count - 1;
                editor.view.dynamic_output_ports.RemoveAt(index);
                editor.sequence_node.outputs.RemoveAt(index);
                editor.view.size_changed = true;
            }

            public void redo() {
                ++editor.m_last_id;
                editor.view.dynamic_output_ports.Add((OutputPortView)port_view);
                editor.sequence_node.outputs.Add((SequenceNode.OutputPort)port_view.port);
            }

            public int dirty_count => 1;
        }

        private class RemovePort : GraphUndo.ICommand {
            public SequenceNodeEditor editor;
            public PortView port_view;
            public ConnectionView connection;
            public int index;

            public void undo() {
                editor.sequence_node.outputs.Insert(index, (SequenceNode.OutputPort)port_view.port);
                editor.view.dynamic_output_ports.Insert(index, (OutputPortView)port_view);
                if (connection != null) {
                    editor.view.graph.add_connection_unchecked(connection);
                }
                editor.view.size_changed = true;
            }

            public void redo() {
                editor.sequence_node.outputs.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                if (connection != null) {
                    editor.view.graph.remove_connection_unchecked(connection);
                }
                editor.view.size_changed = true;
            }

            public int dirty_count => 1;
        }

        private class ChangeIndex : GraphUndo.ICommand {
            public SequenceNodeEditor editor;
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

        private class ReassignIds : GraphUndo.ICommand {
            public SequenceNodeEditor editor;
            public int[] old_ids;
            public int old_last_id;

            public void undo() {
                var node = editor.sequence_node;
                for (int i = 0; i < old_ids.Length; ++i) {
                    node.outputs[i].id = old_ids[i];
                }
                editor.m_last_id = old_last_id;
            }

            public void redo() {
                var node = editor.sequence_node;
                var last_id = 0;
                foreach (var port in node.outputs) {
                    port.id = ++last_id;
                }
                editor.m_last_id = last_id;
            }

            public int dirty_count => 1;
        }
    }

}