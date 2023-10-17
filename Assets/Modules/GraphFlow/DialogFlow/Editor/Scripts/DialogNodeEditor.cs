
using GraphNode.Editor;

namespace DialogFlow.Editor {

    [NodeEditor(typeof(DialogNode))]
    public class DialogNodeEditor : GenericNodeEditor {

        public virtual void rebuild_all_expressions() {
            foreach (var pe in m_properties) {
                if (pe is ExpressionEditor ee) {
                    ee.build();
                } else if (pe is ContextActionEditor cae) {
                    cae.build_parameters();
                } else if (pe is DialogTextEditor dte) {
                    dte.build_parameters();
                }
            }
        }

    }
}