
using UnityEngine;

using System.Collections.Generic;

using GraphNode;
using InvokeFlow;

namespace StateFlow {

    [System.Serializable]
    [Graph(typeof(StateGraph))]
    public class StateNode : StateNodeBase {

        [Output]
        [Display("Enter")]
        public Invoke enter { get; set; }

        [Output]
        [Display("Tick")]
        public Invoke tick { get; set; }

        [Output]
        [Display("Leave")]
        public Invoke leave { get; set; }

        [Display("Variables")]
        public Variables variables;

        public int[] stack_frame;

        [System.Serializable]
        public class EventPort : NodeDelegatePort<Invoke>, InvokeWithVariables.IPort {
            public StateEventNode node;

            public override IO io => IO.Output;

            public override string name => $"[{node.name}]";

            public override bool can_mulit_connect => false;

            IEnumerator<Variable> InvokeWithVariables.IPort.enumerate_variables() {
                if (node != null) {
                    foreach (var p in node.available_parameters) {
                        yield return p;
                    }
                }
            }
        }

        public readonly SortedList<string, EventPort> events = new SortedList<string, EventPort>();


        public override void do_enter(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (stack_frame != null) {
                    context.push_stack(stack_frame);
                }
                enter?.Invoke(context);
            }
        }

        public override void do_tick(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                tick?.Invoke(context);
            }
        }

        public override void do_leave(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                try {
                    leave?.Invoke(context);
                } finally {
                    if (stack_frame != null) {
                        context.pop_stack(stack_frame.Length);
                    }
                }
            }
        }

        public override bool rise_event(IContext context, string name, StateEventParam[] parameters) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (events.TryGetValue(name, out var port)) {
                    if (port.value != null) {
                        var argc = port.node.available_parameters.Count;
                        var stack = context.stack;
                        var stack_base = stack.Count;
                        if (argc > parameters.Length) {
                            foreach (var p in parameters) {
                                stack.Add(p.stack_value);
                            }
                            var c = argc - parameters.Length;
                            for (int i = 0; i < c; ++i) {
                                stack.Add(0);
                            }
                        } else {
                            for (int i = 0; i < argc; ++i) {
                                stack.Add(parameters[i].stack_value);
                            }
                        }
                        try {
                            port.value.Invoke(context);
                        } finally {
                            var c = Mathf.Min(argc, parameters.Length);
                            for (int i = 0; i < c; ++i) {
                                ref var p = ref parameters[i];
                                if (p.ref_val != null) {
                                    p.ref_val.stack_value = stack[stack_base + i];
                                }
                            }
                            for (int last = stack.Count - 1; last >= stack_base; --last) {
                                stack.RemoveAt(last);
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
        }
    }
}