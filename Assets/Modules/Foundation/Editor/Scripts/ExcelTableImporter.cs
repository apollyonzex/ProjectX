
using UnityEngine;
using UnityEditor;

#if UNITY_2020_1_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

using System.IO;
using System.Collections.Generic;

namespace Foundation.Editor {


    [ScriptedImporter(6, new string[] { "xlsx", "xlsm" }, AllowCaching = false)]
    public class ExcelTableImporter : ScriptedImporter, ExcelTableUtility.IOutput {

        [System.Serializable]
        public class TableConfig {
            public string sheet_name;
            public List<ExportInfo> exports;
        }

        public enum CodeLang {
            CShape,
            Rust,
        }

        [System.Serializable]
        public class ExportInfo {
            public string name;
            public bool has_mask;
            public uint mask;
            public bool generate_asset = true;
            public bool generate_code = true;
            public CodeLang code_lang;
            public string code_path;

            public uint get_mask() => has_mask ? mask : 0xFFFFFFFF;
            public string get_code_path() {
                return code_lang switch {
                    CodeLang.CShape => $"Assets/AutoCode/Tables/{name}.cs",
                    _ => Path.Combine(code_path, $"{name}.rs"),
                };
            }
        }
        
        public List<TableConfig> items;

        public override void OnImportAsset(AssetImportContext ctx) {
            if (Path.GetFileName(ctx.assetPath).StartsWith("~$")) {
                return;
            }

            var self = ScriptableObject.CreateInstance<ExcelFileAsset>();
            ctx.AddObjectToAsset("_", self, EditorResources.instance.excelFileAssetIcon);
            ctx.SetMainObject(self);
            m_sheets.Clear();

            var refresh = false;

            var err_msg = new List<string>();
            m_err_msg = null;

            bool failed = false;

            var file = ExcelTableUtility.ExcelFile.open(ctx.assetPath, this);
            if (file != null) {
                if (!file.load_workbook(this)) {
                    file.dispose();
                    file = null;
                }
            }
            err_msg.Add(m_err_msg);
            if (file != null) {
                if (items != null) {
                    var assets = new List<BinaryAsset>(items.Count);
                    foreach (var item in items) {
                        if (item.exports != null && item.exports.Count != 0) {
                            if (m_sheets.TryGetValue(item.sheet_name, out var sheet_id)) {
                                m_err_msg = null;
                                var sheet = file.load_sheet(sheet_id, this);
                                if (sheet != null) {
                                    err_msg.Add(null);
                                    foreach (var export in item.exports) {
                                        BinaryAsset asset = null;
                                        if (export.generate_asset) {
                                            asset = ScriptableObject.CreateInstance<BinaryAsset>();
                                            asset.name = export.name;
                                            assets.Add(asset);
                                            ctx.AddObjectToAsset(export.name, asset);
                                        }

                                        m_err_msg = null;
                                        var builder = sheet.create_builder(export.get_mask(), parsers, this);
                                        if (builder != null) {
                                            if (export.generate_asset) {
                                                var table_data = builder.generate_table(this);
                                                if (table_data != null) {
                                                    asset.bytes = table_data.get_content();
                                                    table_data.dispose();
                                                } else {
                                                    failed = true;
                                                }
                                            }
                                            if (export.generate_code) {
                                                var code_path = export.get_code_path();
                                                if (!string.IsNullOrEmpty(code_path)) {
                                                    var code_dir = Path.GetDirectoryName(code_path);
                                                    if (Directory.Exists(code_dir)) {
                                                        try {
                                                            var data = export.code_lang switch {
                                                                CodeLang.CShape => builder.generate_code(export.name),
                                                                CodeLang.Rust => builder.generate_rust_code(export.name),
                                                                _ => null,
                                                            };
                                                            if (data != null) {
                                                                using (data) {
                                                                    if (File.Exists(code_path)) {
                                                                        var file_bytes = File.ReadAllBytes(code_path);
                                                                        if (!data.Equals(file_bytes)) {
                                                                            File.WriteAllBytes(code_path, data.get_content());
                                                                            refresh = true;
                                                                        }
                                                                    } else {
                                                                        File.WriteAllBytes(code_path, data.get_content());
                                                                        refresh = true;
                                                                    }
                                                                }
                                                            }
                                                        } catch (System.Exception) {

                                                        }
                                                    }
                                                }
                                            }

                                            builder.dispose();
                                        } else {
                                            failed = true;
                                        }
                                        err_msg.Add(m_err_msg);
                                    }

                                    sheet.dispose();
                                } else {
                                    err_msg.Add(m_err_msg);
                                    failed = true;
                                }
                            } else {
                                failed = true;
                                err_msg.Add($"sheet \'{item.sheet_name}\' does not exist");
                            }
                        } else {
                            err_msg.Add(null);
                        }
                    }
                    self.assets = assets.ToArray();
                }
                file.dispose();
            }

            self.errMsgs = err_msg;

            if (failed) {
                ctx.LogImportError($"import \'{ctx.assetPath}\' failed", self);
            }

            

            if (refresh) {
                EditorApplication.delayCall += AssetDatabase.Refresh;
            }

            var asset_path = ctx.assetPath;
            EditorApplication.delayCall += () => ExcelTableImporterEditor.notify_imported(asset_path);
        }

        public override bool SupportsRemappedAssetType(System.Type type) {
            return type == typeof(BinaryAsset);
        }

        public readonly static ExcelTableUtility.ExternalParsers parsers = new ExcelTableUtility.ExternalParsers();

        public (TableConfig, int) find_config(string sheet_name) {
            if (items != null) {
                for (int i = 0; i < items.Count; ++i) {
                    var item = items[i];
                    if (item.sheet_name == sheet_name) {
                        return (item, i);
                    }
                }
            }
            return (null, -1);
        }

        void ExcelTableUtility.IOutput.output(string content) {

            if (string.IsNullOrEmpty(m_err_msg)) {
                m_err_msg = content;
            } else {
                m_err_msg += "\n" + content;
            }
            
        }

        void ExcelTableUtility.IOutput.info(string name, uint id) {
            try {
                m_sheets.Add(name, id);
            } catch (System.ArgumentException) {

            }
        }

        private Dictionary<string, uint> m_sheets = new Dictionary<string, uint>();
        [System.NonSerialized]
        private string m_err_msg;

    }
}