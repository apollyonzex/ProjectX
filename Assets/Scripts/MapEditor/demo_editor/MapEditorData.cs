using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace demo_editor {

    [CreateAssetMenu(fileName ="MapEditorObject",menuName ="ScriptObejct/MapEditorDataObject", order = 1)]
    public class MapEditorData : ScriptableObject
    {
        
        public List<SiteData> sites = new List<SiteData>();

        public int start_index;

        public Sprite map_bkg;

        public List<int> exit_indexs = new();
    }

}

