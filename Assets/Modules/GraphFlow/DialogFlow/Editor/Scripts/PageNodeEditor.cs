
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using GraphNode;
using GraphNode.Editor;

namespace DialogFlow.Editor {

    [NodeEditor(typeof(PageNode))]
    public class PageNodeEditor : PageNodeBaseEditor {

        public new PageNode node => m_node as PageNode;

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            var this_node = this.node;
            if (this_node.options == null) {
                this_node.options = new List<PageOption>();
            }
            m_list = new ReorderableList(this_node.options, typeof(PageOption));
            m_list.drawHeaderCallback = rect => GUI.Label(rect, "Options");
            m_list.onAddCallback = on_add_option;
            m_list.onRemoveCallback = on_remove_port;
            m_list.drawElementCallback = draw_item;
            m_list.onReorderCallbackWithDetails = on_reorder_port;
            m_list.onSelectCallback = on_select;
        }


        protected override void clone_to(Node node) {
            base.clone_to(node);
            if (node is PageNode other) {
                var page_node = this.node;
                other.options = new List<PageOption>(page_node.options.Count);
                foreach (var option in page_node.options) {                    
                    other.options.Add(new PageOption() { content = option.content.clone() });
                }
            }
        }

        public override void on_inspector_gui() {
            base.on_inspector_gui();
            var graph = this.graph.graph;
           
            if (graph.localizable) {
                var this_node = node;
                if (!string.IsNullOrEmpty(this_node.content.content)) {
                    if (graph.try_localize(this_node.content.content, out var content)) {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.LabelField(content, EditorStyles.wordWrappedLabel);
                        EditorGUILayout.EndVertical();
                    } else {
                        EditorGUILayout.HelpBox("Localize Failed", MessageType.Warning);
                    }
                }
                m_list.DoLayoutList();
                if (m_option_content_editor != null) {
                    EditorGUILayout.PrefixLabel("Option Content");
                    m_option_content_editor.on_inspector_gui();
                    var option_key = (m_option_content_editor.get_value() as DialogText).content;
                    if (!string.IsNullOrEmpty(option_key)) {
                        if (graph.try_localize(option_key, out var content)) {
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            EditorGUILayout.LabelField(content, EditorStyles.wordWrappedLabel);
                            EditorGUILayout.EndVertical();
                        } else {
                            EditorGUILayout.HelpBox("Localize Failed", MessageType.Warning);
                        }
                    }
                }

            } else {
                m_list.DoLayoutList();
                if (m_option_content_editor != null) {
                    EditorGUILayout.PrefixLabel("Option Content");
                    m_option_content_editor.on_inspector_gui();
                }
            }

        }

        public override IEnumerator<NodePort> enumerate_dynamic_ports() {
            foreach (var option in node.options) {
                yield return option.input;
                yield return option.output;
            }
        }

