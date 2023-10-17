
using System.Collections;
using Foundation;

namespace Inits {

    public class LoadingState : Foundation.LoadingState {

        protected override IEnumerator prepare() {
            AssetBundleManager.instance.load_scene("scene", "loading", default);
            while (LoadingSceneRoot.instance == null) {
                yield return null;
            }
            LoadingSceneRoot.instance.state = this;
        }

        public void add_clean_job(System.Action complete) {
            add_job(new UnloadSceneJob(null, "loading", complete), 0);
        }

        public override void update() {
            var root = LoadingSceneRoot.instance;
            if (root != null) {
                root.synchronize_camera();
            }
        }
    }

}