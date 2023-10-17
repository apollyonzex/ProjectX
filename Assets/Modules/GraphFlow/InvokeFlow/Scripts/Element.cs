

namespace InvokeFlow {
    public class Element {

        public static readonly Element DEFAULT = new Element();

        public object key;
        public StructDefNode def;

        public int[] elements;

        public void reset(object key, StructDefNode def, int[] elements) {
            this.key = key;
            this.def = def;
            this.elements = elements;
        }
    }
}