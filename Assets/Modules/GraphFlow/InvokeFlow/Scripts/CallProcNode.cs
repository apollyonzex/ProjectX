
using GraphNode;
using System;

namespace InvokeFlow {

    [Serializable]
    [Graph(typeof(InvokeGraph))]
    public class CallProcNode : ExpressionContextNodeWithInput {

        [Output][Display("")]
        public InvokeWithVariables output { get; set; }

        [ShowInBody]
        public ProcEntryNode proc_entry;

        public Expression[] parameters;


        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (proc_entry != null) {
                    proc_entry.init(context);
                    if (parameters != null) {
                        var args = new int[parameters.Length];
                        for (int i = 0; i < args.Length; ++i) {
                            if (!parameters[i].calc(context, context.context_type, out args[i])) {
                                // ERROR;
                            }
                        }
                        foreach (var arg in args) {
                            context.push_stack(arg);
                        }
                        proc_entry.call(context);
                        context.pop_stack(parameters.Length);
                    } else {
                        proc_entry.call(context);
                    }
                    InvokeState r;
                    if (output != null) {
                        r = output.action.Invoke(context);
                    } else {
                        r = InvokeState.None;
                    }
                    proc_entry.fini(context);
                    return r;
                }

                if (output != null) {
                    return output.action.Invoke(context);
                }
                return InvokeState.None;
            }
        }

    }

}