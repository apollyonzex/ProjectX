
using System.Collections.Generic;

namespace BehaviourFlow {

    public interface IContext : GraphNode.IContext {
        int get_shared_int(string name);
        float get_shared_float(string name);
        void set_shared_int(string name, int value);
        void set_shared_float(string name, float value);
        IEnumerator<(string name, int value)> enumerate_shared_ints();
        IEnumerator<(string name, float value)> enumerate_shared_floats();
    }

}