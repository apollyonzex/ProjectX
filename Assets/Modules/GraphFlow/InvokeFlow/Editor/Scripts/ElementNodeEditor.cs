
using GraphNode;
using GraphNode.Editor;
using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(ElementNode))]
    public class ElementNodeEditor : InvokeNodeEditor, INamedNodeEditor<ElementNodeEditor> {

        public int name_index { get; set; }

        public event System.Action<ElementNodeEditor> on_removed;

        string INamedNodeEditor<ElementNodeEditor>.name {
            get {
                var node = this.node;
                return $"{node.name}<{node.GetHashCode():X}>";
            }
        }

        public new ElementNode node => (ElementNode)m_node;

        public override Color node_color => new Color32(94, 90, 55, 255);

        public override void attach(GraphEditor graph, Node node) {
            base.attach(graph, node);
            if (try_get_property("name", out var pe)) {
                pe.on_changed += on_name_changed;
            }
        }

        public override void on_graph_open() {
            base.on_graph_open();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.elements.add(this, false);
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.elements.add(this, true);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            on_removed?.Invoke(this);
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.elements.remove(this);
            }
        }


        void on_name_changed(PropertyEditor _, bool by_user) {
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.elements.notify_name_changed(this);
            }
        }
    }
}