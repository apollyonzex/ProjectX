
using GraphNode.Editor;
using InvokeFlow.Editor;
using System.Collections.Generic;

namespace StateFlow.Editor {

    [GraphEditor(typeof(StateGraph))]
    public class StateGraphEditor : InvokeGraphEditor {

        public new StateGraph graph => base.graph as StateGraph;

        public override void on_open() {
            state_names_dirty = true;
            base.on_open();
            build_state_names();
        }

        public void notify_state_added(StateNodeBaseEditor state) {
            state_dict.Add(state.node.name, state);
            if (view.undo.operating) {
                if (!state_names_dirty) {
                    state_names_dirty = true;
                    view.undo.operating_delay_call += build_state_names;
                }
            } else {
                build_state_names();
            }
        }

        public void notify_state_removed(string name) {
            if (!state_dict.Remove(name)) {
                return;
            }
            if (view.undo.operating) {
                if (!state_names_dirty) {
                    state_names_dirty = true;
                    view.undo.operating_delay_call += build_state_names;
                }
            } else {
                build_state_names();
            }
        }

        public readonly SortedDictionary<string, StateNodeBaseEditor> state_dict = new SortedDictionary<string, StateNodeBaseEditor>();
        public string[] state_names { get; private set; }

        public bool state_names_dirty { get; private set; }

        public event System.Action state_names_done;

        void build_state_names() {
            state_names = new string[state_dict.Count + 1];
            state_names[0] = "<None>";
            int index = 1;
            foreach (var kvp in state_dict) {
                state_names[index] = kvp.Key;
                kvp.Value.name_index = index;
                ++index;
            }
            state_names_dirty = false;
            if (state_names_done != null) {
                state_names_done.Invoke();
                state_names_done = null;
            }
        }

        public event System.Action<StateEventNodeEditor, bool> on_state_event_changed;
        public void notify_state_event_added(StateEventNodeEditor see) {
            state_events.Add(see.node.name, see);
            on_state_event_changed?.Invoke(see, true);
        }

        public void notify_state_event_removed(StateEventNodeEditor see) {
            state_events.Remove(see.node.name);
            on_state_event_changed?.Invoke(see, false);
        }

        public event System.Action<int> on_state_event_parameters_changed;

        public void notify_state_event_parameters_changed(StateEventNodeEditor see) {
            if (on_state_event_parameters_changed != null) {
                on_state_event_parameters_changed.Invoke(state_events.IndexOfKey(see.node.name));
            }
        }

        public readonly SortedList<string, StateEventNodeEditor> state_events = new SortedList<string, StateEventNodeEditor>();
    }
}