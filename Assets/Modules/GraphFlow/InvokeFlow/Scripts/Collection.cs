
using System.Collections.Generic;

namespace InvokeFlow {
    public class Collection {
        public static readonly Collection EMPTY = new Collection { read_count = 1 };

        public readonly Dictionary<object, int[]> items = new Dictionary<object, int[]>();

        public int read_count;
        public CollectionNodeBase prototype;

        public bool insert(object key) {
            if (read_count != 0 || key == null || items.ContainsKey(key)) {
                return false;
            }
            int[] value = null;
            var element_type = prototype.get_element_type();
            if (element_type != null && element_type.stack_frame != null) {
                value = (int[])element_type.stack_frame.Clone();
            }
            items.Add(key, value);
            return true;
        }

        public bool remove(object key) {
            if (read_count != 0 || key == null) {
                return false;
            }
            return items.Remove(key);
        }
    }
}