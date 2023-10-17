
namespace GraphNode {



    [System.Serializable]
    public class Action {

        public IExpression[] parameters;
        public ActionMethod method;

        public virtual Action clone() {
            return clone_to(new Action());
        }

        public bool invoke(System.Type obj_type, object obj, out object ret) {
            if (method == null) {
                ret = null;
                return true;
            }
            if (parameters == null) {
                ret = null;
                return false;
            }
            return method.invoke(obj_type, obj, parameters, out ret);
        }

        protected virtual Action clone_to(Action other) {
            other.parameters = (IExpression[])parameters.Clone();
            other.method = method;
            foreach (ref var e in Foundation.ArraySlice.create(other.parameters)) {
                e = e.clone();
            }
            return other;
        }
    }

    [System.Serializable]
    public class Action<T> {

        public IExpression[] parameters;
        public ActionMethod<T> method;

        public virtual Action<T> clone() {
            return clone_to(new Action<T>());
        }

        public bool invoke(System.Type obj_type, object obj, T param, out object ret) {
            if (method == null) {
                ret = null;
                return true;
            }
            if (parameters == null) {
                ret = null;
                return false;
            }
            return method.invoke(obj_type, obj, param, parameters, out ret);
        }

        protected virtual Action<T> clone_to(Action<T> other) {
            other.parameters = (IExpression[])parameters.Clone();
            other.method = method;
            foreach (ref var e in Foundation.ArraySlice.create(other.parameters)) {
                e = e.clone();
            }
            return other;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ActionReturnAttribute : System.Attribute {
        public readonly System.Type type;
        public ActionReturnAttribute(System.Type type) {
            this.type = type;
        }
    }
}