
using GraphNode;
using GraphNode.Editor;
using UnityEngine;

namespace DialogFlow.Editor {

    [NodeEditor(typeof(PageNodeBase))]
    public class PageNodeBaseEditor : DialogNodeEditor {

        public new PageNodeBase node => m_node as PageNodeBase;

        public int page_index { get; set; }

        public DialogGraphEditor graph { get; private set; }

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (graph is DialogGraphEditor ge) {
                this.graph = ge;
                if (try_get_property("name", out var pe)) {
                    pe.on_changed += (_1, _2) => {
                        this.graph.update_page_name(this);
                    };
                }
            }
        }

        public override void on_graph_open() {
            base.on_graph_open();
            graph.add_page(this);
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            graph.add_page(this);
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            graph.remove_page(this);
        }

        public override void on_header_gui(GUIStyle style) {
            var this_node = node;
            if (string.IsNullOrEmpty(this_node.name)) {
                GUILayout.Label("Page", style);
            } else {
                GUILayout.Label($"Page \'{this_node.name}\'", style);
            }
        }
    }
}