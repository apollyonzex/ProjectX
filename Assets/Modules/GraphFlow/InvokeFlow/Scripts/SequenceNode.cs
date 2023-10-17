
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {

    [System.Serializable][Graph(typeof(InvokeGraph))]
    public class SequenceNode : InvokeNodeWithInput {

        [System.Serializable]
        public class OutputPort : NodeDelegatePort<Invoke> {

            public OutputPort(int id) {
                this.id = id;
            }

            public int id { get; set; }

            public sealed override IO io => IO.Output;
            public sealed override bool can_mulit_connect => false;
            public override string name => id.ToString();
        }

        public List<OutputPort> outputs;

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (outputs != null) {
                    foreach (var output in outputs) {
                        if (output.value != null) {
                            var ret = output.value.Invoke(context);
                            if (ret != InvokeState.None) {
                                return ret;
                            }
                        }
                    }
                }

                return InvokeState.None;
            }
        }
    }

}