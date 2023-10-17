
using UnityEngine;
using System.Collections.Generic;

namespace BackPack {
    [System.Serializable]
    public class KnapSackArea {

        public bool locked = true;

        public List<KnapSackCell> cells = new List<KnapSackCell>();
    }
}
