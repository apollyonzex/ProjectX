using System;
using System.Collections.Generic;


namespace World_Formal.Card.Controls
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ControlsAction : System.Attribute
    {
        private string m_name;
        public string action_name => m_name;
        public ControlsAction(string action_name)
        {
            m_name = action_name;
        }
    }
}
