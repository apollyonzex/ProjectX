
using GraphNode;
using GraphNode.Editor;
using UnityEngine;

namespace InvokeFlow.Editor {

    [NodeEditor(typeof(CollectionNodeBase))]
    public class CollectionNodeBaseEditor : InvokeNodeEditor, INamedNodeEditor<CollectionNodeBaseEditor> {

        public int name_index { get; set; }

        public event System.Action<CollectionNodeBaseEditor> on_removed;

        string INamedNodeEditor<CollectionNodeBaseEditor>.name {
            get {
                var node = this.node;
                return $"{node.name}<{node.GetHashCode():X}>";
            }
        }

        public new CollectionNodeBase node => (CollectionNodeBase)m_node;

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
                ge.collections.add(this, false);
            }
        }

        public override void on_add_to_graph() {
            base.on_add_to_graph();
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.collections.add(this, true);
            }
        }

        public override void on_remove_from_graph() {
            base.on_remove_from_graph();
            on_removed?.Invoke(this);
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.collections.remove(this);
            }
        }


        void on_name_changed(PropertyEditor _, bool by_user) {
            if (view.graph.editor is InvokeGraphEditor ge) {
                ge.collections.notify_name_changed(this);
            }
        }
    }
}