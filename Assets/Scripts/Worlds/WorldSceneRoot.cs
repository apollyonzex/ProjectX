using Common;
using Common.Expand;
using UnityEngine;
using Worlds.Missions.Battles;

namespace Worlds
{
    public class WorldSceneRoot : SceneRoot<WorldSceneRoot>
    {
        public SceneLayer SceneLayer;
        public MonoLayer MonoLayer;

        public GameObject mission_layer => m_mission_layer;
        GameObject m_mission_layer;

        public Transform focus;

        public float Vcamera;

        //==================================================================================================


        void Start()
        {
            WorldState.instance.enter_new_mission();
        }


        private void LateUpdate()
        {
            
        }


        public T Open_UI_Prefab<T>(string bundle, string path, Transform parent = null) where T : Component
        {
            parent ??= uiRoot.transform;
            return Common.Utility.load_and_instantiate_component_from_prefab<T>(bundle, path, parent, false);
        }

        
        public void load_mission_layer(string layer_name, out GameObject layer)
        {
            var asset = Open_UI_Prefab<RectTransform>("ui", layer_name, uiRoot.transform);
            layer = asset.gameObject;

            set_base_layers(false);

            m_mission_layer = layer;
        }


        public void unload_mission_layer()
        {
            Destroy(m_mission_layer);
            set_base_layers(true);
        }


        /// <summary>
        /// 是否激活: 非mission的所有layers
        /// </summary>
        /// <param name="bl"></param>
        void set_base_layers(bool bl)
        {
            SceneLayer.gameObject.SetActive(bl);
            MonoLayer.gameObject.SetActive(bl);
        }
    }
}


