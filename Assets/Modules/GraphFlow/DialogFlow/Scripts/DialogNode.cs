
using GraphNode;

namespace DialogFlow {

    public interface IContext : GraphNode.IContext {
        void show_text(string content);
        void show_text(string format, object[] arguments);
        void show_option(int index, bool enabled, string content);
        void show_option(int index, bool enabled, string format, object[] arguments);
        void show_continue(float timeout);
        void jump_to(PageNodeBase page);
        void end();

        string translate_argument(string content);
        void push_externals(Nodes.JumpToDialog node);
        Nodes.JumpToDialog peek_externals();
        Nodes.JumpToDialog pop_externals();
    }

    public enum OptionState {
        Invisible = 0,
        Disable = 1,
        Enable = 2,
    }


    public delegate void DialogAction(IContext context);
    public delegate OptionState OptionStateFunc(IContext context);

    [System.Serializable]
    public class DialogNode : Node {

    }

    [System.Serializable]
    public class PageNodeBase : DialogNode {

        [Display("Name")]
        [SortedOrder(1)]
        public string name;

        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(IContext context) {
            context.jump_to(this);
        }

        public virtual void show(IContext context) { }
        public virtual void do_option(IContext context, int index) { }
        public virtual void do_continue(IContext context) { }

    }

    [System.Serializable]
    public class OptionStateNodeBase : DialogNode {
        [Output(can_multi_connect = true)]
        [Display("")]
        public virtual OptionState get_option_state(IContext context) { return OptionState.Invisible; }
    }


}