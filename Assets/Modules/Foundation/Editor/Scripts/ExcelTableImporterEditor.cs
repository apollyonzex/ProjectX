using UnityEditor;

#if UNITY_2020_1_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

using UnityEngine;

using System.Collections.Generic;

namespace Foundation.Editor {
    
    [CustomEditor(typeof(ExcelTableImporter))]
    public class ExcelTableImporterEditor : ScriptedImporterEditor, ExcelTableUtility.IOutput {

        private string m_err_msg = null;
       
        private class ItemInfo {
            public string sheet_name;
            public uint sheet_id;
            public bool export;
        }

        private List<ItemInfo> m_edit_items = new List<ItemInfo>();

        public new ExcelTableImporter target => base.target as ExcelTableImporter;

        private static HashSet<ExcelTableImporterEditor> s_editors = new HashSet<ExcelTableImporterEditor>();

        public static void notify_imported(string asset_path) {
            foreach (var editor in s_editors) {
                if (editor.target.assetPath == asset_path) {
                    editor.refresh();
                    editor.Repaint();
                }
            }
        }

        public override void OnEnable() {
            base.OnEnable();

            s_editors.Add(this);

            if (System.IO.Path.GetFileName(target.assetPath).StartsWith("~$")) {
                return;
            }

            refresh();
        }

        public override void OnDisable() {
            base.OnDisable();
            s_editors.Remove(this);
        }

        void ExcelTableUtility.IOutput.output(string content) {
            if (string.IsNullOrEmpty(m_err_msg)) {
                m_err_msg = content;
            } else {
                m_err_msg += "\n";
                m_err_msg += content;
            }
        }

        void ExcelTableUtility.IOutput.info(string name, uint id) {
            m_edit_items.Add(new ItemInfo {
                sheet_name = name,
                sheet_id = id,
            });
        }

        private bool touch_sheet(string sheet_name) {
            foreach (var item in m_edit_items) {
                if (item.sheet_name == sheet_name) {
                    item.export = true;
                    return true;
                }
            }
            return false;
        }

