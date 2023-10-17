
using GraphNode;
using System.Collections.Generic;

namespace InvokeFlow {
    [System.Serializable]
    [Graph(typeof(InvokeGraph))]
    public class SwitchNode : ExpressionContextNodeWithInput {

        [ExpressionType(CalcExpr.ValueType.Integer)]
        [Display("Expression")]
        [ShowInBody]
        public Expression expression;

        [Output]
        [Display("_")]
        public Invoke default_output { get; set; }

        [System.Serializable]
        public class OutputPort : NodeDelegatePort<Invoke> {
            public sealed override IO io => IO.Output;
            public sealed override bool can_mulit_connect => false;
            public override string name => index.ToString();

            [System.NonSerialized]
            public int index;
        }

        public List<OutputPort> outputs;

        public override InvokeState invoke(IContext context) {
#if UNITY_EDITOR || INVOKE_FLOW_DEBUG
            using (new Recorder.NodeInvoke(context, this))
#endif
            {
                expression.calc(context, context.context_type, out int value);
                if (value >= 0 && value < outputs.Count) {
                    var output = outputs[value];
                    if (output.value != null) {
                        return output.value(context);
                    }
                    return InvokeState.None;
                }

                if (default_output != null) {
                    return default_output(context);
                }
                return InvokeState.None;
            }

        }
    }
}