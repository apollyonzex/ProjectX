using UnityEngine;
using System.Collections.Generic;
namespace CaravanEnhanced {

    [CreateAssetMenu(fileName = "MapEditorObject", menuName = "ScriptObejct/MapEditorDataObject")]
    public class CaravanData :ScriptableObject {

        public string body_path;

        public string body_prefab_path;

        public bool isboxcolider;

        public Vector2 size;

        public float wheel_height;
        public float wheel_to_center_dis;

        public Vector2 body_spine_offset;

        public List<CaravanMakerSlotData> slots = new List<CaravanMakerSlotData>();
    }
}
