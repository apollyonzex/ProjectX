

using UnityEngine.UI;

namespace Inits {
    public class LoadingSceneRoot : Common.SceneRoot<LoadingSceneRoot> {
        public Slider progress;

        public LoadingState state { get; set; }

        private void Update() {
            synchronize_camera();
        }

        private void LateUpdate() {
            if (progress != null && state != null) {
                progress.normalizedValue = state.progress;
            }
        }
    }
}