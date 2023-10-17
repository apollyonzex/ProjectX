
using UnityEngine;
using Foundation.Tables;
using System.Reflection;

namespace Foundation {

    public abstract class DB {

        public void load_all(string bundle) {
            var table_type = typeof(ITable);
            foreach (var fi in GetType().GetFields()) {
                if (table_type.IsAssignableFrom(fi.FieldType)) {
                    load(fi, bundle, fi.Name);
                }
            }
        }

        public void load_all(string bundle, string path) {
            var table_type = typeof(ITable);
            foreach (var fi in GetType().GetFields()) {
                if (table_type.IsAssignableFrom(fi.FieldType)) {
                    load(fi, bundle, System.IO.Path.Combine(path, fi.Name));
                }
            }
        }

        private void load(FieldInfo fi, string bundle, string path) {
            var err = AssetBundleManager.instance.load_asset<BinaryAsset>(bundle, path, out var asset);
            bytes_loaded(fi, err, asset?.bytes);
        }

        public void add_loading_jobs(LoadingState loading, string bundle, string name_format) {
            var table_type = typeof(ITable);
            foreach (var fi in GetType().GetFields()) {
                if (table_type.IsAssignableFrom(fi.FieldType)) {
                    add_loading_job(loading, fi, bundle, fi.Name, string.Format(name_format, fi.Name));
                }
            }
        }

        public void add_loading_jobs(LoadingState loading, string bundle, string path, string name_format) {
            if (string.IsNullOrWhiteSpace(path)) {
                add_loading_jobs(loading, bundle, name_format);
                return;
            }
            var table_type = typeof(ITable);
            foreach (var fi in GetType().GetFields()) {
                if (table_type.IsAssignableFrom(fi.FieldType)) {
                    add_loading_job(loading, fi, bundle, System.IO.Path.Combine(path, fi.Name), string.Format(name_format, fi.Name));
                }
            }
        }

        private void add_loading_job(LoadingState loading, FieldInfo fi, string bundle, string path, string name) {
            loading.add_job(new LoadingState.LoadBundleAsset<BinaryAsset>(name, bundle, path, (err, asset) => bytes_loaded(fi, err, asset?.bytes)));
        }

        private void bytes_loaded(FieldInfo fi, Error err, byte[] bytes) {
            if (bytes != null) {
                if (!(fi.GetValue(this) as ITable).load_from(bytes)) {
                    Debug.LogError($"load table \'{fi.Name}\' failed");
                }
            } else {
                Debug.LogError($"load table asset \'{fi.Name}\' failed: {err}");
            }
        }
    }

}