
using GraphNode.Editor;
using CalcExpr;

namespace DialogFlow.Editor {

    [PropertyEditor(typeof(Expression))]
    public class ExpressionEditor : ExpressionEditor<Expression> {

        protected override bool get_external(string name, out ValueType ty, out IExpressionExternal external) {
            var ge = m_graph as DialogGraphEditor;
            if (ge.try_get_constant(name, out var constant)) {
                ty = constant.type;
                external = constant;
                return true;
            }
            var dict = GraphNode.Editor.Utility.get_expression_externals(ge.graph.context_type);
            if (dict.TryGetValue(name, out external)) {
                ty = external.ret_type;
                return true;
            }

            if (m_graph is DialogGraphEditor graph && graph.try_get_expression_external(name, out external)) {
                ty = external.ret_type;
                return true;
            }

            ty = ValueType.Unknown;
            external = null;
            return false;
        }
    }
}