using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Enviroment {
    public class EnviromentObjView : MonoBehaviour {

        public float depth;
        public EnviromentObjData data;
        public Vector3 default_pos;
        public void init(float depth,EnviromentObjData data,Vector3 dp) {
            this.depth = depth;
            this.data = data;
            this.default_pos = dp;
        }
    }
}
