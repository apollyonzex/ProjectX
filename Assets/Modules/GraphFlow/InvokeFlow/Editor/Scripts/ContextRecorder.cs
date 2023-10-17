
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Runtime.InteropServices;

using GraphNode.Editor;

namespace InvokeFlow.Editor {
    public class ContextRecorder : EditorWindow {

        [MenuItem("Window/Analysis/InvokeFlow Recorder")]
        public static void open() {
            var wnd = GetWindow<ContextRecorder>();
            wnd.Show();
        }

        public static ContextRecorder instance { get; private set; }

        public static bool is_current(IContext context) {
            if (instance != null && instance.m_current != null) {
                return instance.m_current.Value.Target == context;
            }
            return false;
        }

        public static void notify_recorded() {
            if (instance != null) {
                if (instance.m_select_new_unless_recorded) {
                    instance.m_select_new_unless_recorded = false;
                    instance.Repaint();
                }
            }
        }

        private void OnEnable() {
            titleContent = new GUIContent("InvokeFlow Recorder");
            instance = this;
            Recorder.context_event += on_context_event;
            m_last_update_time = EditorApplication.timeSinceStartup;
            EditorApplication.update += on_editor_update;
            EditorApplication.playModeStateChanged += on_play_mode_changed;
        }

        private void OnDisable() {
            free_all_handles();
            instance = null;
            Recorder.context_event -= on_context_event;
            EditorApplication.update -= on_editor_update;
            EditorApplication.playModeStateChanged -= on_play_mode_changed;
        }

        private void OnGUI() {
            m_select_new_unless_recorded = EditorGUILayout.Toggle("Select New Unless Recorded", m_select_new_unless_recorded);
            m_scroll_position = EditorGUILayout.BeginScrollView(m_scroll_position, GUILayout.ExpandHeight(true));
            for (var node = m_handles.First; node != null; node = node.Next) {
                var handle = node.Value;
                if (handle.Target is IContext ctx) {
                    var content = node == m_current ? $"-- {ctx.debug_name} --" : ctx.debug_name;
                    if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.ExpandWidth(true))) {
                        if (m_current != node) {
                            m_current = node;
                        } else {
                            m_current = null;
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            m_picked_asset = EditorGUILayout.ObjectField(m_picked_asset, typeof(InvokeGraphAsset), false) as InvokeGraphAsset;
            GUI.enabled = m_picked_asset != null;
            if (GUILayout.Button("Open", GUILayout.ExpandWidth(false))) {
                GraphEditorWindow.open_runtime(m_picked_asset);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        Vector2 m_scroll_position;

        void on_context_event(IContext context, bool status) {
            if (status) {
                var handle = GCHandle.Alloc(context, GCHandleType.Weak);
                var item = m_handles.AddLast(handle);
                if (m_select_new_unless_recorded) {
                    m_current = item;
                }
                Repaint();
            } else {
                var need_repaint = false;
                for (var node = m_handles.First; node != null; node = node.Next) {
                    var handle = node.Value;
                    if (handle.Target == context) {
                        handle.Free();
                        if (m_current == node) {
                            m_current = null;
                        }
                        m_handles.Remove(node);
                        need_repaint = true;
                        break;
                    }
                }
                if (need_repaint) {
                    Repaint();
                }
            }
        }

        void on_editor_update() {
            var now = EditorApplication.timeSinceStartup;
            var delta = now - m_last_update_time;
            if (delta >= 1) {
                bool need_repaint = false;
                m_last_update_time = now;
                for (var node = m_handles.First; node != null; ) {
                    var handle = node.Value;
                    if (handle.Target == null) {
                        handle.Free();
                        if (m_current == node) {
                            m_current = null;
                        }
                        var t = node;
                        node = node.Next;
                        m_handles.Remove(t);
                        need_repaint = true;
                    } else {
                        node = node.Next;
                    }
                }
                if (need_repaint) {
                    Repaint();
                }
            }
        }

        void on_play_mode_changed(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingPlayMode) {
                free_all_handles();
            }
        }

        void free_all_handles() {
            foreach (var handle in m_handles) {
                handle.Free();
            }
            m_handles.Clear();
            m_current = null;
        }

        double m_last_update_time;
        LinkedList<GCHandle> m_handles = new LinkedList<GCHandle>();
        LinkedListNode<GCHandle> m_current = null;

        InvokeGraphAsset m_picked_asset = null;
        bool m_select_new_unless_recorded = false;
    }
}