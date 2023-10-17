using Foundation;
using UnityEngine.EventSystems;

namespace Common.Expand {
    public class SceneLayer : MonoBehaviourSingleton<SceneLayer>, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            MonoLayer.instance?.onPointerClick(eventData);
        }
    }
}


