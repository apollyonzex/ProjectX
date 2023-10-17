using Foundation;
using Foundation.Tables;

namespace CaravanEnhanced
{
    public class CaravanFunc
    {

        public enum slotType
        {
            top = 0,
            bottom = 1,
            front = 2,
            back = 3,
            back_top = 4,
            front_top = 5,
            wheel,
        }

        public static AutoCode.Tables.Item.Record TryGetItemData(uint id)
        {
            AutoCode.Tables.Item item = new AutoCode.Tables.Item();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "item", out var asset);
            item.load_from(asset);
            item.try_get(id, out var t);
            return t;
        }

        public static AutoCode.Tables.Caravanbody.Record TryGetBodyData(uint id)
        {
            AutoCode.Tables.Caravanbody body = new AutoCode.Tables.Caravanbody();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "caravanbody", out var asset);
            body.load_from(asset);
            body.try_get(id, out var t);
            return t;
        }

        public static Item TryMakeItem(uint id)
        {
            var record = TryGetItemData(id);
            if (record == null)
            {
                return null;
            }
            Item t = new Item
            {
                id = record.f_id,
                hp = record.f_hp,
                name = record.f_name,
                current_hp = record.f_hp,
                description = record.f_desc,
                graph_path = record.f_graph_path,
                icon_path = record.f_icon,
                driving_speed_limit = record.f_driving_speed_limit,
                driving_acc = record.f_driving_acc,
                braking_acc = record.f_braking_acc,
                descend_speed_limit = record.f_descend_speed_limit,
                damage = record.f_damage,
                size = record.f_size,
            };
            t.height = record.f_wheel_height ?? 0;

            foreach (var r in record.f_slot_and_prefeb)
            {
                var e = get_normal_path(r.Item2);
                t.item_paths.Add(r.Item1.value, e);
                t.item_battle_paths.Add(r.Item1.value, r.value);
            }

            foreach (var tag in record.f_tags)
            {
                t.tags.Add(tag);
            }
            return t;
        }


        public static AutoCode.Tables.Item TryGetItemData()
        {
            AutoCode.Tables.Item item = new AutoCode.Tables.Item();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "item", out var asset);
            item.load_from(asset);
            return item;
        }

        /// <summary>
        /// 临时
        /// 获取非战斗状态下的设备prefab路径
        /// </summary>
        static (string, string) get_normal_path((string, string) v)
        {
            var strs = v.Item2.Split('/');
            var e0 = strs[0];
            var e1 = strs[1];

            e0 = $"{e0}_normal";

            v = (v.Item1, $"{e0}/{e1}");
            return v;
        }
    }
}
