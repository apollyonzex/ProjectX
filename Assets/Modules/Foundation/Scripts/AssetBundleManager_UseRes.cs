
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Foundation {
    public partial class AssetBundleManager {
        class UseRes : IImpl {
            public UseRes(AssetBundleManager self) {
                this.self = self;
            }

            private AssetBundleManager self { get; }

            public Error load_bundle(string name, out IBundle obj) {
#if UNITY_EDITOR
                var bundle = new Bundle(name, self);
                if (!System.IO.Directory.Exists("Assets/Resources/" + bundle.bundle_path)) {
                    obj = null;
                    return Error.AssetBundle_LoadFailed;
                }
                obj = bundle;
#else
                obj = new Bundle(name, self);
#endif
                return Error.NoError;
            }

            public IBundleRequest load_bundle_async(string bundle) {
                var err = load_bundle(bundle, out IBundle obj);
                return new ResBundleLoaded {
                    bundle = obj,
                    error = err,
                };
            }

            public void unload_asset(Object asset) {
#if !UNITY_EDITOR
                Resources.UnloadAsset(asset);            
#endif
            }

            struct Bundle : IBundle {
                public string name;
                public string bundle_path;
                public AssetBundleManager owner;

                public Bundle(string name, AssetBundleManager owner) {
                    this.name = name;
                    bundle_path = "RawResources/" + name + "/";
                    this.owner = owner;
                }

                public Error load_asset(string path, out Object asset) {
                    asset = Resources.Load(bundle_path + path);
                    if (asset == null) {
                        return Error.Asset_LoadFailed;
                    }
                    if (asset is AssetRef r) {
                        asset = r.asset;
                        if (asset == null) {
                            return Error.Asset_LoadFailed;
                        }
                    }
                    return Error.NoError;
                }

                public Error load_asset(string path, System.Type type, out Object asset) {
                    asset = Resources.Load(bundle_path + path, type);
                    if (asset != null) {
                        return Error.NoError;
                    }
                    var asset_ref = Resources.Load<AssetRef>(bundle_path + path);
                    if (asset_ref == null || asset_ref.asset == null) {
                        return Error.Asset_LoadFailed;
                    }
                    if (!type.IsAssignableFrom(asset_ref.asset.GetType())) {
                        return Error.Asset_CastTypeFailed;
                    }
                    asset = asset_ref.asset;
                    return Error.NoError;
                }

                public IAssetRequest load_asset_async(string path) {
                    var req = new AssetRequest();
                    owner.StartCoroutine(load_asset_async(path, req));
                    return req;
                }

                public IAssetRequest load_asset_async(string path, System.Type type) {
                    var req = new AssetRequest();
                    owner.StartCoroutine(load_asset_async(path, type, req));
                    return req;
                }

                public Error load_scene(string path, LoadSceneParameters param) {

                    var scene_path = "Assets/Resources/" + bundle_path + path + ".unity";
#if UNITY_EDITOR && !FORCE_USE_RES
                    var scene = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scene_path, param);
#else
                    var scene = SceneManager.LoadScene(scene_path, param);
#endif
                    return scene.path == scene_path ? Error.NoError : Error.Asset_LoadFailed;
                }

                public ISceneRequest load_scene_async(string path, LoadSceneParameters param) {
                    var scene_path = "Assets/Resources/" + bundle_path + path + ".unity";
#if UNITY_EDITOR && !FORCE_USE_RES
                    var req = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(scene_path, param);
#else
                    var req = SceneManager.LoadSceneAsync(scene_path, param);
#endif
                    if (req == null) {
                        return new SceneFailed();
                    }
                    return new SceneRequest { req = req };
                }

                public void unload(bool unload_all_loaded_objects) {
                    
                }

                class AssetRequest : IAssetRequest {
                    public ResourceRequest req;
                    public bool is_done;
                    public Object asset;
                    Object IAssetRequest.asset => asset;

                    Error IRequest.error {
                        get {
                            if (req.isDone && req.asset == null) {
                                return Error.Asset_LoadFailed;
                            }
                            return Error.NoError;
                        }
                    }

                    bool IRequest.is_done => is_done;

                    float IRequest.progress => req.progress;

                    int IRequest.priority { get => req.priority; set => req.priority = value; }

                    object IEnumerator.Current => null;

                    bool IEnumerator.MoveNext() { return !is_done; }

                    void IEnumerator.Reset() { }
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

                IEnumerator load_asset_async(string path, AssetRequest request) {
                    var req = Resources.LoadAsync(bundle_path + path);
                    request.req = req;
                    yield return req;
                    if (req.asset != null) {
                        if (req.asset is AssetRef r) {
                            request.asset = r.asset;
                        } else {
                            request.asset = req.asset;
                        }
                    }
                    request.is_done = true;
                }

                IEnumerator load_asset_async(string path, System.Type type, AssetRequest request) {
                    path = bundle_path + path;
                    var req = Resources.LoadAsync(path, type);
                    request.req = req;
                    yield return req;
                    if (req.asset != null) {
                        request.asset = req.asset;
                        request.is_done = true;
                        yield break;
                    }
                    req = Resources.LoadAsync<AssetRef>(path);
                    req.priority = request.req.priority;
                    request.req = req;
                    yield return req;
                    request.is_done = true;
                    if (req.asset is AssetRef asset_ref && asset_ref.asset != null && type.IsAssignableFrom(asset_ref.asset.GetType())) {
                        request.asset = asset_ref.asset;
                    }
                }
            }
            struct ResBundleLoaded : IBundleRequest {
                public IBundle bundle;
                public Error error;
                public int priority;
                IBundle IBundleRequest.bundle => bundle;
                Error IRequest.error => error;
                bool IRequest.is_done => true;
                float IRequest.progress => 1;
                int IRequest.priority { get => priority; set => priority = value; }
                object IEnumerator.Current => null;
                bool IEnumerator.MoveNext() { return false; }
                void IEnumerator.Reset() { }
            }
        }
    }
}