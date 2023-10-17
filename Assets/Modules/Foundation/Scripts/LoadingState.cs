
namespace Foundation {


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class LoadingState : Game.State {

        public interface IJob {
            string name { get; }
            IEnumerator start();
            float progress { get; }
        }

        public interface IJobStatus {
            float progress { set; }
        }

        public delegate bool TryGetBundleAssetPath(out string bundle, out string path);

        public float progress {
            get {
                int total = m_jobs_weight + m_completed_weight;
                if (m_current.job != null) {
                    total += m_current.weight;
                    return (m_completed_weight + Mathf.Clamp01(m_current.job.progress) * m_current.weight) / total;
                }
                return (float)m_completed_weight / total;
            }
        }

        public override void enter(Game.IState last) {
            m_coroutine = Game.instance.StartCoroutine(working());
        }

        public override void leave() {
            abort();
        }

        public virtual void abort() {
            if (m_coroutine != null) {
                Game.instance.StopCoroutine(m_coroutine);
                m_coroutine = null;
            }
        }

        public void add_job(IJob job, int weight = 1) {
            weight = Mathf.Max(0, weight);
            m_jobs_weight += weight;
            m_jobs.Enqueue(new JobItem(job, weight));
        }

        public void add_job(string name, System.Action job, int weight = 1) {
            weight = Mathf.Max(0, weight);
            m_jobs_weight += weight;
            m_jobs.Enqueue(new JobItem(new Job0(name, job), weight));
        }

        public void add_job(string name, System.Func<IJobStatus, IEnumerator> job, int weight = 1) {
            weight = Mathf.Max(0, weight);
            m_jobs_weight += weight;
            m_jobs.Enqueue(new JobItem(new Job1(name, job), weight));
        }

        protected virtual IEnumerator prepare() { yield break; }
        protected virtual void on_current_changed(IJob current) { }

        private class Job0 : IJob {

            public string name { get; }
            public System.Action action { get; }

            public Job0(string name, System.Action action) {
                this.name = name;
                this.action = action;
            }

            float IJob.progress => 0;
            IEnumerator IJob.start() {
                action?.Invoke();
                yield break;
            }
        }

        private class Job1 : IJob, IJobStatus {
            public string name { get; }

            public Job1(string name, System.Func<IJobStatus, IEnumerator> job) {
                this.name = name;
                this.job = job;
            }

            float IJob.progress => progress;

            float IJobStatus.progress { set => progress = value; }

            IEnumerator IJob.start() {
                return job?.Invoke(this);
            }

            float progress;
            System.Func<IJobStatus, IEnumerator> job;
        }

        public class UnloadSceneJob : IJob {
            string scene_name;
            AsyncOperation ao;
            System.Action callback;

            public string name { get; }
            public UnloadSceneJob(string name, string scene_name, System.Action callback) {
                this.name = name;
                this.scene_name = scene_name;
                this.callback = callback;
            }

            float IJob.progress => ao != null ? ao.progress : 0;

            IEnumerator IJob.start() {
                ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene_name);
                yield return ao;
                callback?.Invoke();
            }
        }

        public class LoadSceneJob : IJob {
            string scene_bundle;
            string scene_path;
            AssetBundleManager.ISceneRequest req;
            UnityEngine.SceneManagement.LoadSceneParameters param;
            System.Action callback;

            public string name { get; }
            public LoadSceneJob(string name, string scene_bundle, string scene_path, UnityEngine.SceneManagement.LoadSceneParameters param, System.Action callback) {
                this.name = name;
                this.scene_bundle = scene_bundle;
                this.scene_path = scene_path;
                this.param = param;
                this.callback = callback;
            }


            float IJob.progress => req != null ? req.progress : 0;

            IEnumerator IJob.start() {
                req = AssetBundleManager.instance.load_scene_async(scene_bundle, scene_path, param);
                yield return req;
                callback?.Invoke();
            }
        }

        public class LoadBundleAsset<T> : IJob where T: Object {
            string bundle;
            string path;
            TryGetBundleAssetPath func;
            System.Action<Error, T> callback;
            AssetBundleManager.IAssetRequest req;


            public string name { get; }
            public LoadBundleAsset(string name, string bundle, string path, System.Action<Error, T> callback) {
                this.name = name;
                this.bundle = bundle;
                this.path = path;
                this.callback = callback;
            }

            public LoadBundleAsset(string name, TryGetBundleAssetPath func, System.Action<Error, T> callback) {
                this.name = name;
                this.func = func ?? throw new System.ArgumentNullException();
                this.callback = callback;
            }

            float IJob.progress => req != null ? req.progress : 0;

            IEnumerator IJob.start() {
                func?.Invoke(out bundle, out path);
                req = AssetBundleManager.instance.load_asset_async<T>(bundle, path);
                yield return req;
                callback?.Invoke(req.error, req.asset as T);
            }
        }

        private IEnumerator working() {
            yield return prepare();
            for (; ; ) {
                if (m_jobs.Count == 0) {
                    yield return null;
                } else {
                    for (; ; ) {
                        m_current = m_jobs.Dequeue();
                        m_jobs_weight -= m_current.weight;
                        on_current_changed(m_current.job);
                        yield return m_current.job.start();
                        m_completed_weight += m_current.weight;
                        m_current.job = null;
                        m_current.weight = 0;
                        if (m_jobs.Count == 0) {
                            break;
                        }
                    }
                    on_current_changed(null);
                }
            }
        }

        struct JobItem {
            public int weight;
            public IJob job;

            public JobItem(IJob job, int weight) {
                this.job = job;
                this.weight = weight;
            }
        }

        Queue<JobItem> m_jobs = new Queue<JobItem>();
        int m_jobs_weight = 0;
        int m_completed_weight = 0;
        Coroutine m_coroutine = null;
        JobItem m_current;

    }

}
