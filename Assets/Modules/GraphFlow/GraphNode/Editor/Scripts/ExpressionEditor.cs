

using CalcExpr;

namespace GraphNode.Editor {

    [PropertyEditor(typeof(Expression))]
    public class ExpressionEditor : ExpressionEditor<Expression> {

    }

    public class ExpressionEditor<E> : ExpressionEditorBase where E : Expression<E>, new() {

        public new E target => base.target as E;

        public override ExpressionBase create_expression() {
            return new E();
        }

        protected override void init_external(int count) {
            if (count > 0) {
                m_externals = new IExpressionExternal[count];
            }
        }

        protected override bool get_external(int index, string name, out ValueType ty) {
            return get_external(name, out ty, out m_externals[index]);
        }

        protected override void drop_external() {
            m_externals = null;
        }

        protected override void store_external() {
            target.externals = m_externals;
            m_externals = null;
        }

        protected override void store_compiled_external(int count) {
            if (count > 0) {
                target.externals = new IExpressionExternal[count];
            } else {
                target.externals = null;
            }
        }

        protected override void move_external_to_compiled(int index, int target) {
            this.target.externals[target] = m_externals[index];
        }

        protected virtual bool get_external(string name, out ValueType ty, out IExpressionExternal external) {
            var dict = Utility.get_expression_externals(expression_context_type);
            if (dict.TryGetValue(name, out external)) {
                ty = external.ret_type;
                return true;
            }
            ty = ValueType.Unknown;
            return false;
        }

        IExpressionExternal[] m_externals;
    }
}