using UnityEngine;
using System.Collections.Generic;
namespace demo_editor {
    [ExecuteInEditMode]
    public class Site  : MonoBehaviour{

        [HideInInspector]
        public int index;

        public List<uint> excel_id_list = new();

        public List<Site> next_sites = new();

        public TextMesh text;

        public List<uint> scene_resource_ids = new();

        private void OnDestroy() {
            transform.parent.GetComponent<MapEditorRoot>().remove_point(index);
        }

        private void Update() {
            foreach(var site in next_sites) {
                if (site != null) {
                    Debug.DrawLine(transform.position, site.transform.position, Color.black);
                    DrawDirection(transform.position, site.transform.position);
                }
            }
        }
        private void DrawDirection(Vector2 v1, Vector2 v2) {
            v1 = v2 - (v2 - v1).normalized / 2;
            var new_v1 = RotateRound(v1, v2, 30);
            var new_v2 = RotateRound(v1, v2, -30);
            Debug.DrawLine(new_v1, v2, new Color(160 / 255f, 32 / 255f, 244 / 255f));
            Debug.DrawLine(new_v2, v2, new Color(160 / 255f, 32 / 255f, 244 / 255f));
        }
        public Vector2 RotateRound(Vector2 position, Vector2 center, float angle) {
            Vector2 v = Quaternion.AngleAxis(angle, Vector3.forward) * (position - center);
            Vector2 resultVec2 = center + v;
            return resultVec2;
        }
    }
}
