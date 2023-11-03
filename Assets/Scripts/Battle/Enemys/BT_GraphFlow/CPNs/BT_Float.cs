using CalcExpr;
using System;

namespace Battle.Enemys.BT_GraphFlow.CPNs
{
    [Serializable]
    public class BT_Float : BT_CPN
    {
        [ExprConst("bl")]
        public bool bl => m_bl;

        bool m_bl = true;
    }
}

