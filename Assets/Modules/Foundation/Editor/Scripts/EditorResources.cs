
using UnityEngine;

namespace Foundation.Editor {

    public class EditorResources : ScriptableObject {

        public static EditorResources instance {
            get {
                if (s_instance == null) {
                    s_instance = CreateInstance<EditorResources>();
                }
                return s_instance;
            }
        }

        private static EditorResources s_instance = null;


        public Texture2D excelFileAssetIcon;
        public Texture2D binaryAssetIcon;

    }

}