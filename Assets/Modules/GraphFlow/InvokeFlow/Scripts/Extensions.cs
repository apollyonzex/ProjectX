
namespace InvokeFlow {

    public static class Extensions {

        public static int get_stack_int(this IContext self, int pos) {
            var stack = self.stack;
            return stack[stack.Count - pos - 1];
        }

        public static float get_stack_float(this IContext self, int pos) {
            return CalcExpr.Utility.convert_float_from((uint)self.get_stack_int(pos));
        }

        public static void set_stack_int(this IContext self, int pos, int value) {
            var stack = self.stack;
            stack[stack.Count - pos - 1] = value;
        }

        public static void push_stack(this IContext self, int[] data) {
            var stack = self.stack;
            foreach (var item in data) {
                stack.Add(item);
            }
        }

        public static void push_obj_stack(this IContext self, int count) {
            var stack = self.obj_stack;
            for (int i = 0; i < count; ++i) {
                stack.Add(null);
            }
        }

        public static void pop_obj_stack(this IContext self, int count) {
            var stack = self.obj_stack;
            var end = stack.Count - count;
            for (var last = stack.Count - 1; last >= end; --last) {
                stack.RemoveAt(last);
            }
        }

        public static T get_or_new_obj_in_stack<T>(this IContext self, int pos) where T : class, new() {
            var stack = self.obj_stack;
            var index = stack.Count - pos - 1;
            var obj = stack[index];
            if (obj == null) {
                obj = new T();
                stack[index] = obj;
            }
            return obj as T;
        }

        public static void push_stack(this IContext self) {
            self.stack.Add(0);
        }

        public static void push_stack(this IContext self, int data) {
            self.stack.Add(data);
        }

        public static void push_stack(this IContext self, float data) {
            self.stack.Add((int)CalcExpr.Utility.convert_to(data));
        }

        public static void push_stack(this IContext self, bool data) {
            self.stack.Add(data ? 1 : 0);
        }

        public static void pop_stack(this IContext self, int count) {
            var stack = self.stack;
            while (count > 0) {
                --count;
                stack.RemoveAt(stack.Count - 1);
            }
        }

        public static void pop_stack(this IContext self) {
            var stack = self.stack;
            stack.RemoveAt(stack.Count - 1);
        }

        public static Collection get_collection(this IContext self, CollectionNodeBase key) {
            if (key == null) {
                return Collection.EMPTY;
            }
            var stack = self.obj_stack;
            var index = stack.Count - key.stack_pos - 1;
            var obj = stack[index];
            if (obj == null) {
                obj = new Collection { prototype = key };
                stack[index] = obj;
            }
            return obj as Collection;
        }

        public static Element get_element(this IContext self, ElementNode key) {
            return self.get_or_new_obj_in_stack<Element>(key != null ? key.stack_pos : 0);
        }

        public static string to_string(this VariableType self) {
            switch (self) {
                case VariableType.Integer: return "int";
                case VariableType.Floating: return "float";
                case VariableType.Boolean: return "bool";
            }
            return string.Empty;
        }

    }

}