        public override bool get_dynamic_port_id(NodePort port, out int id) {
            for (int i = 0; i < node.options.Count; ++i) {
                var option = node.options[i];
                if (option.input == port) {
                    id = -(i + 1);
                    return true;
                }
                if (option.output == port) {
                    id = i + 1;
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public override bool get_dynamic_port_by_id(int id, out NodePort port) {
            var node = this.node;
            if (id < 0) {
                var index = -id - 1;
                if (index < node.options.Count) {
                    port = node.options[index].input;
                    return true;
                }
            } else if (id > 0) {
                var index = id - 1;
                if (index < node.options.Count) {
                    port = node.options[index].output;
                    return true;
                }
            }
            port = null;
            return false;
        }

        public override void on_inspector_enable() {
            base.on_inspector_enable();
            m_option_content_editor?.on_inspector_enable();
        }

        public override void on_inspector_disable() {
            base.on_inspector_disable();
            m_option_content_editor?.on_inspector_disable();
        }

        private ReorderableList m_list;
        private DialogTextEditor m_option_content_editor;

        private void on_add_option(ReorderableList _) {
            var option = new PageOption { content = new DialogText() };
            var input_view = view.add_dynamic_port(option.input);
            var output_view = view.add_dynamic_port(option.output);

            view.graph.undo.record(new AddOption {
                editor = this,
                option = option,
                input_view = input_view,
                output_view = output_view,
            });
            node.options.Add(option);
        }

        private void on_remove_port(ReorderableList _) {
            var index = m_list.index;
            var option = node.options[index];
           
            var input_view = view.dynamic_input_ports[index];
            var input_conn = input_view.get_connection_array();
            view.dynamic_input_ports.RemoveAt(index);

            var output_view = view.dynamic_output_ports[index];
            var output_conn = output_view.get_connection_array();
            view.dynamic_output_ports.RemoveAt(index);


            node.options.RemoveAt(index);
            view.size_changed = true;
            foreach (var conn in input_conn) {
                view.graph.remove_connection_unchecked(conn);
            }
            foreach (var conn in output_conn) {
                view.graph.remove_connection_unchecked(conn);
            }
            view.graph.undo.record(new RemoveOption {
                editor = this,
                option = option,
                input_view = input_view,
                input_conn = input_conn,
                output_view = output_view,
                output_conn = output_conn,
                index = index,
            });

            --m_list.index;
            m_option_content_editor = null;
        }

        private void on_reorder_port(ReorderableList _, int old_index, int new_index) {
            view.graph.undo.record(new ChangeIndex {
                editor = this,
                old_index = old_index,
                new_index = new_index,
            });
            var input_view = view.dynamic_input_ports[old_index];
            var output_view = view.dynamic_output_ports[old_index];
            view.dynamic_input_ports.RemoveAt(old_index);
            view.dynamic_input_ports.Insert(new_index, input_view);
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, output_view);
            view.size_changed = true;
        }

        private void on_select(ReorderableList _) {
            if (m_list.index == -1) {
                return;
            }
            var option = node.options[m_list.index];
            if (option != m_option_content_editor?.obj) {
                if (inspector_enabled) {
                    m_option_content_editor?.on_inspector_disable();
                }
                m_option_content_editor = new DialogTextEditor();
                m_option_content_editor.attach(option, option.GetType().GetField("content"), view.graph.editor, this, string.Empty);
                if (inspector_enabled) {
                    m_option_content_editor.on_inspector_enable();
                }
            }
        }
        

        private void change_index(int old_index, int new_index) {
            var node = this.node;
            var option = node.options[old_index];
            var input_view = view.dynamic_input_ports[old_index];
            var output_view = view.dynamic_output_ports[old_index];
            node.options.RemoveAt(old_index);
            node.options.Insert(new_index, option);
            view.dynamic_input_ports.RemoveAt(old_index);
            view.dynamic_input_ports.Insert(new_index, input_view);
            view.dynamic_output_ports.RemoveAt(old_index);
            view.dynamic_output_ports.Insert(new_index, output_view);
            view.size_changed = true;
        }

        private void draw_item(Rect rect, int index, bool isActive, bool isFocused) {
            GUI.Label(rect, node.options[index].content.content);
        }

        private class AddOption : GraphUndo.ICommand {
            public PageNodeEditor editor;
            public PageOption option;
            public PortView input_view;
            public PortView output_view;

            public void undo() {
                var node = editor.node;
                var index = node.options.Count - 1;
                if (editor.m_list.index == index) {
                    --editor.m_list.index;
                    editor.m_option_content_editor = null;
                }
                editor.view.dynamic_input_ports.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                node.options.RemoveAt(index);
                editor.view.size_changed = true;
            }

            public void redo() {
                editor.view.dynamic_input_ports.Add((InputPortView)input_view);
                editor.view.dynamic_output_ports.Add((OutputPortView)output_view);
                editor.node.options.Add(option);
            }

            public int dirty_count => 1;
        }

        private class RemoveOption : GraphUndo.ICommand {
            public PageNodeEditor editor;
            public PageOption option;
            public PortView input_view;
            public PortView output_view;
            public ConnectionView[] input_conn;
            public ConnectionView[] output_conn;
            public int index;

            public void undo() {
                editor.node.options.Insert(index, option);
                editor.view.dynamic_input_ports.Insert(index, (InputPortView)input_view);
                editor.view.dynamic_output_ports.Insert(index, (OutputPortView)output_view);
                foreach (var conn in input_conn) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                foreach (var conn in output_conn) {
                    editor.view.graph.add_connection_unchecked(conn);
                }
                editor.view.size_changed = true;
            }

            public void redo() {
                editor.node.options.RemoveAt(index);
                editor.view.dynamic_input_ports.RemoveAt(index);
                editor.view.dynamic_output_ports.RemoveAt(index);
                foreach (var conn in input_conn) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                foreach (var conn in output_conn) {
                    editor.view.graph.remove_connection_unchecked(conn);
                }
                editor.view.size_changed = true;

                if (editor.m_list.index == index) {
                    --editor.m_list.index;
                    editor.m_option_content_editor = null;
                }
            }

            public int dirty_count => 1;
        }

        private class ChangeIndex : GraphUndo.ICommand {
            public PageNodeEditor editor;
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