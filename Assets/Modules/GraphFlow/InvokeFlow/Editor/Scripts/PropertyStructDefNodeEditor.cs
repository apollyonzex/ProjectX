
using GraphNode.Editor;

namespace InvokeFlow.Editor {

    [PropertyEditor(typeof(StructDefNode))]
    public class PropertyStructDefNodeEditor : PropertyNamedNodeEditor<StructDefNodeEditor> {

        public int name_index { get; set; } = 0;

        public event System.Action<bool> on_struct_members_changed;

        public override NodeEditorList<StructDefNodeEditor> node_editor_list => (m_graph as InvokeGraphEditor).struct_defs;

        protected override void on_got_value_editor() {
            base.on_got_value_editor();
            m_value_editor.on_members_changed += rise_struct_members_changed;
        }

        protected override void on_lose_value_editor() {
            base.on_lose_value_editor();
            m_value_editor.on_members_changed -= rise_struct_members_changed;
        }

        protected override void delay_init_value_editor() {
            init_value_editor();
            if (on_struct_members_changed != null) {
                on_struct_members_changed.Invoke(false);
            }
        }

        protected override void notify_changed(bool by_user) {
            base.notify_changed(by_user);
            rise_struct_members_changed();
        }

        protected override string get_display_in_body() {
            if (m_value_editor == null) {
                return "Empty";
            }
            return $"'{m_value_editor.node.name}'";
        }

        void rise_struct_members_changed() {
            on_struct_members_changed?.Invoke(true);
        }
    }
}