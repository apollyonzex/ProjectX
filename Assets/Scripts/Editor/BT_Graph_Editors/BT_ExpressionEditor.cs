using CalcExpr;
using GraphNode;
using GraphNode.Editor;
using World_Formal.BT_GraphFlow;
using World_Formal.BT_GraphFlow.Expression_Externals;

namespace Scripts.Editor.BT_Graph_Editors
{
    [PropertyEditor(typeof(BT_Expression))]
    public class BT_ExpressionEditor : ExpressionEditor<BT_Expression>
    {
        public override ExpressionBase create_expression()
        {
            return new BT_Expression();
        }


        protected override bool get_external(string name, out ValueType ty, out IExpressionExternal external)
        {
            if (name.IndexOf('.') >= 0)
            {
                return get_cpn_external(name, out ty, out external);
            }

            return base.get_external(name, out ty, out external);
        }


        bool get_cpn_external(string name, out ValueType value_type, out IExpressionExternal external)
        {
            value_type = ValueType.Unknown;
            external = null;

            if (!try_cut_to_parts(name, out var cpn_name, out var module_name, out var property_name))
                return false;

            if (BT_CPN_Info_Dic.cpn_infos_dic.TryGetValue(cpn_name, out var cpn_info))
            {
                var cpn_type = cpn_info.cpn_type;
                var expression_externals_dic = GraphNode.Editor.Utility.get_expression_externals(cpn_type);
                if (expression_externals_dic.TryGetValue(property_name, out var expression_external))
                {
                    value_type = expression_external.ret_type;

                    var ee = (IBT_EE)System.Activator.CreateInstance(cpn_info.ee_type);
                    external = ee.init(cpn_type, module_name, expression_external);
                    return true;
                }
            }

            return false;
        }


        bool try_cut_to_parts(string str, out string n1, out string n2, out string n3)
        {
            n1 = "";
            n2 = "";
            n3 = "";

            var p1 = str.IndexOf('.');
            if (p1 < 0) return false;

            var t1 = str[(p1 + 1)..];
            var p2 = t1.IndexOf('.');
            if (p2 < 0) return false;

            n1 = str[..p1];
            n2 = t1[..p2];
            n3 = t1[(p2 + 1)..];

            return true;
        }
    }
}

