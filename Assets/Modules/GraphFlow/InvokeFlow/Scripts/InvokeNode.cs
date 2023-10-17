
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {

    public enum InvokeState {
        None,
        Break,
        Continue,
        Return,
    }

    public interface IContext : GraphNode.IContext {
        IList<int> stack { get; }
        IList<object> obj_stack { get; }
        string debug_name { get; }
    }

    [System.Serializable]
    public class InvokeNode : Node {
        public delegate InvokeState Invoke(IContext context);

        public virtual InvokeState invoke(IContext context) { return InvokeState.None; }
    }

}