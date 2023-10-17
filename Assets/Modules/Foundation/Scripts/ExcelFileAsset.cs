using System.Collections.Generic;
using UnityEngine;

namespace Foundation {
    public class ExcelFileAsset : ScriptableObject {
        public BinaryAsset[] assets;
        [HideInInspector]
        public List<string> errMsgs;
    }
}