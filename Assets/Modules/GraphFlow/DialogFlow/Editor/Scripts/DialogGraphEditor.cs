
using GraphNode;
using GraphNode.Editor;
using CalcExpr;
using UnityEngine;
using System.Collections.Generic;

namespace DialogFlow.Editor {

    [GraphEditor(typeof(DialogGraph))]
    public class DialogGraphEditor : GenericGraphEditor {

        public new DialogGraph graph => base.graph as DialogGraph;

        public override void attach(Graph graph, GraphView view) {
            base.attach(graph, view);

            if (try_get_property("constants", out var pe)) {
                pe.on_changed += (_pe, _) => rebuild_constant_indices();
            }

            build_constant_indices();
            var this_graph = this.graph;
            if (this_graph.externals == null) {
                this_graph.externals = new Dictionary<string, Nodes.External>();
            }
            if (this_graph.option_state_externals == null) {
                this_graph.option_state_externals = new Dictionary<string, Nodes.OptionState_External>();
            }
            if (this_graph.options_externals == null) {
                this_graph.options_externals = new Dictionary<string, Nodes.PageOptions_External>();
            }
        }

        public override Color query_node_port_color(NodePort port) {
            if (port.sign_ret == typeof(OptionState)) {
                return new Color32(199, 210, 40, 255);
            }
            if (port.sign_ret == typeof(Nodes.PageOptionsBase)) {
                return new Color32(40, 128, 210, 255);
            }
            return base.query_node_port_color(port);
        }

        public override void on_open() {
            m_pages = new List<PageNodeBaseEditor>();
            m_pages.Add(null);
            m_page_names_dirty = true;
            base.on_open();
        }

        public void add_page(PageNodeBaseEditor page_node) {
            page_node.page_index = m_pages.Count;
            m_pages.Add(page_node);
            m_page_names_dirty = true;
        }

        public void remove_page(PageNodeBaseEditor page_node) {
            var last = m_pages.Count - 1;
            if (page_node.page_index != last) {
                var last_page = m_pages[last];
                last_page.page_index = page_node.page_index;
                m_pages[page_node.page_index] = last_page;
            }
            m_pages.RemoveAt(last);
            m_page_names_dirty = true;
            on_page_removed?.Invoke(page_node);
        }

        public void update_page_name(PageNodeBaseEditor page_node) {
            if (m_page_names_dirty) {
                build_page_names();
            } else {
                var node = page_node.node;
                m_page_names[page_node.page_index] = generate_page_name(node);
            }
        }

        private List<PageNodeBaseEditor> m_pages;

        public string[] page_names {
            get {
                if (m_page_names_dirty) {
                    build_page_names();
                }
                return m_page_names;
            }
        }

        public event System.Action<PageNodeBaseEditor> on_page_removed;

        public PageNodeBaseEditor get_page(int index) {
            return m_pages[index];
        }

        public PageNodeBaseEditor get_page(PageNodeBase node) {
            for (int i = 0; i < m_pages.Count; ++i) {
                var pe = m_pages[i];
                if (pe?.node == node) {
                    return pe;
                }
            }
            return null;
        }

        public int get_page_index(PageNode node) {
            for (int i = 0; i < m_pages.Count; ++i) {
                if (m_pages[i]?.node == node) {
                    return i;
                }
            }
            return -1;
        }

        public bool try_get_constant(string name, out Constant constant) {
            return m_constant_indices.TryGetValue(name, out constant);
        }

        public void rebuild_constant_indices() {
            m_constant_indices.Clear();
            build_constant_indices();
            foreach (var node in view.nodes) {
                if (node.editor is DialogNodeEditor ne) {
                    ne.rebuild_all_expressions();
                }
            }
        }

        public virtual bool try_get_expression_external(string name, out IExpressionExternal external) {
            external = null;
            return false;
        }

        private void build_constant_indices() {
            foreach (var e in graph.constants) {
                if (string.IsNullOrEmpty(e.name) || m_constant_indices.ContainsKey(e.name)) {
                    continue;
                }
                m_constant_indices.Add(e.name, e);
            }
        }

        private Dictionary<string, Constant> m_constant_indices = new Dictionary<string, Constant>();

        private void build_page_names() {
            m_page_names_dirty = false;
            m_page_names = new string[m_pages.Count];
            m_page_names[0] = "<None>";
            for (int i = 1; i < m_pages.Count; ++i) {
                var node = m_pages[i].node;
                m_page_names[i] = generate_page_name(node);
            }
        }

        private static string generate_page_name(PageNodeBase node) {
            return string.IsNullOrEmpty(node.name) ? "<Anonymous>" : node.name;
            /*
            if (string.IsNullOrEmpty(node.name)) {
                return $"[{node.GetHashCode():X}]";
            }
            return $"{node.name} [{node.GetHashCode():X}]";
            */
        }

        private string[] m_page_names;
        private bool m_page_names_dirty;
    }
}