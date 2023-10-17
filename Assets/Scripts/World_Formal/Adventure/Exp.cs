using System;
using System.Collections.Generic;

namespace World_Formal.Adventure
{
    public class Exp
    {
        protected uint m_level;
        private int m_exp;


        public Exp(uint init_level, int init_exp)
        {
            m_level = init_level;
            m_exp = init_exp;
        }

        protected virtual void LevelUp()
        {
        }

        public virtual void AddExp(int delta)
        {

        }

        public uint level
        {
            get { return m_level; }
        }

        public int exp
        {
            get { return m_exp; }
            set { m_exp = value; }
        }
    }
}
