
using CalcExpr;

namespace GraphNode {

    [System.Serializable]
    public class Expression<E> : ExpressionBase<E> where E : Expression<E>, new() {

        public IExpressionExternal[] externals;

        public override void reset() {
            base.reset();
            externals = null;
        }

        protected override E clone_to(E other) {
            base.clone_to(other);
            other.externals = externals?.Clone() as IExpressionExternal[];
            return other;
        }

        protected override void apply_externals(Calculator calculator, object context, System.Type context_type) {
            if (externals != null) {
                for (int i = 0; i < externals.Length; ++i) {
                    externals[i].set_external(calculator, context, context_type, i);
                }
            }
        }
    }


    [System.Serializable]
    public class Expression : Expression<Expression> {

    }
}