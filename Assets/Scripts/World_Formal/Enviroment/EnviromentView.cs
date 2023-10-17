using Foundation;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Enviroment 
{
    public class EnviromentView : MonoBehaviour, IEnviromentView
    {
        EnviromentMgr owner;

        public Dictionary<float,EnviromentLayerView> layer_dic = new();

        public EnviromentCaravanView enviroment_caravan_view;

        public EnviromentLayerView prefab_layer;

        //==================================================================================================

        void IModelView<EnviromentMgr>.attach(EnviromentMgr owner)
        {
            this.owner = owner;
        }


        void IModelView<EnviromentMgr>.detach(EnviromentMgr owner)
        {
            this.owner = null;
        }

        void IEnviromentView.init_enviorment() {
            foreach(var layer in owner.layers) {
                var _layer  = instantiate_layer(layer.Key);
                foreach (var obj in layer.Value.objSet) {
                    _layer.Item2.AddObj(obj);
                }
            }
        }


        void IModelView<EnviromentMgr>.shift(EnviromentMgr old_owner, EnviromentMgr new_owner)
        {
        }

        void IEnviromentView.update_enviroment() {
            //enviroment_caravan_view.transform.position = new Vector3(owner.enviroment_caravan.position.x, owner.enviroment_caravan.position.y,10) ;
        }

        void IEnviromentView.add_obj(float depth,EnviromentObjData obj) {

            if (obj == null)
                return;

            if (layer_dic.ContainsKey(depth))
            {
                layer_dic[depth].AddObj(obj);
                return;
            }
            instantiate_layer(depth);
            layer_dic[depth].AddObj(obj);
        }

        void IEnviromentView.remove_obj(float depth, EnviromentObjData data) {
            if (layer_dic.ContainsKey(depth))
            {
                layer_dic[depth].RemoveObj(data);
                if (layer_dic[depth].obj_list.Count == 0)
                {
                    Destroy(layer_dic[depth].gameObject);
                    layer_dic.Remove(depth);
                }
                return;
            }
            Debug.LogError($"试图删除一个不存在的layer中的物体   ---- layer {depth}");
        }

        void IEnviromentView.reset_enviroment() {
            foreach (var layer in layer_dic)
            {
                layer.Value.SynObjPositon();
            }
            //enviroment_caravan_view.transform.position = new Vector3(owner.enviroment_caravan.position.x, owner.enviroment_caravan.position.y, 10);
        }


        private (float,EnviromentLayerView) instantiate_layer(float depth)
        {
            if (layer_dic.ContainsKey(depth))
            {
                Debug.LogError($"已经存在{depth}.layer 等仍然试图创建");
                return (-1, null);
            }
            var t_layer = Instantiate(prefab_layer, transform, false);
            t_layer.name = $"{depth} layer";
            t_layer.distance = depth;
            layer_dic.Add(depth, t_layer);
            t_layer.gameObject.SetActive(true);
            return (depth, t_layer);
        }


    }
}

