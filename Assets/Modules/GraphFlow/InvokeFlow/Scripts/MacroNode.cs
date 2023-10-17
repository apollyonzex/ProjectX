
using GraphNode;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace InvokeFlow {

    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class MacroNode : ExpressionContextNodeWithInput {

        public override void on_serializing(List<Object> referenced_objects) {
            macro_graph_asset.before_serialize(referenced_objects);
        }

        public override void on_deserialized(Object[] referenced_objects) {
            macro_graph_asset.after_deserialize(referenced_objects);
        }

        public UnityObjectRef<InvokeMacroGraphAsset> macro_graph_asset;

        public Expression[] parameters;

        public int output_count { get; set; }

        public ElementNode[] elements;
        public CollectionNodeBase[] collections;


        [Output][Display("")]
        public InvokeWithVariables output { get; set; }

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                if (macro_graph_asset.obj != null) {
                    var pc = parameters.Length;
                    var macro = macro_graph_asset.obj.graph;
                    if (macro.argument_count == pc && macro.return_count == output_count) {
                        var argv = new int[parameters.Length];
                        for (int i = 0; i < pc; ++i) {
                            if (!parameters[i].calc(context, context.context_type, out argv[i])) {
                                Debug.LogError($"Macro: param #{i} failed!");
                            }
                        }
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
                        using (new Recorder.GraphInvoke(context, macro))
#endif
                        {

                            object[] es = null;
                            if (elements != null && macro.external_elements != null) {
                                var c = Mathf.Min(elements.Length, macro.external_elements.Count);
                                if (c != 0) {
                                    es = new object[c];
                                    for (int i = 0; i < c; ++i) {
                                        es[i] = context.get_element(elements[i]);
                                    }
                                }
                            }
                            object[] cs = null;
                            if (collections != null && macro.external_collections != null) {
                                var c = Mathf.Min(collections.Length, macro.external_collections.Count);
                                if (c != 0) {
                                    cs = new object[c];
                                    for (int i = 0; i < c; ++i) {
                                        cs[i] = context.get_collection(collections[i]);
                                    }
                                }
                            }

                            macro.init_stack(context);
                            var stack = context.obj_stack;
                            var top = stack.Count - 1;
                            if (es != null) {
                                for (int i = 0; i < es.Length; ++i) {
                                    stack[top - macro.external_elements[i].stack_pos] = es[i];
                                }
                            }
                            if (cs != null) {
                                for (int i = 0; i < cs.Length; ++i) {
                                    stack[top - macro.external_collections[i].stack_pos] = cs[i];
                                }
                            }
                            context.push_stack(argv);
                            macro.entry.invoke(context);
                            macro.fini_stack(context);
                        }
                        var r = InvokeState.None;
                        if (output != null) { r = output.action.Invoke(context); }
                        context.pop_stack(output_count);
                        return r;
                    }

                    Debug.LogError("Macro: invalid arguments or returns");
                }
                return InvokeState.None;
            }
        }
    }

}