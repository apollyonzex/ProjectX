﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.HandCards.Funcs
{
    public class Test_Func : IFunc
    {
        int i;

        public Test_Func(int i)
        {
            this.i = i;
        }

        bool IFunc.exec()
        {
            Debug.Log(i);
            return true;
        }
    }
}
