
using UnityEngine;
using UnityEditor;

namespace GraphNode.Editor {

    public class GraphResources : ScriptableObject {
        
        private static GraphResources s_current;
        internal static GraphResources current {
            get {
                if (s_current == null) {
                    s_current = CreateInstance<GraphResources>();
                }
                return s_current;
            }
        }

        public static (Texture2D dot, Texture2D dot_outer) dot_tex {
            get {
                var cur = current;
                return (cur.node_dot_tex, cur.node_dot_outer_tex);
            }
        }

        [SerializeField]
        private Texture2D node_tex = null;
        [SerializeField]
        internal Texture2D node_selected_tex = null;
        [SerializeField]
        private Texture2D node_dot_tex = null;
        [SerializeField]
        private Texture2D node_dot_outer_tex = null;

        public static Texture2D grid_texture {
            get {
                if (s_grid_texture == null) {
                    s_grid_texture = generate_grid_texture(new Color(0.45f, 0.45f, 0.45f), new Color(0.18f, 0.18f, 0.18f));
                }
                return s_grid_texture;
            }
        }

        public static Texture2D cross_texture {
            get {
                if (s_cross_texture == null) {
                    s_cross_texture = generate_cross_texture(new Color(0.55f, 0.55f, 0.55f));
                }
                return s_cross_texture;
            }
        }


        private static Texture2D generate_grid_texture(Color line, Color bg) {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++) {
                for (int x = 0; x < 64; x++) {
                    Color col = bg;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, bg, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, bg, 0.35f);
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        private static Texture2D generate_cross_texture(Color line) {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++) {
                for (int x = 0; x < 64; x++) {
                    Color col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Cross";
            tex.Apply();
            return tex;
        }

        private static Texture2D s_grid_texture, s_cross_texture;

        public class Styles {
            public readonly GUIStyle node_header, node_body;
            public readonly GUIStyle node_selected_body, node_selected_outer;
            public readonly GUIStyle node_input_port, node_output_port;
            public readonly GUIStyle node_input_desc, node_output_desc;
            public readonly GUIStyle menu_item;
            public readonly GUIStyle comment, red_label;

            public Styles() {
                var current = GraphResources.current;
                node_header = new GUIStyle();
                node_header.alignment = TextAnchor.MiddleCenter;
                node_header.fontStyle = FontStyle.Bold;
                node_header.normal.textColor = Color.white;
                node_header.fixedHeight = 30;

                node_body = new GUIStyle();
                node_body.normal.background = current.node_tex;
                node_body.border = new RectOffset(32, 32, 32, 32);
                node_body.padding = new RectOffset(16, 16, 4, 16);

                node_selected_body = new GUIStyle();
                node_selected_body.normal.background = current.node_tex;
                node_selected_body.border = new RectOffset(32, 32, 32, 32);
                node_selected_body.padding = new RectOffset();

                node_selected_outer = new GUIStyle();
                node_selected_outer.normal.background = current.node_selected_tex;
                node_selected_outer.border = new RectOffset(32, 32, 32, 32);
                node_selected_outer.padding = new RectOffset(16, 16, 4, 16);

                node_input_port = new GUIStyle();
                node_input_port.alignment = TextAnchor.MiddleLeft;
                node_input_port.fixedHeight = 24;
                node_input_port.padding = new RectOffset(2, 2, 0, 0);

                node_output_port = new GUIStyle();
                node_output_port.alignment = TextAnchor.MiddleRight;
                node_output_port.fixedHeight = 24;
                node_output_port.padding = new RectOffset(2, 2, 0, 0);

                node_input_desc = new GUIStyle(EditorStyles.miniLabel);
                node_input_desc.alignment = TextAnchor.UpperLeft;
                node_input_desc.normal.textColor = new Color32(128, 128, 128, 255);

                node_output_desc = new GUIStyle(EditorStyles.miniLabel);
                node_output_desc.alignment = TextAnchor.UpperRight;
                node_output_desc.normal.textColor = new Color32(128, 128, 128, 255);

                menu_item = new GUIStyle(GUI.skin.button);
                menu_item.alignment = TextAnchor.MiddleLeft;

                comment = new GUIStyle(EditorStyles.label);
                comment.fontStyle = FontStyle.Italic;
                comment.normal.textColor = new Color32(192, 192, 192, 255);

                red_label = new GUIStyle(GUI.skin.label);
                red_label.normal.textColor = Color.red;
            }
        }

        private static Styles s_styles = null;

        public static Styles styles {
            get {
                if (s_styles == null) {
                    s_styles = new Styles();
                }
                return s_styles;
            }
        }

        public static string get_nicify_name(string name) {
            return ObjectNames.NicifyVariableName(name);
        }
    }

}