
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {
    [PropertyEditor(typeof(AddressableAsset<>))]
    public class PropertyAddressableAssetEditor<T> : GenericPropertyEditor where T : Object {

        public AddressableAsset<T> target => m_target;
        public T asset => m_asset;

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node) {
            base.attach(obj, fi, graph, node);
            m_target = fi.GetValue(obj) as AddressableAsset<T>;
            if (m_target == null) {
                m_target = new AddressableAsset<T>();
                fi.SetValue(obj, target);
            }
            load_asset();
        }

        public override void attach(object obj, FieldInfo fi, GraphEditor graph, NodeEditor node, string name, int order = 0, ShowInBodyAttribute show_in_body = null) {
            base.attach(obj, fi, graph, node, name, order, show_in_body);
            if (fi != null) {
                m_target = fi.GetValue(obj) as AddressableAsset<T>;
                if (m_target == null) {
                    m_target = new AddressableAsset<T>();
                    fi.SetValue(obj, target);
                }
            } else {
                m_target = obj as AddressableAsset<T> ?? new AddressableAsset<T>();
            }
            load_asset();
        }

        public override void clone_to(object target) {
            if (m_fi != null) {
                m_fi.SetValue(target, new AddressableAsset<T> {
                    bundle = m_target.bundle,
                    path = m_target.path,
                });
            }
        }

        public override void on_body_gui() {
            if (m_show_in_body != null) {
                string content;
                if (m_asset == null) {
                    content = "<None>";
                } else {
                    content = $"'{asset.name}'";
                }
                if (m_show_in_body.format != null) {
                    content = string.Format(m_show_in_body.format, content);
                }
                GUILayout.Label(content);
            }
        }

        public override void on_inspector_enable() {

        }

        public override void on_inspector_disable() {
            m_cmd = null;
        }

        public override void on_inspector_gui() {
            if (!string.IsNullOrEmpty(name)) {
                EditorGUILayout.PrefixLabel(name);
            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            var bundle = EditorGUILayout.TextField("Bundle", m_target.bundle)?.Trim();
            var path = EditorGUILayout.TextField("Path", m_target.path)?.Trim();
            if (m_target.bundle != bundle || m_target.path != path) {
                record_change(bundle, path);
            }
            var asset = EditorGUILayout.ObjectField("Asset", m_asset, typeof(T), false);
            if (asset != m_asset) {
                asset = validate_asset(asset, out bundle, out path);
                if (asset != m_asset && (bundle != m_target.bundle || path != m_target.path)) {
                    m_asset = asset as T;
                    record_change(bundle, path);
                }
            }
            EditorGUILayout.EndVertical();
        }

        static Object validate_asset(Object asset, out string bundle, out string path) {
            if (asset != null) {
                var asset_path = AssetDatabase.GetAssetPath(asset);
                if (asset_path.StartsWith("Assets/Resources/RawResources/")) {
                    asset_path = asset_path.Substring(30);
                    var idx = asset_path.IndexOf('/');
                    if (idx >= 0) {
                        bundle = asset_path.Substring(0, idx);
                        path = asset_path.Substring(idx + 1);
                        idx = path.LastIndexOf('.');
                        if (idx >= 0) {
                            path = path.Substring(0, idx);
                        }
                        return asset;
                    }
                }
            }
            bundle = null;
            path = null;
            return null;
        }

        void record_change(string bundle, string path) {
            var undo = m_graph.view.undo;
            if (m_cmd != null && undo.is_last(m_cmd)) {
                m_target.bundle = bundle;
                m_target.path = path;
                m_cmd.new_value = (bundle, path);

                undo.begin_group();
                notify_changed(true);
                m_cmd.associated = undo.cancel_group();
                if (m_cmd.associated != null) {
                    m_cmd = null;
                }
            } else {
                var old_value = (m_target.bundle, m_target.path);
                m_target.bundle = bundle;
                m_target.path = path;

                undo.begin_group();
                notify_changed(true);
                m_cmd = new ChangeValue() {
                    editor = this,
                    old_value = old_value,
                    new_value = (bundle, path),
                    associated = undo.cancel_group(),
                };
                undo.record(m_cmd);
                if (m_cmd.associated != null) {
                    m_cmd = null;
                }
            }
        }

        void load_asset() {
            if (string.IsNullOrEmpty(m_target.bundle) || string.IsNullOrEmpty(m_target.path)) {
                m_asset = null;
            } else {
                m_asset = Resources.Load<T>($"RawResources/{m_target.bundle}/{m_target.path}");
            }
        }

        protected override void notify_changed(bool by_user) {
            load_asset();
            base.notify_changed(by_user);
        }



        AddressableAsset<T> m_target;
        T m_asset;

        class ChangeValue : GraphUndo.ICommand {

            public (string, string) old_value;
            public (string, string) new_value;
            public PropertyAddressableAssetEditor<T> editor;
            public GraphUndo.CommandGroup associated;


            public void undo() {
                editor.m_target.bundle = old_value.Item1;
                editor.m_target.path = old_value.Item2;
                associated?.undo();
                editor.notify_changed(false);
            }

            public void redo() {
                editor.m_target.bundle = new_value.Item1;
                editor.m_target.path = new_value.Item2;
                associated?.redo();
                editor.notify_changed(false);
            }

            public int dirty_count => 1;
        }

        ChangeValue m_cmd = null;
    }
}