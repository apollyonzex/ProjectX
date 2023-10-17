
using GraphNode;
using InvokeFlow;

namespace StateFlow {

    [System.Serializable]
    public class StateNodeBase : ExpressionContextNode {

        [NonProperty]
        public string name;

        public virtual void do_enter(IContext context) {

        }

        public virtual void do_tick(IContext context) {

        }

        public virtual void do_leave(IContext context) {
            
        }

        public virtual bool rise_event(IContext context, string name, StateEventParam[] parameters) {
            return false;
        }
    }
}