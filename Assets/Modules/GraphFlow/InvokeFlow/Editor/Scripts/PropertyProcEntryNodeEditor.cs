
using GraphNode.Editor;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(ProcEntryNode))]
    public class PropertyProcEntryNodeEditor : PropertyNamedNodeEditor<ProcEntryNodeEditor> {

        public event System.Action<bool> on_proc_entry_stack_changed;

        public override NodeEditorList<ProcEntryNodeEditor> node_editor_list => (m_graph as InvokeGraphEditor).proc_entries;

        protected override string get_display_in_body() {
            return m_value_editor != null ? $"'{m_value_editor.node.name}'" : "Empty";
        }

        protected override void on_got_value_editor() {
            base.on_got_value_editor();
            m_value_editor.on_build_stack_done += rise_proc_entry_stack_changed;
        }

        protected override void on_lose_value_editor() {
            base.on_lose_value_editor();
            m_value_editor.on_build_stack_done -= rise_proc_entry_stack_changed;
        }

        protected override void delay_init_value_editor() {
            init_value_editor();
            if (m_value_editor != null) {
                on_proc_entry_stack_changed?.Invoke(false);
            }
        }

        protected override void notify_changed(bool by_user) {
            base.notify_changed(by_user);
            on_proc_entry_stack_changed?.Invoke(by_user);
        }

        void rise_proc_entry_stack_changed() {
            if (on_proc_entry_stack_changed != null) {
                on_proc_entry_stack_changed.Invoke(!m_graph.view.undo.operating);
            }
        }
    }
}