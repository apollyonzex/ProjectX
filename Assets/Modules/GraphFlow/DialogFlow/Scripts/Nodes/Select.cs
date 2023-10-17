
using UnityEngine;
using System.Collections.Generic;
using GraphNode;


namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class Select : DialogNode {

        [System.Serializable]
        public class Output : NodeDelegatePort<DialogAction> {
            public sealed override IO io => IO.Output;
            public sealed override bool can_mulit_connect => false;
            public override string name => index.ToString();

            [System.NonSerialized]
            public int index;
        }

        [ExpressionType(CalcExpr.ValueType.Integer)]
        [Display("Expression")]
        [ShowInBody]
        public Expression expression;

        [Output]
        [Display("_")]
        public DialogAction default_value { get; set; }

        public List<Output> values;

        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(IContext context) {
            if (expression == null) {
                Debug.LogError("Select: invalid expression");
            } else if (!expression.calc(context, context.context_type, out int ret)) {
                Debug.LogError("Select: expression failed!");
            } else {
                if (values != null && ret >= 0 && ret < values.Count) {
                    var val = values[ret];
                    if (val.value != null) {
                        val.value.Invoke(context);
                    } else {
                        context.end();
                    }
                    return;
                }

                if (default_value != null) {
                    default_value.Invoke(context);
                    return;
                }
            }

            context.end();
        }
    }
}