
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GraphNode.Editor;

namespace BehaviourFlow.Editor {
    public class BehaviourTreeDebugger : EditorWindow {

        [MenuItem("Window/Analysis/BehaviourTree Debugger")]
        public static void open() {
            var wnd = GetWindow<BehaviourTreeDebugger>();
            wnd.Show();
        }

        public static BehaviourTreeDebugger instance { get; private set; }

        private void OnEnable() {
            titleContent = new GUIContent("BehaviourTree Debugger");
            BTExecutorBase.instance_event += on_instance_event;
            instance = this;
        }



        private void OnDisable() {
            instance = null;
            current = null;
            BTExecutorBase.instance_event -= on_instance_event;
        }


        private void OnGUI() {
            EditorGUILayout.BeginVertical();
            m_scroll_position = EditorGUILayout.BeginScrollView(m_scroll_position, GUILayout.ExpandHeight(true));
            foreach (var exec in BTExecutorBase.all_instances) {
                if (exec.parent != null) {
                    continue;
                }
                if (GUILayout.Button(exec.name, EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true))) {
                    current = exec;
                    if (exec.game_object != null) {
                        EditorGUIUtility.PingObject(exec.game_object);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            foreach (var kvp in m_current_assets) {
                if (GUILayout.Button(kvp.Key.name, EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true))) {
                    GraphEditorWindow.open_runtime(kvp.Key);
                }
            }
            EditorGUILayout.BeginHorizontal();
            m_picked_asset = EditorGUILayout.ObjectField(m_picked_asset, typeof(BehaviourTreeAsset), false) as BehaviourTreeAsset;
            GUI.enabled = current != null && m_picked_asset != null;
            if (GUILayout.Button("Open")) {
                GraphEditorWindow.open_runtime(m_picked_asset);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        Vector2 m_scroll_position;

        public BTExecutorBase current {
            get => m_current;
            private set {
                if (m_current != value) {
                    m_current_assets.Clear();
                    if (m_current != null) {
                        foreach (var e in m_current.enumerate_executors()) {
                            e.stack_event -= on_stack_event;
                        }
                    }
                    m_current = value;
                    if (m_current != null) {
                        foreach (var e in m_current.enumerate_executors()) {
                            e.stack_event += on_stack_event;
                            foreach (var (asset, _) in e.pending_nodes) {
                                on_stack_event(asset, true);
                            }
                        }
                    }
                }
            }
        }

        private void on_stack_event(BehaviourTreeAsset asset, bool push) {
            if (!m_current_assets.TryGetValue(asset, out var info)) {
                info = new AssetInfo();
                m_current_assets.Add(asset, info);
            }
            if (push) {
                if (++info.count == 1) {
                    Repaint();
                }
            } else {
                if (--info.count == 0) {
                    m_current_assets.Remove(asset);
                    Repaint();
                }
            }
        }

        private void on_instance_event(BTExecutorBase exec, bool added) {
            bool repaint = false;
            if (exec.parent == null) {
                repaint = true;
            }
            if (added) {
                if (current != null && current == exec.root) {
                    exec.stack_event += on_stack_event;
                }
            } else {
                if (current != null) {
                    if (current == exec.root) {
                        exec.stack_event -= on_stack_event;
                    }
                    if (current == exec) {
                        m_current_assets.Clear();
                        m_current = null;
                    }
                }
            }
            if (repaint) {
                Repaint();
            }
        }

        private BTExecutorBase m_current;

        private class AssetInfo {
            public int count;
        }

        private Dictionary<BehaviourTreeAsset, AssetInfo> m_current_assets = new Dictionary<BehaviourTreeAsset, AssetInfo>();
        private BehaviourTreeAsset m_picked_asset = null;
    }
}