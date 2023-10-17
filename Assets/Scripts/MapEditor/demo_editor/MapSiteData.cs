using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo_editor {

    [System.Serializable]
    public class SiteData {
        public List<uint> id = new();

        public int index;

        public Vector2 position;

        public List<int> next_index = new List<int>();

        public List<uint> scene_resource_ids = new();
    }
}
