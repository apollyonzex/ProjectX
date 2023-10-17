
using System.IO;
using UnityEngine;

namespace Foundation.Tables {

    public interface ITable {
        void load_from(BinaryReader reader);
    }

    public static class ITableExtension {
        public static bool load_from(this ITable table, TextAsset asset) {
            return table.load_from(asset.bytes);
        }

        public static bool load_from(this ITable table, BinaryAsset asset) {
            return table.load_from(asset.bytes);
        }

        public static bool load_from(this ITable table, byte[] bytes) {
            using (var stream = new MemoryStream(bytes)) {
                try {
                    table.load_from(new BinaryReader(stream));
                } catch (System.Exception e) {
                    Debug.LogError(e);
                    return false;
                }
            }
            return true;
        }
    }

}