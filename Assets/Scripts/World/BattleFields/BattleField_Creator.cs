using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World.BattleFields
{
    public class BattleField_Creator : Creator
    {
        BattleFieldMgr mgr;

        //==================================================================================================

        public override void @do()
        {
            mgr = new(Config.BattleFieldMgr_Name);
        }
    }
}

