
using System.Collections.Generic;
using UnityEngine;
using GraphNode;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class PageOptions_Branch : PageOptionsBase {

        [Output]
        [Display("{True}")]
        public PageOptionsBase true_part { get; set; }

        [Output]
        [Display("{False}")]
        public PageOptionsBase false_part { get; set; }

        [Output]
        [Display("")]
        public PageOptionsBase tail { get; set; }

        [Display("Condition")]
        [ShowInBody]
        [ExpressionType(CalcExpr.ValueType.Boolean)]
        public Expression condition;

        public override int show_options(IContext context, ref int index) {
            int available_count = 0;
            bool ok = false;
            if (condition == null) {
                Debug.LogError("OptionState_Branch: invalid expression");
            } else if (!condition.calc(context, context.context_type, out ok)) {
                Debug.LogError("OptionState_Branch: expression failed!");
            }
            if (ok) {
                if (true_part != null) {
                    available_count += true_part.show_options(context, ref index);
                }
                false_part?.skip_options(context, ref index);
            } else {
                true_part?.skip_options(context, ref index);
                if (false_part != null) {
                    available_count += false_part.show_options(context, ref index);
                }
            }
            if (tail != null) {
                available_count += tail.show_options(context, ref index);
            }
            return available_count;
        }

        public override void skip_options(IContext context, ref int index) {
            true_part?.skip_options(context, ref index);
            false_part?.skip_options(context, ref index);
            tail?.skip_options(context, ref index);
        }

        public override bool do_option(IContext context, ref int index) {
            if (true_part != null && true_part.do_option(context, ref index)) {
                return true;
            }
            if (false_part != null && false_part.do_option(context, ref index)) {
                return true;
            }
            if (tail != null) {
                return tail.do_option(context, ref index);
            }
            return false;
        }
    }
}