
using CalcExpr;

namespace GraphNode {

    public interface IExpression {
        string content { get; }
        IExpression clone();
        bool calc(object context, System.Type context_type, out int ret);
        bool calc(object context, System.Type context_type, out float ret);
        bool calc(object context, System.Type context_type, out bool ret);
    }

    [System.Serializable]
    public abstract class ExpressionBase : IExpression {

        public string content;

        public uint[] code;

        public ExpressionFunction[] functions;

        public uint? constant;

        public abstract IExpression clone();

        public bool calc(object context, System.Type context_type, out int ret) {
            if (constant.HasValue) {
                ret = (int)constant.Value;
                return true;
            }
            if (code == null) {
                ret = 0;
                return true;
            }
            var cx = ExpressionCalculator.instance;
            if (!calc_without_result(cx, context, context_type)) {
                ret = 0;
                return false;
            }
            return cx.get_result(out ret);
        }

        public bool calc(object context, System.Type context_type, out float ret) {
            if (constant.HasValue) {
                ret = Utility.convert_float_from(constant.Value);
                return true;
            }
            if (code == null) {
                ret = 0;
                return true;
            }
            var cx = ExpressionCalculator.instance;
            if (!calc_without_result(cx, context, context_type)) {
                ret = 0;
                return false;
            }
            return cx.get_result(out ret);
        }

        public bool calc(object context, System.Type context_type, out bool ret) {
            if (constant.HasValue) {
                ret = constant.Value != 0;
                return true;
            }
            if (code == null) {
                ret = false;
                return true;
            }
            var cx = ExpressionCalculator.instance;
            if (!calc_without_result(cx, context, context_type)) {
                ret = false;
                return false;
            }
            return cx.get_result(out ret);
        }

        public virtual void reset() {
            code = null;
            functions = null;
            constant = null;
        }

        protected abstract void apply_externals(Calculator calculator, object context, System.Type context_type);

        protected bool calc_without_result(Calculator calculator, object context, System.Type context_type) {
            if (!calculator.attach(code)) {
                return false;
            }
            apply_externals(calculator, context, context_type);

            if (functions != null) {
                for (int i = 0; i < functions.Length; ++i) {
                    functions[i].initialize(context_type);
                }
            }

            ExpressionFunction.fns = functions;
            ExpressionFunction.obj = context;
            var ret = calculator.run(ExpressionFunction.entry);
            ExpressionFunction.fns = null;
            ExpressionFunction.obj = null;
            return ret;
        }

        string IExpression.content => content;
    }

    [System.Serializable]
    public abstract class ExpressionBase<E> : ExpressionBase where E : ExpressionBase<E>, new() {

        public override IExpression clone() {
            return clone_to(new E());
        }

        protected virtual E clone_to(E other) {
            other.content = content;
            return other;
        }
    }

    public static class ExpressionCalculator {

        public static Calculator instance {
            get {
                if (s_instance == null) {
                    s_instance = new Calculator();
                }
                return s_instance;
            }
        }

        private static Calculator s_instance = null;
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ExpressionTypeAttribute : System.Attribute {
        public ValueType type { get; }
        public ExpressionTypeAttribute(ValueType type) {
            this.type = type;
        }
    }

}