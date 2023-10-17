
using GraphNode;
using UnityEngine;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class LogNode : ExpressionContextNodeWithInput {

        [Output][Display("")]
        public Invoke output { get; set; }


        [System.Serializable]
        public struct Parameter {
            public VariableName name;
            public VariableType type;
        }

        public Parameter[] parameters;


        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (!string.IsNullOrEmpty(comment.content)) {
                    if (parameters == null || parameters.Length == 0) {
                        log_output(comment.content, new object[0]);
                    } else {
                        var ps = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; ++i) {
                            ref var p = ref parameters[i];
                            if (p.name.stack_pos != -1) {
                                switch (p.type) {
                                    case VariableType.Integer:
                                        ps[i] = context.get_stack_int(p.name.stack_pos);
                                        break;

                                    case VariableType.Floating:
                                        ps[i] = context.get_stack_float(p.name.stack_pos);
                                        break;

                                    case VariableType.Boolean:
                                        ps[i] = context.get_stack_int(p.name.stack_pos) != 0;
                                        break;
                                }
                            }
                        }
                        log_output(comment.content, ps);
                    }
                }

                if (output != null) {
                    return output.Invoke(context);
                }
                return InvokeState.None;
            }
        }

        protected virtual void log_output(string format, object[] args) {
            Debug.LogFormat(format, args);
        }
    }

}