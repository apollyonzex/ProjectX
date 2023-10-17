using BehaviourFlow.Exports;
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ActionNode : BTChildNode {

        [Display("Method")]
        [ShowInBody]
        public ContextAction method;

        public override BTResult exec(BTExecutorBase executor) {
            if (method == null) {
                return BTResult.success;
            }
            method.invoke(executor.context.context_type, executor.context, executor, out var ret);
            if (ret is IEnumerator<BTResult> e) {
                return BTResult.enumerator(e);
            }
            if (ret is bool r && !r) {
                return BTResult.failed;
            }
            return BTResult.success;
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.Action();
            if (method != null && method.method != null) {
                var _method = new AutoCode.Packets.BehaviourFlowExports.MethodInfo();
                _method.name_index = (ulong)exporter.get_string_index(method.method.method_name);
                _method.args.items = new AutoCode.Packets.BehaviourFlowExports.ParamRef[method.parameters.Length];
                for (int i = 0; i < method.parameters.Length; ++i) {
                    _method.args.items[i] = ((Expression)method.parameters[i]).export_as_param(exporter);
                }
                node.method = _method;
            }
            index = exporter.add_node(node);
            return true;
        }
    }
}