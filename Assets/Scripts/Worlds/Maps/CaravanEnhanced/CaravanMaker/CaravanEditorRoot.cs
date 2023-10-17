using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CaravanEnhanced {

    [ExecuteInEditMode]
    public class CaravanEditorRoot : MonoBehaviour {

        public string body_path;

        public CaravanBody body;

        public CaravanData caravan_data;

        public Transform body_spine_offset;


        [Header("模板")]
        public CaravanBody prefab;
        public void save() {
            if (caravan_data != null) {
            } else {
                var t = ScriptableObject.CreateInstance<CaravanData>();
                AssetDatabase.CreateAsset(t, @"Assets\Scripts\Worlds\Maps\CaravanEnhanced\CaravanMaker\data\" + "new caravandata" + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                caravan_data = t;
            }
            caravan_data.slots.Clear();
            if(body_path == null) {
                Debug.LogWarning("车体sprite的路径为空,请确认这是否位你想要的结果");
            }

            caravan_data.body_path = body_path;
            caravan_data.body_spine_offset = body_spine_offset.transform.localPosition;

            foreach (var slot in body.slots) {
                var d = new CaravanMakerSlotData {
                    type = slot.slotType,
                    position = slot.transform.position,
                    rotation = slot.transform.rotation,
                    item_id = slot.id,
                    bone_name = slot.boneFollower.boneName,
                    bone_pos = slot.boneFollower.transform.position
                };
                slot.TryGetComponent<SpriteRenderer>(out var component);
                if(component!= null) {
                    d.horizontal_flip = component.flipX;
                    d.vertical_flip = component.flipY;
                }
                caravan_data.slots.Add(d);

                if (slot.slotType == AutoCode.Tables.Device.e_slotType.i_车轮)
                {
                    DB.instance.items.try_get(slot.id, out var r);
                    caravan_data.wheel_height = r.f_wheel_height ?? 0;
                    caravan_data.wheel_to_center_dis = Mathf.Abs(slot.transform.position.y);
                }

            }
            var colider = body.GetComponent<BoxCollider2D>();
            if (colider!=null) {
                caravan_data.isboxcolider = true;
                caravan_data.size = colider.size;
            } else {
                Debug.Log("缺少boxclolider2d");
            }
            EditorUtility.SetDirty(caravan_data);
            AssetDatabase.SaveAssetIfDirty(caravan_data);
            Debug.Log("保存结束");
        }

        public void new_body() {
            body = Instantiate(prefab, transform, false);
            body.gameObject.SetActive(true);
        }
    }

}
