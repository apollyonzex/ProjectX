using UnityEditor;
using UnityEngine;

namespace Editor.DIY_Editor
{
    public abstract class Root : MonoBehaviour
    {
        public abstract void save_asset();
        public abstract void load_asset();
    }


    public abstract class Root<T> : Root where T : ScriptableObject
    {
        public T asset;

        //==================================================================================================

        public sealed override void save_asset()
        {
            if (asset == null)
            {
                Debug.LogWarning("保存失败：没有目标asset");
                return;
            }

            save_asset(asset);
            EditorUtility.SetDirty(asset);
        }


        protected abstract void save_asset(T asset);
    }
}

