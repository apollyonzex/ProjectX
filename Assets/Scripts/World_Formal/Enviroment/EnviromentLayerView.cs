using UnityEngine;
using System.Collections.Generic;
using Foundation;

namespace World_Formal.Enviroment
{
    public class EnviromentLayerView : MonoBehaviour
    {
        [HideInInspector]
        public float distance;

        public List<EnviromentObjView> obj_list = new();


        public void AddObj(EnviromentObjData obj)
        {
            AssetBundleManager.instance.load_asset<EnviromentObjView>(obj.bundle, obj.path, out var _gameObject);
            if (_gameObject == null)
            {
		Debug.LogError($"bundle: {obj.bundle}  path: {obj.path}");
                Debug.LogError("生成的EnviromentObjectView为空,请检查预制体是否添加EnviromentObjectView脚本或者路径是否正确配置");
                return;
            }


            var g = Instantiate(_gameObject, transform, false);
            g.init(distance, obj, g.transform.localPosition);
            g.transform.localPosition += new Vector3(obj.position.x, obj.position.y, distance);
            g.transform.localScale = new Vector3(distance / 10f, distance / 10f, 1);
            
            obj_list.Add(g);
        }

        public void RemoveObj(EnviromentObjData obj) 
        {
            for(int i = obj_list.Count - 1; i >= 0; i--)
            {
                if (obj_list[i].data == obj)
                {
                    Destroy(obj_list[i].gameObject);
                    obj_list.RemoveAt(i);
                    return;
                }
            }
        }
        public void MoveObjs()
        {
            foreach (var obj in obj_list)
            {
                obj.transform.position = obj.data.position;
            }
        }

        public void SynObjPositon()
        {
            foreach (var obj in obj_list)
            {
                var v = obj.data.position ;
                var vd = obj.default_pos;
                obj.transform.position = new Vector3(v.x + vd.x,v.y + vd.y,distance);
            }
        }
        public void ResetObjs()
        {
            foreach(var obj in obj_list)
            {
                obj.transform.position = obj.data.position;
            }
        }

        public void ChangeScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}
