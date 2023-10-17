

using CalcExpr;
using GraphNode.Editor;

namespace BehaviourFlow.Editor {

    [PropertyEditor(typeof(Expression))]
    public class ExpressionEditor : ExpressionEditor<Expression> {

        protected override bool get_external(string name, out ValueType ty, out IExpressionExternal external) {
            if (name.StartsWith("shared_int.", System.StringComparison.Ordinal)) {
                ty = ValueType.Integer;
                external = new ContextSharedInt(name.Substring(11));
                return true;
            }
            if (name.StartsWith("shared_float.", System.StringComparison.Ordinal)) {
                ty = ValueType.Floating;
                external = new ContextSharedFloat(name.Substring(13));
                return true;
            }
            var ge = m_graph as BehaviourTreeEditor;
            if (name.StartsWith("const.", System.StringComparison.Ordinal)) {
                if (ge.try_get_constant(name.Substring(6), out var constant)) {
                    ty = constant.type;
                    external = constant;
                    return true;
                }
            }
            var graph = m_graph.graph as BehaviourTree;
            var dict = GraphNode.Editor.Utility.get_expression_externals(graph.context_type);
            if (dict.TryGetValue(name, out var value)) {
                external = value;
                ty = external.ret_type;
                return true;
            }
            ty = ValueType.Unknown;
            external = null;
            return false;
        }
    }

}