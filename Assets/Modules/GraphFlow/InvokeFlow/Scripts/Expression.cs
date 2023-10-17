
using CalcExpr;
using GraphNode;

namespace InvokeFlow {

    [System.Serializable]
    public class Expression : Expression<Expression> {

        public override IExpression clone() {
            return clone_to(new Expression());
        }
    }

    [System.Serializable]
    public class ContextStackLocal : IExpressionExternal {
        public int stack_pos;

        ValueType IExpressionExternal.ret_type => ValueType.Unknown;

        bool IExpressionExternal.get_value(object obj, System.Type obj_type, out object value) {
            value = null;
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (obj is IContext ctx) {
                calculator.set_external(index, ctx.get_stack_int(stack_pos));
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class ContextElementFunction : IExpressionExternal {
        public ExpressionContextNode node;
        public int index;
        public ActionMethod<Element> func;
        public ValueType ty;

        ValueType IExpressionExternal.ret_type => ty;

        public bool get_value(object obj, System.Type obj_type, out object value) {
            value = null;
            if (func != null && obj is IContext ctx && func.invoke(obj_type, obj, ctx.get_element(node.referenced_elements[index]), new IExpression[0], out value)) {
                return true;
            }
            return false;
        }

        bool IExpressionExternal.set_external(Calculator calculator, object obj, System.Type obj_type, int index) {
            if (get_value(obj, obj_type, out var value)) {
                switch (ty) {
                    case ValueType.Integer:
                        if (value is int vi) {
                            calculator.set_external(index, vi);
                            return true;
                        }
                        break;
                    case ValueType.Floating:
                        if (value is float vf) {
                            calculator.set_external(index, vf);
                            return true;
                        }
                        break;
                    case ValueType.Boolean:
                        if (value is bool vb) {
                            calculator.set_external(index, vb);
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
    }
}