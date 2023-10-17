using System.Text.RegularExpressions;
using UnityEditor;

namespace Editor.AutoCreators
{
    public class View
    {
        static string path = "Assets/Editor/Templates/View_Template.cs.txt";

        //==================================================================================================

        [MenuItem("Assets/AutoScript/View", false, -1)]
        public static void EXE()
        {
            CreateScriptByTemplate.EXE(path, create_file_name, create_diy_fields);
        }


        static string create_file_name(string folder_name)
        {
            return $"{folder_name.TrimEnd('s')}View";
        }


        static void create_diy_fields(ref string txt, string folder_name)
        {
            var e = folder_name.TrimEnd('s');
            var iview = $"I{e}View";
            txt = Regex.Replace(txt, "#iview#", iview);

            var mgr = $"{e}Mgr";
            txt = Regex.Replace(txt, "#mgr#", mgr);
        }
    }
}

