using UnityEngine;
using World_Formal.DS;

namespace World_Formal.Caravans.Devices
{
    public class Device : ITarget
    {
        public AutoCode.Tables.Device.Record _desc;

        public int current_hp;
        public AutoCode.Tables.Device.e_slotType type;

        public bool is_valid = false;

        public string spine_anim_name = "";

        public Vector2 position;
        public Vector2 direction;

        Vector2 DS.ITarget.Position => position;

        Collider2D ITarget.collider => throw new System.NotImplementedException();

        //==================================================================================================

        public Device(uint id, AutoCode.Tables.Device.e_slotType type)
        {
            init(id);
            this.type = type;
        }


        public Device(uint id)
        {
            init(id);
        }


        public Device(uint id,uint rank)
        {
            init(id, rank);
        }


        public Device(uint id,uint rank, AutoCode.Tables.Device.e_slotType type)
        {
            init(id,rank);
            this.type = type;
        }


        void init(uint id,uint rank = 0)
        {
            if (!DB.instance.device.try_get(id,rank, out var r))
            {
                if (id != 0) //0在编辑器里代表无装备
                    UnityEngine.Debug.LogWarning($"编号{id}设备不存在，检查配置文件");
                    return;
            }

            this._desc = r;
            this.current_hp = r.f_hp;
            this.is_valid = true;
        }


        public (string, string) get_pfb_path(AutoCode.Tables.Device.e_slotType type)
        {
            _desc.f_slot_and_prefeb.try_get_value(type, out var e);
            return e;
        }

        void DS.ITarget.hurt(int dmg)
        {
            current_hp -= dmg;
        }
    }
}

