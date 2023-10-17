using UnityEditor;

namespace Editor.AutoCreators
{
    public class Cell
    {
        static string path = "Assets/Editor/Templates/Cell_Template.cs.txt";

        //==================================================================================================

        [MenuItem("Assets/AutoScript/Cell", false, -1)]
        public static void EXE()
        {
            CreateScriptByTemplate.EXE(path, create_file_name);
        }


        static string create_file_name(string folder_name)
        {
            return $"{folder_name.TrimEnd('s')}";
        }
    }
}

