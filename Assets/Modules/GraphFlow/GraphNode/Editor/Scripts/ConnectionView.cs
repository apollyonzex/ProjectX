
using UnityEngine;

namespace GraphNode.Editor {

    public class ConnectionView {

        public InputPortView input { get; private set; }
        public OutputPortView output { get; private set; }

        public Vector2 input_position => input.node.pos_in_clipped_gui + input.position.center;
        public Vector2 output_position => output.node.pos_in_clipped_gui + output.position.center;

        public void on_repaint(GraphView view) {
            if (input.node.size_changed || output.node.size_changed) {
                view.window.Repaint();
                return;
            }
            view.draw_connection(output_position, input_position, output.color);
        }

        public ConnectionView(InputPortView input, OutputPortView output) {
            this.input = input;
            this.output = output;
        }

        public void connect(bool rise_event) {
            input.add_connection(this);
            output.add_connection(this);
            if (rise_event) {
                input.port.connect_unchecked(input.node.editor.node, output.node.editor.node, output.port);
                input.node.editor.on_input_connected(this);
                output.node.editor.on_output_connected(this);
            }
        }

        public void disconnect() {
            input.remove_connection(this);
            output.remove_connection(this);
            input.port.disconnect_unchecked(input.node.editor.node, output.node.editor.node, output.port);
            input.node.editor.on_input_disconnected(this);
            output.node.editor.on_output_disconnected(this);
        }

    }

}