
using UnityEngine;
using UnityEditor;

namespace Foundation.Editor {


    [CustomPropertyDrawer(typeof(BuildAssetBundleOptions))]
    public class BuildAssetBundleOptionsDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            //EditorGUI.BeginProperty(position, label, property);

            property.intValue = (int)(BuildAssetBundleOptions)EditorGUI.EnumFlagsField(position, property.name, (BuildAssetBundleOptions)property.intValue);

            //EditorGUI.EndProperty();
        }

    }

}