        public override void OnInspectorGUI() {

            Undo.RecordObject(base.target, "ExcelImport");

            var asset = assetTarget as ExcelFileAsset;

            var has_sheet_error = false;
            var err_index = 0;
            if (asset != null && asset.errMsgs != null && asset.errMsgs.Count > 0) {
                var err_msg = asset.errMsgs[0];
                if (!string.IsNullOrEmpty(err_msg)) {
                    EditorGUILayout.HelpBox(err_msg, MessageType.Error);
                } else {
                    has_sheet_error = true;
                    err_index += 1;
                }
            }

            var target = this.target;

            if (target.items != null) {
                var item_index = -1;
                foreach (var item in target.items) {
                    item_index += 1;
                    var has_export_error = false;
                    GUILayout.BeginVertical(item.sheet_name, GUI.skin.window);
                    if (has_sheet_error) {
                        var err_msg = asset.errMsgs[err_index];
                        err_index += 1;
                        if (!string.IsNullOrEmpty(err_msg)) {
                            EditorGUILayout.HelpBox(err_msg, MessageType.Error);
                        } else {
                            has_export_error = true;
                        }
                    }
                    if (touch_sheet(item.sheet_name)) {
                        if (GUILayout.Button("Add Export")) {
                            if (item.exports == null) {
                                item.exports = new List<ExcelTableImporter.ExportInfo>();
                            }
                            var count = item.exports.Count;
                            item.exports.Add(new ExcelTableImporter.ExportInfo {
                                name = item.sheet_name,
                            });
                            if (has_export_error) {
                                Undo.RecordObject(asset, "Add Export");
                                asset.errMsgs.Insert(err_index + count, null);
                            }
                        }
                        if (item.exports != null) {
                            var export_index = -1;
                            foreach (var export in item.exports) {
                                export_index += 1;
                                GUILayout.BeginVertical(GUI.skin.box);
                                export.name = EditorGUILayout.TextField("Name", export.name);
                                if (string.IsNullOrEmpty(export.name)) {
                                    export.name = item.sheet_name;
                                }
                                export.has_mask = EditorGUILayout.Toggle("Mask", export.has_mask);
                                if (export.has_mask) {
                                    export.mask = (uint)Mathf.Max(EditorGUILayout.IntField((int)export.mask), 0);
                                }
                                export.generate_asset = EditorGUILayout.Toggle("Generate Asset", export.generate_asset);
                                export.generate_code = EditorGUILayout.Toggle("Generate Code", export.generate_code);
                                if (export.generate_code) {
                                    export.code_lang = (ExcelTableImporter.CodeLang)EditorGUILayout.EnumPopup("Language", export.code_lang);
                                    if (export.code_lang == ExcelTableImporter.CodeLang.Rust) {
                                        GUILayout.BeginHorizontal();
                                        EditorGUI.BeginChangeCheck();
                                        export.code_path = EditorGUILayout.TextField("Output", export.code_path);
                                        if (EditorGUI.EndChangeCheck()) {
                                            export.code_path = FileUtil.GetProjectRelativePath(export.code_path);
                                        }
                                        if (GUILayout.Button("Select", GUILayout.ExpandWidth(false))) {
                                            EditorApplication.delayCall += () => {
                                                var dir = EditorUtility.SaveFolderPanel("Select Rust Code Folder", export.code_path, null);
                                                if (!string.IsNullOrEmpty(dir)) {
                                                    export.code_path = FileUtil.GetProjectRelativePath(dir);
                                                }
                                            };
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }
                                if (has_export_error) {
                                    var err_msg = asset.errMsgs[err_index];
                                    err_index += 1;
                                    if (!string.IsNullOrEmpty(err_msg)) {
                                        EditorGUILayout.HelpBox(err_msg, MessageType.Error);
                                    }
                                }
                                if (GUILayout.Button("Remove")) {
                                    var i1 = export_index;
                                    var i2 = err_index - 1;
                                    var remove_err = has_export_error;
                                    EditorApplication.delayCall += () => {
                                        Undo.RecordObject(target, "Remove Export");
                                        item.exports.RemoveAt(i1);
                                        if (remove_err) {
                                            Undo.RecordObject(asset, "Remove Export");
                                            asset.errMsgs.RemoveAt(i2);
                                        }
                                    };
                                }
                                GUILayout.EndVertical();
                            }
                        }
                    } else {
                        if (GUILayout.Button("Delete")) {
                            var i1 = item_index;
                            var i2 = err_index - 1;
                            var remove_err = has_sheet_error;
                            EditorApplication.delayCall += () => {
                                Undo.RecordObject(target, "Remove Config");
                                target.items.RemoveAt(i1);
                                if (remove_err) {
                                    Undo.RecordObject(asset, "Remove Config");
                                    asset.errMsgs.RemoveAt(i2);
                                }
                            };
                        }
                        if (has_export_error && item.exports != null) {
                            err_index += item.exports.Count;
                        }
                    }
                    GUILayout.EndVertical();
                }
            }

            foreach (var edit_item in m_edit_items) {
                if (edit_item.export) {
                    continue;
                }
                GUILayout.BeginVertical(edit_item.sheet_name, GUI.skin.window);
                if (GUILayout.Button("Add Export")) {
                    if (target.items == null) {
                        target.items = new List<ExcelTableImporter.TableConfig>();
                    }
                    var item = new ExcelTableImporter.TableConfig {
                        sheet_name = edit_item.sheet_name,
                        exports = new List<ExcelTableImporter.ExportInfo>()
                    };
                    item.exports.Add(new ExcelTableImporter.ExportInfo {
                        name = edit_item.sheet_name,
                    });
                    target.items.Add(item);
                    if (has_sheet_error) {
                        Undo.RecordObject(asset, "Add Config");
                        asset.errMsgs.Add(null);
                        asset.errMsgs.Add(null);
                    }
                }
                GUILayout.EndVertical();
            }

            EditorUtility.SetDirty(target);

            ApplyRevertGUI();
        }

        private void refresh() {
            m_err_msg = null;
            m_edit_items.Clear();

            var target = this.target;

            var file = ExcelTableUtility.ExcelFile.open(target.assetPath, this);
            if (file != null) {
                using (file) {
                    file.load_workbook(this);
                }
            }
        }

        //private void on_inspector_gui_items() {
        //    var target = this.target;

        //    //var asset_name = System.IO.Path.GetFileNameWithoutExtension(target.assetPath);

        //    if (target.items != null) {
        //        for (int i = 0; i < target.items.Length; ++i) {
        //            var item = target.items[i];
        //            bool found = false;
        //            for (int j = 0; j < m_edit_items.Count; ++j) {
        //                var edit_item = m_edit_items[j];
        //                if (edit_item.sheet_name == item.sheet_name) {
        //                    edit_item.config_index = i;
        //                    edit_item.name = item.name;
        //                    if (item.has_mask) {
        //                        edit_item.mask = item.mask;
        //                    } else {
        //                        edit_item.mask = null;
        //                    }
        //                    edit_item.err_msg = get_item_err_msg(i);
        //                    edit_item.generate_code = item.generate_code;
        //                    found = true;
        //                    break;
        //                }
        //            }
        //            if (!found) {
        //                m_edit_items.Add(new ItemInfo {
        //                    sheet_name = item.sheet_name,
        //                    name = item.name,
        //                    config_index = i,
        //                    mask = item.has_mask ? (uint?)item.mask : null,
        //                    generate_code = item.generate_code,
        //                    err_msg = get_item_err_msg(i),
        //                });
        //            }
        //        }
        //    }


        //    foreach (var edit_item in m_edit_items) {
        //        if (edit_item.sheet_id > 0) {
        //            GUILayout.BeginVertical(edit_item.sheet_name, GUI.skin.window);
        //        } else {
        //            GUILayout.BeginVertical("[Not Exist] " + edit_item.sheet_name, GUI.skin.window);
        //        }
        //        EditorGUI.BeginChangeCheck();
        //        var import = EditorGUILayout.Toggle("Import", edit_item.config_index != -1);
        //        SerializedProperty sp_item = null;
        //        if (EditorGUI.EndChangeCheck()) {
        //            if (import) {
        //                var index = m_sp_items.arraySize;
        //                ++m_sp_items.arraySize;
        //                sp_item = m_sp_items.GetArrayElementAtIndex(index);
        //                sp_item.FindPropertyRelative("sheet_name").stringValue = edit_item.sheet_name;
        //                sp_item.FindPropertyRelative("name").stringValue = edit_item.name;
        //                sp_item.FindPropertyRelative("has_mask").boolValue = edit_item.mask.HasValue;
        //                sp_item.FindPropertyRelative("mask").intValue = (int)edit_item.mask.GetValueOrDefault();
        //                sp_item.FindPropertyRelative("generate_code").boolValue = edit_item.generate_code;
                        
        //                edit_item.config_index = index;
        //                add_item_err_msg(edit_item.err_msg);
        //            } else {
        //                var index = edit_item.config_index;
        //                edit_item.config_index = -1;
        //                m_sp_items.DeleteArrayElementAtIndex(index);
        //                remove_item_err_msg(index);
                        
        //                foreach (var item in m_edit_items) {
        //                    if (item.config_index > index) {
        //                        --item.config_index;
        //                        break;
        //                    }
        //                }
        //            }
        //        } else if (import) {
        //            sp_item = m_sp_items.GetArrayElementAtIndex(edit_item.config_index);
        //        }

        //        if (import) {
        //            edit_item.name = EditorGUILayout.TextField("Name", edit_item.name);
        //            if (string.IsNullOrEmpty(edit_item.name)) {
        //                edit_item.name = edit_item.sheet_name;
        //            }
        //            var has_mask = EditorGUILayout.Toggle("Mask", edit_item.mask.HasValue);
        //            if (has_mask) {
        //                edit_item.mask = (uint)Mathf.Max(EditorGUILayout.IntField((int)edit_item.mask.GetValueOrDefault(0)), 0);
        //            } else {
        //                edit_item.mask = null;
        //            }
        //            edit_item.generate_code = EditorGUILayout.Toggle("Generate Code", edit_item.generate_code);
        //            if (!string.IsNullOrEmpty(edit_item.err_msg)) {
        //                EditorGUILayout.HelpBox(edit_item.err_msg, MessageType.Error);
        //            }
        //        }
                

        //        if (sp_item != null) {
        //            sp_item.FindPropertyRelative("name").stringValue = edit_item.name;
        //            sp_item.FindPropertyRelative("has_mask").boolValue = edit_item.mask.HasValue;
        //            sp_item.FindPropertyRelative("mask").intValue = (int)edit_item.mask.GetValueOrDefault(0);
        //            sp_item.FindPropertyRelative("generate_code").boolValue = edit_item.generate_code;
        //        }

        //        GUILayout.EndVertical();
        //    }
        //}
    }
    
}