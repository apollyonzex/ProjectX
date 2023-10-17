
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Foundation {

    public partial class AssetBundleManager {

        private System.Func<string, string> m_bundle_path_resolver;

        public System.Func<string, string> bundle_path_resolver {
            get => m_bundle_path_resolver;
            set => m_bundle_path_resolver = value ?? default_bundle_path_resolver;
        }

        string get_bundle_path(string bundle) {
#if UNITY_EDITOR
            return Path.Combine(AssetBundleManagerConfig.instance.asset_bundle_path, bundle);
#else 
            return m_bundle_path_resolver(bundle);
#endif
        }

        static string default_bundle_path_resolver(string bundle) {
            return Path.Combine(Application.streamingAssetsPath, bundle);
        }

        class UseAB : IImpl {

            public UseAB(AssetBundleManager self) {
                this.self = self;
            }

            private AssetBundleManager self { get; }
            public Error load_bundle(string bundle, out IBundle obj) {
                var ab = AssetBundle.LoadFromFile(self.get_bundle_path(bundle));
                if (ab == null) {
                    obj = null;
                    return Error.AssetBundle_LoadFailed;
                }
                obj = new Bundle { name = bundle, ab = ab };
                return Error.NoError;
            }

            public IBundleRequest load_bundle_async(string bundle) {
                var request = new BundleRequest();
                self.StartCoroutine(load_bundle_async(bundle, request));
                return request;
            }

            public void unload_asset(Object asset) {
                Resources.UnloadAsset(asset);
            }

            struct Bundle : IBundle {

                public string name;
                public AssetBundle ab;

                public Error load_asset(string path, out Object asset) {
                    if (string.IsNullOrEmpty(path)) {
                        asset = null;
                        return Error.Asset_LoadFailed;
                    }
                    asset = ab.LoadAsset(path);
                    if (asset == null) {
                        return Error.Asset_LoadFailed;
                    }
                    if (asset is AssetRef r) {
                        asset = r.asset;
                    }
                    return Error.NoError;
                }

                public Error load_asset(string path, System.Type type, out Object asset) {
                    if (string.IsNullOrEmpty(path)) {
                        asset = null;
                        return Error.Asset_LoadFailed;
                    }
                    asset = ab.LoadAsset(path);
                    if (asset == null) {
                        return Error.Asset_LoadFailed;
                    }
                    if (asset is AssetRef r) {
                        asset = r.asset;
                    }
                    if (asset is GameObject prefab && typeof(Component).IsAssignableFrom(type)) {
                        asset = prefab.GetComponent(type);
                    } else if (!type.IsAssignableFrom(asset.GetType())) {
                        asset = null;
                        return Error.Asset_CastTypeFailed;
                    }
                    return Error.NoError;
                }

                public IAssetRequest load_asset_async(string path) {
                    var req = ab.LoadAssetAsync(path);
                    return new AssetRequest { req = req };
                }

                public IAssetRequest load_asset_async(string path, System.Type type) {
                    var req = ab.LoadAssetAsync(path);
                    return new AssetRequest { req = req, type = type };
                }

                public Error load_scene(string path, LoadSceneParameters param) {
                    var scene_path = "Assets/RawResources/" + name + "/" + path + ".unity";
                    var scene = SceneManager.LoadScene(scene_path, param);
                    return scene.path == scene_path ? Error.NoError : Error.Asset_LoadFailed;
                }

                public ISceneRequest load_scene_async(string path, LoadSceneParameters param) {
                    var req = SceneManager.LoadSceneAsync("Assets/RawResources/" + name + "/" + path + ".unity", param);
                    if (req == null) {
                        return new SceneFailed();
                    }
                    return new SceneRequest { req = req };
                }

                public void unload(bool unload_all_loaded_objects) {
                    ab.Unload(unload_all_loaded_objects);
                }

                struct SceneRequest : ISceneRequest {
                    public AsyncOperation req;

                    bool ISceneRequest.allow_scene_activation { get => req.allowSceneActivation; set => req.allowSceneActivation = value; }

                    Error IRequest.error => Error.NoError;

                    bool IRequest.is_done => req.isDone;

                    float IRequest.progress => req.progress;

                    int IRequest.priority { get => req.priority; set => req.priority = value; }

                    object IEnumerator.Current => null;

                    bool IEnumerator.MoveNext() { return !req.isDone; }

                    void IEnumerator.Reset() { }
                }

                struct AssetRequest : IAssetRequest {
                    public AssetBundleRequest req;
                    public System.Type type;
                    public Object asset;
                    public Error error;

                    private bool m_done;

                    Object IAssetRequest.asset => asset;

                    Error IRequest.error => error;

                    bool IRequest.is_done {
                        get {
                            if (!m_done) {
                                if (req.isDone) {
                                    m_done = true;
                                    asset = req.asset;
                                    if (asset == null) {
                                        error = Error.Asset_LoadFailed;
                                    } else {
                                        if (asset is AssetRef r) {
                                            asset = r.asset;
                                        }
                                        if (type != null) {
                                            if (asset is GameObject prefab && typeof(Component).IsAssignableFrom(type)) {
                                                asset = prefab.GetComponent(type);
                                            } else if (!type.IsAssignableFrom(asset.GetType())) {
                                                asset = null;
                                                error = Error.Asset_CastTypeFailed;
                                            }
                                        }
                                    }
                                }
                            }
                            return m_done;
                        }
                    }

                    float IRequest.progress => req.progress;

                    int IRequest.priority { get => req.priority; set => req.priority = value; }

                    object IEnumerator.Current => null;

                    bool IEnumerator.MoveNext() { return !req.isDone; }

                    void IEnumerator.Reset() { }
                }
            }

            class BundleRequest : IBundleRequest {
                public IBundle bundle;
                public bool is_done;
                public AssetBundleCreateRequest request;

                IBundle IBundleRequest.bundle => bundle;
                Error IRequest.error => is_done && bundle == null ? Error.AssetBundle_LoadFailed : Error.NoError;
                bool IRequest.is_done => is_done;
                float IRequest.progress => request.progress;
                int IRequest.priority { get => request.priority; set => request.priority = value; }
                object IEnumerator.Current => null;
                bool IEnumerator.MoveNext() { return !is_done; }
                void IEnumerator.Reset() { }
            }

            IEnumerator load_bundle_async(string bundle, BundleRequest request) {
                request.request = AssetBundle.LoadFromFileAsync(self.get_bundle_path(bundle));
                yield return request.request;
                var ab = request.request.assetBundle;
                if (ab != null) {
                    request.bundle = new Bundle { name = bundle, ab = ab };
                }
                request.is_done = true;
            }
        }
    }

}