using Foundation;
using UnityEngine.EventSystems;
using UnityEngine;
using Worlds;

namespace Common.Expand {
    public class MonoLayer : MonoBehaviourSingleton<MonoLayer>
    {
        public event ClickHandler on_pointer_click;
        public delegate void ClickHandler();
        [HideInInspector]
        public Transform m_hit;

        public void onPointerClick(PointerEventData eventData)
        {
            var uiCamera = WorldSceneRoot.instance.uiCamera;
            var mainCamera = WorldSceneRoot.instance.mainCamera;
            var pos = uiCamera.ScreenToWorldPoint(eventData.position);

            pos.x += mainCamera.transform.position.x;
            pos.y += mainCamera.transform.position.y;
            pos.z = 0;
            m_hit = Physics2D.Raycast(pos, Vector2.zero).transform;
            if (m_hit == null) return;
            on_pointer_click?.Invoke();
            m_hit = null;
        }


        public bool try_get_interact_component<T>(out T t)
        {      
            t = m_hit.transform.GetComponent<T>();
            return t != null;
        }

    }

}


