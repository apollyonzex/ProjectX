
using UnityEngine;
using System.Collections.Generic;

namespace GraphNode.Editor {

    public class GraphUndo {

        public interface ICommand {
            void undo();
            void redo();
            int dirty_count { get; }
        }

        public class CommandGroup : ICommand {
            public void undo() {
                for (int i = m_commands.Count - 1; i >= 0; --i) {
                    m_commands[i].undo();
                }
            }

            public void redo() {
                foreach (var cmd in m_commands) {
                    cmd.redo();
                }
            }

            public void add(ICommand cmd) {
                m_commands.Add(cmd);
                dirty_count += cmd.dirty_count;
            }

            public ICommand this[int index] {
                get => m_commands[index];
            }

            public int count => m_commands.Count;

            public int dirty_count { get; private set; } = 0;

            private List<ICommand> m_commands = new List<ICommand>();
           
        }

        public abstract class ChangeValue<T> : ICommand {
            public T old_value;
            public T new_value;

            public int dirty_count => 1;

            protected abstract void set_value(ref T old_value, ref T new_value);

            public virtual void undo() {
                set_value(ref new_value, ref old_value);
            }

            public virtual void redo() {
                set_value(ref old_value, ref new_value);
            }
        }

        public class ChangeField<T> : ChangeValue<T> {
            
            public object obj { get; }
            public System.Reflection.FieldInfo fi { get; }

            protected override void set_value(ref T old_value, ref T new_value) {
                fi.SetValue(obj, new_value);
            }

            public ChangeField(object obj, System.Reflection.FieldInfo fi, T old_value, T new_value) {
                this.obj = obj;
                this.fi = fi;
                this.old_value = old_value;
                this.new_value = new_value;
            }
        }

        public class NodePosition : ChangeValue<Vector2> {
            public NodeView node;
            
            public NodePosition(NodeView node) {
                this.node = node;
                old_value = node.position;
            }

            public void update_new_value() {
                new_value = node.position;
            }

            public bool validate() {
                return new_value != old_value;
            }

            protected override void set_value(ref Vector2 old_value, ref Vector2 new_value) {
                node.position = new_value;
                node.on_layout_for_recull();
            }
        }

        public class MoveNodes : ICommand { 
            public MoveNodes(List<NodeView> nodes) {
                m_nodes = new NodePosition[nodes.Count];
                for (int i = 0; i < m_nodes.Length; ++i) {
                    m_nodes[i] = new NodePosition(nodes[i]);
                }
            }

            public void update_new_values() {
                foreach (var node in m_nodes) {
                    node.update_new_value();
                }
            }

            public bool validate() {
                foreach (var node in m_nodes) {
                    if (node.validate()) {
                        return true;
                    }
                }
                return false;
            }

            public void undo() {
                foreach (var node in m_nodes) {
                    node.undo();
                }
            }

            public void redo() {
                foreach (var node in m_nodes) {
                    node.redo();
                }
            }

            public int dirty_count => 1;

            private NodePosition[] m_nodes;
        }

        public class AddNode : ICommand {
            public NodeView node;

            public AddNode(NodeView node) {
                this.node = node;
            }

            public void undo() {
                node.graph.remove_node_unchecked(node);
            }

            public void redo() {
                node.graph.add_node_without_undo(node, true);
            }

            public int dirty_count => 1;
        }

        public class RemoveNode : ICommand {
            public NodeView node;

            public RemoveNode(NodeView node) {
                this.node = node;
            }

            public void undo() {
                node.graph.add_node_without_undo(node, true);
            }

            public void redo() {
                node.graph.remove_node_unchecked(node);
            }

            public int dirty_count => 1;
        }

        public class CreateConnection : ICommand {
            public ConnectionView new_connection;
            public ConnectionView old_connection;
            public ConnectionView removed;

            public CreateConnection(ConnectionView new_connection, ConnectionView old_connection, ConnectionView removed) {
                this.old_connection = old_connection;
                this.new_connection = new_connection;
                this.removed = removed;
            }

            public void undo() {
                var graph = new_connection.input.node.graph;
                graph.remove_connection_unchecked(new_connection);
                if (old_connection != null) {
                    graph.add_connection_unchecked(old_connection);
                }
                if (removed != null) {
                    graph.add_connection_unchecked(removed);
                }
            }

            public void redo() {
                var graph = new_connection.input.node.graph;
                if (removed != null) {
                    graph.remove_connection_unchecked(removed);
                }
                if (old_connection != null) {
                    graph.remove_connection_unchecked(old_connection);
                }
                graph.add_connection_unchecked(new_connection);
            }

