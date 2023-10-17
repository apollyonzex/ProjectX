using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Foundation {

    public partial class AssetBundleManager : MonoBehaviourSingleton<AssetBundleManager> {

        public interface IRequest : IEnumerator {
            Error error { get; }
            bool is_done { get; }
            float progress { get; }
            int priority { get; set; }
        }

        public interface IAssetRequest : IRequest {
            Object asset { get; }
        }

        public interface ISceneRequest : IRequest {
            bool allow_scene_activation { get; set; }
        }

        public interface IBundleRequest : IRequest {
            IBundle bundle { get; }
        }

        public interface IBundle {
            Error load_asset(string path, out Object asset);
            Error load_asset(string path, System.Type type, out Object asset);
            IAssetRequest load_asset_async(string path);
            IAssetRequest load_asset_async(string path, System.Type type);
            Error load_scene(string path, LoadSceneParameters param);
            ISceneRequest load_scene_async(string path, LoadSceneParameters param);
            void unload(bool unload_all_loaded_objects);
        }

        public Error load_asset(string bundle, string path, out Object asset) {
            var err = load_bundle(bundle, out IBundle obj);
            if (!err.is_ok()) {
                asset = null;
                return err;
            }
            return obj.load_asset(path, out asset);
        }

        public Error load_asset(string bundle, string path, System.Type type, out Object asset) {
            var err = load_bundle(bundle, out IBundle obj);
            if (!err.is_ok()) {
                asset = null;
                return err;
            }
            return obj.load_asset(path, type, out asset);
        }

        public Error load_asset<T>(string bundle, string path, out T asset) where T: Object {
            var err = load_bundle(bundle, out IBundle obj);
            if (!err.is_ok()) {
                asset = null;
                return err;
            }
            err = obj.load_asset(path, typeof(T), out Object asset_obj);
            if (!err.is_ok()) {
                asset = null;
                return err;
            }
            asset = asset_obj as T;
            return Error.NoError;
        }

        public IAssetRequest load_asset_async(string bundle, string path) {
            var request = new LoadBundleAssetAsync();
            StartCoroutine(load_asset_async(bundle, path, request));
            return request;
        }

        public IAssetRequest load_asset_async(string bundle, string path, System.Type type) {
            var request = new LoadBundleAssetAsync();
            StartCoroutine(load_asset_async(bundle, path, type, request));
            return request;
        }

        public IAssetRequest load_asset_async<T>(string bundle, string path) where T: Object {
            return load_asset_async(bundle, path, typeof(T));
        }

        public void unload_asset(Object asset) {
            impl.unload_asset(asset);
        }

        public Error load_scene(string bundle, string path, LoadSceneParameters param) {
            var err = load_bundle(bundle, out IBundle obj);
            if (!err.is_ok()) {
                return err;
            }
            return obj.load_scene(path, param);
        }

        public ISceneRequest load_scene_async(string bundle, string path, LoadSceneParameters param) {
            var request = new LoadBundleSceneAsync();
            StartCoroutine(load_scene_async(bundle, path, param, request));
            return request;
        }

        public Error load_bundle(string bundle, out IBundle obj) {
            if (string.IsNullOrEmpty(bundle)) {
                obj = null;
                return Error.Asset_LoadFailed;
            }
            if (m_bundles.TryGetValue(bundle, out Bundle b)) {
                if (b.bundle != null) {
                    obj = b;
                    return Error.NoError;
                }
                if (b.loading_request != null) {
                    obj = null;
                    return Error.AssetBundle_AsyncLoadNotComplete;
                }
                m_bundles.Remove(bundle);
            }
            var err = impl.load_bundle(bundle, out obj);
            if (err.is_ok()) {
                b = new Bundle {
                    name = bundle,
                    owner = this,
                    loading_request = null,
                    bundle = obj
                };
                m_bundles.Add(bundle, b);
                obj = b;
            }
            return err;
        }

        public IBundleRequest load_bundle_async(string bundle) {
            if (m_bundles.TryGetValue(bundle, out Bundle b)) {
                if (b.bundle != null) {
                    return new BundleLoaded(b);
                }
                if (b.loading_request != null) {
                    return b.loading_request;
                }
                m_bundles.Remove(bundle);
            }
            var request = impl.load_bundle_async(bundle);
            b = new Bundle {
                name = bundle,
                owner = this,
            };
            b.loading_request = new BundleLoading {
                bundle = b,
                request = request,
            };
            m_bundles.Add(bundle, b);
            StartCoroutine(waiting_bundle(b, request));
            return b.loading_request;
        }

        public void unload_bundle(string bundle, bool unload_all_loaded_objects) {
            if (m_bundles.TryGetValue(bundle, out Bundle b)) {
                b.unload(unload_all_loaded_objects);
            }
        }

        public bool try_get_loaded_bundle(string bundle, out IBundle obj) {
            if (m_bundles.TryGetValue(bundle, out Bundle b)) {
                if (b.bundle != null) {
                    obj = b;
                    return true;
                }
            }
            obj = null;
            return false;
        }

        public AssetBundleManager() {
            m_bundle_path_resolver = default_bundle_path_resolver;
#if (UNITY_EDITOR || FORCE_USE_RES) && !FORCE_USE_AB
            impl = new UseRes(this);
#else
            impl = new UseAB(this);
#endif
        }

        IEnumerator waiting_bundle(Bundle bundle, IBundleRequest request) {
            yield return request;
            if (request.bundle != null) {
                bundle.bundle = request.bundle;
            } else {
                m_bundles.Remove(bundle.name);
            }
            bundle.loading_request = null;
        }

        IEnumerator load_asset_async(string bundle, string path, LoadBundleAssetAsync request) {
            var bundle_req = load_bundle_async(bundle);
            request.request = bundle_req;
            while (!bundle_req.is_done) {
                request.progress = bundle_req.progress * 0.1f;
                yield return null;
            }
            if (bundle_req.bundle == null) {
                request.progress = 1;
                request.is_done = true;
                yield break;
            }
            var asset_req = bundle_req.bundle.load_asset_async(path);
            asset_req.priority = bundle_req.priority;
            request.request = asset_req;
            while (!asset_req.is_done) {
                request.progress = asset_req.progress * 0.9f + 0.1f;
                yield return null;
            }
            request.progress = 1;
            request.is_done = true;
            request.asset = asset_req.asset;
        }

        IEnumerator load_asset_async(string bundle, string path, System.Type type, LoadBundleAssetAsync request) {
            var bundle_req = load_bundle_async(bundle);
            request.request = bundle_req;
            while (!bundle_req.is_done) {
                request.progress = bundle_req.progress * 0.1f;
                yield return null;
            }
            if (bundle_req.bundle == null) {
                request.progress = 1;
                request.is_done = true;
                yield break;
            }
            var asset_req = bundle_req.bundle.load_asset_async(path, type);
            asset_req.priority = bundle_req.priority;
            request.request = asset_req;
            while (!asset_req.is_done) {
                request.progress = asset_req.progress * 0.9f + 0.1f;
                yield return null;
            }
            request.progress = 1;
            request.is_done = true;
            request.asset = asset_req.asset;
        }

        IEnumerator load_scene_async(string bundle, string path, LoadSceneParameters param, LoadBundleSceneAsync request) {
            var bundle_req = load_bundle_async(bundle);
            request.request = bundle_req;
            yield return bundle_req;
            if (bundle_req.bundle == null) {
                request.is_done = true;
                yield break;
            }
            var scene_req = bundle_req.bundle.load_scene_async(path, param);
            request.request = request.scene_request = scene_req;
            scene_req.priority = bundle_req.priority;
            scene_req.allow_scene_activation = request.allow_scene_activation;
            yield return scene_req;
            request.is_done = true;
        }

        private IImpl impl { get; }

        interface IImpl {

            void unload_asset(Object asset);
            Error load_bundle(string bundle, out IBundle obj);
            IBundleRequest load_bundle_async(string bundle);
        }

        class Bundle : IBundle {
            public string name;
            public AssetBundleManager owner;
            public IBundleRequest loading_request;
            public IBundle bundle;

            public Error load_asset(string path, out Object asset) {
                if (bundle == null) {
                    asset = null;
                    return Error.AssetBundle_NotLoad;
                }
                if (m_loading.TryGetValue(path, out IAssetRequest req)) {
                    if (req.is_done) {
                        asset = req.asset;
                        if (asset == null) {
                            return Error.AssetBundle_LoadFailed;
                        }
                        return Error.NoError;
                    }
                    asset = null;
                    return Error.AssetBundle_AsyncLoadNotComplete;
                }
                return bundle.load_asset(path, out asset);
            }

            public Error load_asset(string path, System.Type type, out Object asset) {
                if (bundle == null) {
                    asset = null;
                    return Error.AssetBundle_NotLoad;
                }
                if (m_loading.TryGetValue(path, out IAssetRequest req)) {
                    if (req.is_done) {
                        asset = req.asset;
                        if (asset == null) {
                            return Error.AssetBundle_LoadFailed;
                        }
                        return Error.NoError;
                    }
                    asset = null;
                    return Error.AssetBundle_AsyncLoadNotComplete;
                }
                return bundle.load_asset(path, type, out asset);
            }

            public IAssetRequest load_asset_async(string path) {
                if (bundle == null) {
                    return new LoadError(Error.AssetBundle_NotLoad);
                }

                if (m_loading.TryGetValue(path, out IAssetRequest req)) {
                    return new WaitingAsset(req);
                }
                req = bundle.load_asset_async(path);
                if (req.is_done) {
                    return req;
                }
                owner.StartCoroutine(waiting_asset_loaded(req, path));
                return req;
            }

            public IAssetRequest load_asset_async(string path, System.Type type) {
                if (bundle == null) {
                    return new LoadError(Error.AssetBundle_NotLoad);
                }

                if (m_loading.TryGetValue(path, out IAssetRequest req)) {
                    return new WaitingAsset(req);
                }
                req = bundle.load_asset_async(path, type);
                if (req.is_done) {
                    return req;
                }
                owner.StartCoroutine(waiting_asset_loaded(req, path));
                return req;
            }

            public Error load_scene(string path, LoadSceneParameters param) {
                if (bundle == null) {
                    return Error.AssetBundle_NotLoad;
                }
                return bundle.load_scene(path, param);
            }

            public ISceneRequest load_scene_async(string path, LoadSceneParameters param) {
                if (bundle == null) {
                    return new LoadError(Error.AssetBundle_NotLoad);
                }
                return bundle.load_scene_async(path, param);
            }

            public void unload(bool unload_all_loaded_objects) {
                if (bundle != null) {
                    bundle.unload(unload_all_loaded_objects);
                    bundle = null;
                    owner.m_bundles.Remove(name);
                }
            }

            IEnumerator waiting_asset_loaded(IAssetRequest request, string path) {
                m_loading.Add(path, request);
                yield return request;
                m_loading.Remove(path);
            }

            Dictionary<string, IAssetRequest> m_loading = new Dictionary<string, IAssetRequest>();
        }

        struct WaitingAsset : IAssetRequest {
            public IAssetRequest request;
            public WaitingAsset(IAssetRequest request) {
                this.request = request;
            }

            public Object asset => request.asset;

            public Error error => request.error;

            public float progress => request.progress;

            public int priority { get => request.priority; set => request.priority = value; }

            public bool is_done => request.is_done;

            object IEnumerator.Current => null;

            bool IEnumerator.MoveNext() {
                return !request.is_done;
            }
            void IEnumerator.Reset() { }
        }

        struct LoadError : IAssetRequest, ISceneRequest {
            public LoadError(Error error) {
                this.error = error;
                priority = 0;
                allow_scene_activation = true;
            }

            public Error error;
            public int priority { get; set; }

            public Object asset => null;

            public bool is_done => true;

            public float progress => 1;

            object IEnumerator.Current => null;

            Error IRequest.error => error;
            public bool allow_scene_activation { get; set; }
            bool IEnumerator.MoveNext() { return false; }
            void IEnumerator.Reset() { }
        }

        struct BundleLoading : IBundleRequest {
            public Bundle bundle;
            public IBundleRequest request;

            public Error error => request.error;

            public bool is_done => bundle.loading_request == null;

            public float progress => request.progress;

            public int priority { get => request.priority; set => request.priority = value; }

            object IEnumerator.Current => null;

            IBundle IBundleRequest.bundle => bundle.bundle != null ? bundle : null;

            bool IEnumerator.MoveNext() { return !is_done; }
            void IEnumerator.Reset() { }
        }

        struct BundleLoaded : IBundleRequest {
            public BundleLoaded(IBundle bundle) {
                this.bundle = bundle;
                priority = 0;
            }
            public IBundle bundle { get; }
            public bool is_done => true;
            public Error error => Error.NoError;
            public float progress => 1;
            public int priority { get; set; }

            object IEnumerator.Current => null;
            bool IEnumerator.MoveNext() { return false; }
            void IEnumerator.Reset() { }
        }

        class LoadBundleAssetAsync : IAssetRequest {
            public Object asset;
            public bool is_done;
            public float progress;
            public IRequest request;
            
            Object IAssetRequest.asset => asset;

            Error IRequest.error => request.error;

            bool IRequest.is_done => is_done;

            float IRequest.progress => progress;

            int IRequest.priority { get => request.priority; set => request.priority = value; }
            object IEnumerator.Current => null;
            bool IEnumerator.MoveNext() { return !is_done; }
            void IEnumerator.Reset() { }
        }

        class LoadBundleSceneAsync : ISceneRequest {
            public bool is_done;
            public ISceneRequest scene_request;
            public IRequest request;

            public bool allow_scene_activation = true;

            bool ISceneRequest.allow_scene_activation {
                get => allow_scene_activation;
                set {
                    allow_scene_activation = value;
                    if (scene_request != null) {
                        scene_request.allow_scene_activation = value;
                    }
                }
            }

            Error IRequest.error => request.error;
            bool IRequest.is_done => is_done;
            float IRequest.progress => scene_request != null ? scene_request.progress : 0;
            int IRequest.priority { get => request.priority; set => request.priority = value; }
            object IEnumerator.Current => null;
            bool IEnumerator.MoveNext() { return !is_done; }
            void IEnumerator.Reset() { }
        }

        struct SceneFailed : ISceneRequest {
            bool ISceneRequest.allow_scene_activation { get => true; set { } }

            Error IRequest.error => Error.Asset_LoadFailed;

            bool IRequest.is_done => true;

            float IRequest.progress => 0;

            int IRequest.priority { get => 0; set { } }

            object IEnumerator.Current => null;

            bool IEnumerator.MoveNext() {
                return false;
            }

            void IEnumerator.Reset() { }
        }


        Dictionary<string, Bundle> m_bundles = new Dictionary<string, Bundle>();
    }

}