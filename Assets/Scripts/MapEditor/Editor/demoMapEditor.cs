using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace demo_editor {
    [CustomEditor(typeof(MapEditorRoot))]
    public class demoMapEditor : Editor {

        MapEditorRoot root;

        enum mouseType {
            Point,
            Line,
            None,
        }

        private mouseType mouse_type =mouseType.None;
        private Site startSite, endSite;
        private void OnEnable() {
            root = (MapEditorRoot)target;
            root.transform.position = Vector3.zero;
            SceneView.duringSceneGui += mouseFunc;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= mouseFunc;
        }

        private void mouseFunc(SceneView view) {
            if(mouse_type == mouseType.Point) {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                    var mousePos = Event.current.mousePosition;
                    mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y;
                    var pos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(mousePos);
                    root.create_point(pos);
                }
            }
            else if(mouse_type == mouseType.Line) {
               
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                    var mousePos = Event.current.mousePosition;
                    mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y;
                    var pos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(mousePos);
                    var hit = Physics2D.Raycast(pos, Vector2.zero);
                    if (hit) {
                        startSite = hit.transform.GetComponent<Site>();
                    }
                }

                if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                    var mousePos = Event.current.mousePosition;
                    mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y;
                    var pos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(mousePos);
                    var hit = Physics2D.Raycast(pos, Vector2.zero);
                    if (hit) {
                        endSite = hit.transform.GetComponent<Site>();
                        if (startSite != null && endSite != null && startSite != endSite) {
                            if(startSite.next_sites.Contains(endSite) == false) {
                                startSite.next_sites.Add(endSite);
                                view.Repaint();
                            }
                        }
                    } else {
                        startSite = null;
                    }
                }
            } else {

            }
        }




        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("save")){
                root.save();
            }
        }

        private void OnSceneGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 100, 100));
            if(mouse_type == mouseType.None) {
                GUILayout.Label("看看模式");
            }
            else if (mouse_type == mouseType.Point) {
                GUILayout.Label("生成点模式");
            } else {
                GUILayout.Label("连线模式");
            }
            if (GUILayout.Button("看看模式")) {
                mouse_type = mouseType.None;
            }
            if (GUILayout.Button("生成点模式")) {
                mouse_type = mouseType.Point;
            }
            if (GUILayout.Button("连线模式")) {
                mouse_type = mouseType.Line;
            }
            GUILayout.EndArea();
        }
    }
}