            public int dirty_count => 1;
        }

        public class DestroyConnection : ICommand {
            public ConnectionView connection;

            public DestroyConnection(ConnectionView connection) {
                this.connection = connection;
            }

            public void undo() {
                var graph = connection.input.node.graph;
                graph.add_connection_unchecked(connection);
            }

            public void redo() {
                var graph = connection.input.node.graph;
                graph.remove_connection_unchecked(connection);
            }

            public int dirty_count => 1;
        }

        public class PushGraph : ICommand {
            public GraphView view;
            public GraphInspector inspector;
            public PushGraph(GraphView view, GraphInspector inspector) {
                this.view = view;
                this.inspector = inspector;
            }

            public void undo() {
                view.window.pop_stack();
            }

            public void redo() {
                view.window.push_stack(view, inspector);
            }

            public int dirty_count => 0;
        }

        public class PopGraph : ICommand {
            public GraphView view;
            public GraphInspector inspector;
            public PopGraph(GraphView view, GraphInspector inspector) {
                this.view = view;
                this.inspector = inspector;
            }

            public void undo() {
                view.window.push_stack(view, inspector);
            }

            public void redo() {
                view.window.pop_stack();
            }

            public int dirty_count => 0;
        }

        #region Manager

        public LinkedListNode<ICommand> cursor => m_cursor;
        public int dirty_count => m_dirty_count;

        public void record(ICommand cmd) {
            if (m_operating) {
                Debug.LogError("GraphUndo: can not record during undo/redo");
                return;
            }
            if (m_groups.Count != 0) {
                m_groups.Peek().add(cmd);
                return;
            }

            if (m_cursor != null) {
                while (m_cursor.Next != null) {
                    var node = m_cursor.Next;
                    m_commands.Remove(node);
                }   
            } else {
                m_commands.Clear();
            }
            m_cursor = new LinkedListNode<ICommand>(cmd);
            m_commands.AddLast(m_cursor);
            if (m_commands.Count > 64) {
                m_commands.RemoveFirst();
            }
            m_dirty_count += cmd.dirty_count;
        }

        public bool undo() {
            if (m_operating) {
                return false;
            }
            if (m_cursor == null) {
                return false;
            }
            m_operating = true;
            m_cursor.Value.undo();
            m_dirty_count -= m_cursor.Value.dirty_count;
            m_cursor = m_cursor.Previous;
            m_operating = false;
            operating_done();
            return true;
        }

        public bool redo() {
            if (m_operating) {
                return false;
            }
            LinkedListNode<ICommand> next;
            if (m_cursor != null) {
                next = m_cursor.Next;
            } else {
                next = m_commands.First;
            }
            if (next == null) {
                return false;
            }
            m_cursor = next;
            m_operating = true;
            m_cursor.Value.redo();
            m_dirty_count += m_cursor.Value.dirty_count;
            m_operating = false;
            operating_done();
            return true;
        }

        public void clear() {
            m_commands.Clear();
            m_cursor = null;
            m_dirty_count = 0;
        }

        public void begin_group() {
            if (m_operating) {
                return;
            }
            m_groups.Push(new CommandGroup());
        }

        public void end_group() {
            if (m_operating) {
                return;
            }
            try {
                var group = m_groups.Pop();
                if (group.count != 0) {
                    record(group);
                }
            } catch (System.InvalidOperationException) {
                Debug.LogError("GraphUndo: invalid end_group");
            }
        }

        public CommandGroup cancel_group() {
            if (!m_operating) {
                try {
                    var group = m_groups.Pop();
                    if (group.count != 0) {
                        return group;
                    }
                } catch (System.InvalidOperationException) {
                    Debug.LogError("GraphUndo: invalid cancel_group");
                }
            }
            return null;
        }

        public bool is_last(ICommand cmd) {
            if (m_groups.Count != 0) {
                var group = m_groups.Peek();
                if (group.count == 0) {
                    return false;
                }
                return group[group.count - 1] == cmd;
            }
            return m_cursor != null && m_cursor.Value == cmd && m_cursor.Next == null;
        }

        public bool operating => m_operating;
        public event System.Action operating_delay_call;


        LinkedList<ICommand> m_commands = new LinkedList<ICommand>();
        LinkedListNode<ICommand> m_cursor = null;
        int m_dirty_count = 0;

        Stack<CommandGroup> m_groups = new Stack<CommandGroup>();

        bool m_operating = false;

        void operating_done() {
            if (operating_delay_call != null) {
                operating_delay_call();
                operating_delay_call = null;
            }
        }
        #endregion
    }

}