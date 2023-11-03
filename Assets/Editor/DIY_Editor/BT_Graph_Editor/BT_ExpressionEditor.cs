using Battle.Enemys.BT_GraphFlow;
using CalcExpr;
using GraphNode;
using GraphNode.Editor;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace Editor.BT_Graph_Editor
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

            if (!BT_CPN_Info.infos_dic.TryGetValue(cpn_name, out var cpn_type))
                return false;

            var expression_externals_dic = GraphNode.Editor.Utility.get_expression_externals(cpn_type);
            if (!expression_externals_dic.TryGetValue(property_name, out var expression_external))
                return false;

            value_type = expression_external.ret_type;
            external = new BT_EE().init(cpn_type, module_name, expression_external);
            return true;
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

