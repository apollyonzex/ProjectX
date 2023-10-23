using System.Text.RegularExpressions;
using UnityEditor;

namespace Editor.AutoCreators
{
    public class Mgr
    {
        static string path = "Assets/Editor/Templates/Mgr_Template.cs.txt";

        //==================================================================================================

        [MenuItem("Assets/AutoScript/Mgr", false, -1)]
        public static void EXE()
        {
            CreateScriptByTemplate.EXE(path, create_file_name, create_diy_fields);
        }


        static string create_file_name(string folder_name)
        {
            return $"{folder_name.TrimEnd('s')}Mgr";
        }


        static void create_diy_fields(ref string txt, string folder_name)
        {
            var cell = folder_name.TrimEnd('s');
            txt = Regex.Replace(txt, "#cell#", cell);

            var iview = $"I{cell}View";
            txt = Regex.Replace(txt, "#iview#", iview);
        }
    }
}

