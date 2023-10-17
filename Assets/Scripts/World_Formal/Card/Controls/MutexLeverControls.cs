using System;
using System.Collections.Generic;


namespace World_Formal.Card.Controls
{
    public class MutexLeverControls : LeverControls
    {
        public LeverControls others;

        public override void init()
        {
            base.init();
            lever_change_action += others.rebound;
        }
    }
}
