using UnityEngine;

namespace Foundation {
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {

        #region Singleton
        public static T instance { get; private set; }
        private void Awake() {
            if (instance != null) {
                Debug.LogError("it's singleton!");
            } else {
                instance = (T)this;
                on_init();
            }
        }
        private void OnDestroy() {
            if (instance == this) {
                instance = null;
                on_fini();
            }
        }
        #endregion

        protected virtual void on_init() {

        }

        protected virtual void on_fini() {

        }
    }
}