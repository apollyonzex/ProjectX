

namespace InvokeFlow {
    public static class Recorder {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
        public delegate void GraphEvent(IContext context, InvokeGraph graph, bool status);
        public delegate void NodeEvent(IContext context, InvokeNode node, bool status);
        public delegate void ContextEvent(IContext context, bool status);

        public static event GraphEvent graph_event;
        public static event NodeEvent node_event;
        public static event ContextEvent context_event;

        static void begin_graph(IContext context, InvokeGraph graph) {
            graph_event?.Invoke(context, graph, true);
        }

        static void end_graph(IContext context, InvokeGraph graph) {
            graph_event?.Invoke(context, graph, false);
        }

        static void begin_node(IContext context, InvokeNode node) {
            node_event?.Invoke(context, node, true);
        }

        static void end_node(IContext context, InvokeNode node) {
            node_event?.Invoke(context, node, false);
        }
#endif
        public static void register_context(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            context_event?.Invoke(context, true);
#endif
        }

        public static void unregister_context(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            context_event?.Invoke(context, false);
#endif
        }


        public struct NodeInvoke : System.IDisposable {
            public NodeInvoke(IContext context, InvokeNode node) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                this.context = context;
                this.node = node;
                begin_node(context, node);
#endif
            }

#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            IContext context;
            InvokeNode node;
#endif
            public void Dispose() {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                end_node(context, node);
#endif
            }
        }

        public struct GraphInvoke : System.IDisposable {
            public GraphInvoke(IContext context, InvokeGraph graph) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                this.context = context;
                this.graph = graph;
                begin_graph(context, graph);
#endif
            }

#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            IContext context;
            InvokeGraph graph;
#endif
            public void Dispose() {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                end_graph(context, graph);
#endif
            }
        }
    }
}